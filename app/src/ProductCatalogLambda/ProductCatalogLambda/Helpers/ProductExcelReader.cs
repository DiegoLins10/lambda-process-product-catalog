using ClosedXML.Excel;
using ProductCatalogLambda.Interfaces;
using ProductCatalogLambda.Models;

namespace ProductCatalogLambda.Services.Implementations;

public class ProductExcelReader : IProductExcelReader
{
    public List<Product> ReadProducts(Stream file)
    {
        var products = new List<Product>();

        using var workbook = new XLWorkbook(file);
        var worksheet = workbook.Worksheet(1);
        var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // skip header

        foreach (var row in rows)
        {
            var product = new Product
            {
                SKU = row.Cell(1).GetString(),
                Name = row.Cell(2).GetString(),
                Description = row.Cell(3).GetString(),
                Price = row.Cell(4).GetValue<decimal>(),
                Category = row.Cell(5).GetString()
            };
            products.Add(product);
        }

        return products;
    }
}