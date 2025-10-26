using ProductCatalogLambda.Models;

namespace ProductCatalogLambda.Interfaces
{
    public interface IProductExcelReader
    {
        List<Product> ReadProducts(Stream file);
    }
}