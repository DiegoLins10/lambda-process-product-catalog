using ProductCatalogLambda.Models;

namespace ProductCatalogLambda.Interfaces;

public interface IDynamoService
{
    Task UpsertProductAsync(Product product);
}