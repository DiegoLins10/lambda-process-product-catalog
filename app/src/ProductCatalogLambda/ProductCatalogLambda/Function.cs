using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using ProductCatalogLambda.Extensions;
using ProductCatalogLambda.Interfaces;
using ProductCatalogLambda.Models;
using System.Text.Json;

// Assembly attribute para habilitar logs do Lambda
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace ProductCatalogLambda;

public class Function
{
    private readonly IProductProcessingService _processingService;

    public Function()
    {
        // Configuração DI
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddProductCatalogServices();

        var serviceProvider = serviceCollection.BuildServiceProvider();

        _processingService = serviceProvider.GetRequiredService<IProductProcessingService>();
    }

    public async Task<ProcessResponse> FunctionHandler(dynamic eventBridgeEvent, ILambdaContext context)
    {
        try
        {
            // Serializa o dynamic para JSON
            string json = JsonSerializer.Serialize(eventBridgeEvent);

            // Desserializa para JsonDocument para acessar de forma segura
            using JsonDocument doc = JsonDocument.Parse(json);
            JsonElement root = doc.RootElement;

            // Verifica se é EventBridge
            if (!root.TryGetProperty("source", out var sourceProp) ||
                !root.TryGetProperty("detail-type", out var detailTypeProp))
            {
                context.Logger.LogWarning("Evento não é EventBridge. Ignorado.");
                return new ProcessResponse
                {
                    StatusCode = 400,
                    Status = "BadRequest",
                    Message = "Evento não é EventBridge. Ignorado."
                };
            }

            string source = sourceProp.GetString() ?? "";
            string detailType = detailTypeProp.GetString() ?? "";

            context.Logger.LogLine($"Evento EventBridge detectado: source={source}, detail-type={detailType}");

            // Se não for s3 nao processa.
            if (source != "aws.s3" || !detailType.Contains("Object Created", StringComparison.OrdinalIgnoreCase))
            {
                context.Logger.LogWarning("Evento EventBridge não é S3 ObjectCreated, ignorado");

                return new ProcessResponse
                {
                    StatusCode = 400,
                    Status = "BadRequest",
                    Message = "Evento EventBridge não é S3 ObjectCreated, ignorado"
                };
            }

            // Processa o arquivo recebido.
            var detail = root.GetProperty("detail");
            string bucketName = detail.GetProperty("bucket").GetProperty("name").GetString();
            string key = detail.GetProperty("object").GetProperty("key").GetString();

            context.Logger.LogLine($"Processando arquivo S3: Bucket={bucketName}, Key={key}");
            await _processingService.ProcessFileAsync(bucketName, key);

            context.Logger.LogInformation($"Arquivo processado com sucesso");
            return new ProcessResponse
            {
                StatusCode = 201,
                Status = "OK",
                Message = "Arquivo processado com sucesso",
                Sucess = true
            };
        }
        catch (Exception ex)
        {
            context.Logger.LogError($"Erro durante processamento: {ex.Message}");
            return new ProcessResponse
            {
                StatusCode = 500,
                Status = "Error",
                Message = $"Erro no processamento: {ex.Message}",
                Sucess = false
            };
        }
    }
}