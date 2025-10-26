namespace ProductCatalogLambda.Interfaces
{
    public interface IProductProcessingService
    {
        Task ProcessFileAsync(string bucketName, string key);
    }
}