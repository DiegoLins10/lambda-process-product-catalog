using ProductCatalogLambda.Interfaces;

namespace ProductCatalogLambda.Services;

public class ProductProcessingService : IProductProcessingService
{
    private readonly IS3Service _s3Service;
    private readonly IProductExcelReader _reader;
    private readonly IDynamoService _dynamoService;

    public ProductProcessingService(IS3Service s3Service, IProductExcelReader reader, IDynamoService dynamoService)
    {
        _s3Service = s3Service;
        _reader = reader;
        _dynamoService = dynamoService;
    }

    public async Task ProcessFileAsync(string bucketName, string key)
    {
        using var stream = await _s3Service.GetObjectAsync(bucketName, key);
        var products = _reader.ReadProducts(stream);

        var tasks = new List<Task>();
        foreach (var product in products)
        {
            tasks.Add(_dynamoService.UpsertProductAsync(product));
        }

        await Task.WhenAll(tasks);
    }
}