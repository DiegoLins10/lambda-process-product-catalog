using ProductCatalogLambda.Events;
using System.Text.Json;

namespace ProductCatalogLambda.Helpers;

public sealed class EventBridgeS3EventParser
{
    public bool TryParse(JsonElement root, out S3ObjectCreatedEvent eventInfo, out string errorMessage)
    {
        eventInfo = default!;
        errorMessage = string.Empty;

        if (!root.TryGetProperty("source", out var sourceProp) ||
            !root.TryGetProperty("detail-type", out var detailTypeProp))
        {
            errorMessage = "Evento nao e EventBridge. Ignorado.";
            return false;
        }

        string source = sourceProp.GetString() ?? string.Empty;
        string detailType = detailTypeProp.GetString() ?? string.Empty;

        // Garante que apenas eventos esperados de criacao de objeto no S3 sejam processados.
        if (source != "aws.s3" || !detailType.Contains("Object Created", StringComparison.OrdinalIgnoreCase))
        {
            errorMessage = "Evento EventBridge nao e S3 ObjectCreated, ignorado";
            return false;
        }

        // Extracao defensiva para evitar excecao quando detalhe estiver incompleto.
        if (!TryGetString(root, "detail", "bucket", "name", out var bucketName) ||
            !TryGetString(root, "detail", "object", "key", out var key))
        {
            errorMessage = "Evento S3 invalido: bucket/key ausentes.";
            return false;
        }

        eventInfo = new S3ObjectCreatedEvent
        {
            Source = source,
            DetailType = detailType,
            BucketName = bucketName,
            Key = key
        };

        return true;
    }

    private static bool TryGetString(JsonElement root, string level1, string level2, string level3, out string value)
    {
        value = string.Empty;

        if (!root.TryGetProperty(level1, out var p1) ||
            !p1.TryGetProperty(level2, out var p2) ||
            !p2.TryGetProperty(level3, out var p3))
        {
            return false;
        }

        string? raw = p3.GetString();
        if (string.IsNullOrWhiteSpace(raw))
        {
            return false;
        }

        value = raw;
        return true;
    }
}
