using ClosedXML.Excel;
using ProductCatalogLambda.Services.Implementations;

namespace ProductCatalogLambda.Unit.Tests.Helpers
{
    public class ProductExcelReaderTests
    {
        [Fact]
        public void ReadProducts_Should_Return_List_Of_Products_From_Excel()
        {
            // Arrange
            using var stream = new MemoryStream();
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Produtos");
                // Cabeçalho
                worksheet.Cell(1, 1).Value = "SKU";
                worksheet.Cell(1, 2).Value = "Name";
                worksheet.Cell(1, 3).Value = "Description";
                worksheet.Cell(1, 4).Value = "Price";
                worksheet.Cell(1, 5).Value = "Category";

                // Linha 1
                worksheet.Cell(2, 1).Value = "123";
                worksheet.Cell(2, 2).Value = "Produto 1";
                worksheet.Cell(2, 3).Value = "Descrição 1";
                worksheet.Cell(2, 4).Value = 10.50m;
                worksheet.Cell(2, 5).Value = "Eletrônicos";

                // Linha 2
                worksheet.Cell(3, 1).Value = "456";
                worksheet.Cell(3, 2).Value = "Produto 2";
                worksheet.Cell(3, 3).Value = "Descrição 2";
                worksheet.Cell(3, 4).Value = 25.99m;
                worksheet.Cell(3, 5).Value = "Acessórios";

                workbook.SaveAs(stream);
            }

            stream.Position = 0;

            var reader = new ProductExcelReader();

            // Act
            var products = reader.ReadProducts(stream);

            // Assert
            Assert.NotNull(products);
            Assert.Equal(2, products.Count);

            var p1 = products[0];
            Assert.Equal("123", p1.SKU);
            Assert.Equal("Produto 1", p1.Name);
            Assert.Equal("Descrição 1", p1.Description);
            Assert.Equal(10.50m, p1.Price);
            Assert.Equal("Eletrônicos", p1.Category);

            var p2 = products[1];
            Assert.Equal("456", p2.SKU);
            Assert.Equal("Produto 2", p2.Name);
            Assert.Equal("Descrição 2", p2.Description);
            Assert.Equal(25.99m, p2.Price);
            Assert.Equal("Acessórios", p2.Category);
        }

        [Fact]
        public void ReadProducts_Should_Return_Empty_When_Worksheet_Has_Only_Header()
        {
            // Arrange
            using var stream = new MemoryStream();
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Produtos");
                worksheet.Cell(1, 1).Value = "SKU";
                worksheet.Cell(1, 2).Value = "Name";
                worksheet.Cell(1, 3).Value = "Description";
                worksheet.Cell(1, 4).Value = "Price";
                worksheet.Cell(1, 5).Value = "Category";
                workbook.SaveAs(stream);
            }

            stream.Position = 0;

            var reader = new ProductExcelReader();

            // Act
            var products = reader.ReadProducts(stream);

            // Assert
            Assert.NotNull(products);
            Assert.Empty(products);
        }
    }
}