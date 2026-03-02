namespace ProductCatalogLambda.Events;

public sealed class S3ObjectCreatedEvent
{
    public required string Source { get; init; }
    public required string DetailType { get; init; }
    public required string BucketName { get; init; }
    public required string Key { get; init; }
}
