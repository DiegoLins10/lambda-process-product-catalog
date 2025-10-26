namespace ProductCatalogLambda.Interfaces;

public interface IS3Service
{
    Task<Stream> GetObjectAsync(string bucketName, string key);
}