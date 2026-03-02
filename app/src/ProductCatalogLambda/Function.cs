using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using ProductCatalogLambda.Extensions;
using ProductCatalogLambda.Helpers;
using ProductCatalogLambda.Interfaces;
using ProductCatalogLambda.Models;
using System.Text.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace ProductCatalogLambda;

public class Function
{
    private readonly IProductProcessingService _processingService;
    private readonly EventBridgeS3EventParser _eventParser;

    public Function() : this(BuildDefaultProcessingService(), new EventBridgeS3EventParser())
    {
    }

    public Function(IProductProcessingService processingService)
        : this(processingService, new EventBridgeS3EventParser())
    {
    }

    public Function(IProductProcessingService processingService, EventBridgeS3EventParser eventParser)
    {
        _processingService = processingService ?? throw new ArgumentNullException(nameof(processingService));
        _eventParser = eventParser ?? throw new ArgumentNullException(nameof(eventParser));
    }

    private static IProductProcessingService BuildDefaultProcessingService()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddProductCatalogServices();
        return serviceCollection.BuildServiceProvider()
                                .GetRequiredService<IProductProcessingService>();
    }

    public async Task<ProcessResponse> FunctionHandler(dynamic eventBridgeEvent, ILambdaContext context)
    {
        try
        {
            string json = JsonSerializer.Serialize(eventBridgeEvent);
            using JsonDocument doc = JsonDocument.Parse(json);

            if (!_eventParser.TryParse(doc.RootElement, out var eventInfo, out var errorMessage))
            {
                context.Logger.LogWarning(errorMessage);
                return ProcessResponseFactory.BadRequest(errorMessage);
            }

            context.Logger.LogLine($"Evento EventBridge detectado: source={eventInfo.Source}, detail-type={eventInfo.DetailType}");
            context.Logger.LogLine($"Processando arquivo S3: Bucket={eventInfo.BucketName}, Key={eventInfo.Key}");

            await _processingService.ProcessFileAsync(eventInfo.BucketName, eventInfo.Key);

            context.Logger.LogInformation("Arquivo processado com sucesso");
            return ProcessResponseFactory.Success("Arquivo processado com sucesso");
        }
        catch (Exception ex)
        {
            context.Logger.LogError($"Erro durante processamento: {ex.Message}");
            return ProcessResponseFactory.Error($"Erro no processamento: {ex.Message}");
        }
    }
}
