using Moq;
using ProductCatalogLambda.Interfaces;
using ProductCatalogLambda.Models;
using ProductCatalogLambda.Services;
using System.Text;

namespace ProductCatalogLambda.Unit.Tests.Services
{
    public class ProductProcessingServiceTests
    {
        private readonly Mock<IS3Service> _s3ServiceMock;
        private readonly Mock<IProductExcelReader> _readerMock;
        private readonly Mock<IDynamoService> _dynamoServiceMock;
        private readonly ProductProcessingService _service;

        public ProductProcessingServiceTests()
        {
            _s3ServiceMock = new Mock<IS3Service>();
            _readerMock = new Mock<IProductExcelReader>();
            _dynamoServiceMock = new Mock<IDynamoService>();

            _service = new ProductProcessingService(
                _s3ServiceMock.Object,
                _readerMock.Object,
                _dynamoServiceMock.Object
            );
        }

        [Fact]
        public async Task ProcessFileAsync_Should_Read_And_Upsert_All_Products()
        {
            // Arrange
            const string bucketName = "catalogo-produtos-uploads";
            const string key = "produtos_fornecedor_XYZ.xlsx";

            var stream = new MemoryStream(Encoding.UTF8.GetBytes("fake excel content"));
            var products = new List<Product>
            {
                new() { SKU = "123", Name = "Produto 1" },
                new() { SKU = "456", Name = "Produto 2" }
            };

            _s3ServiceMock
                .Setup(s => s.GetObjectAsync(bucketName, key))
                .ReturnsAsync(stream);

            _readerMock
                .Setup(r => r.ReadProducts(stream))
                .Returns(products);

            _dynamoServiceMock
                .Setup(d => d.UpsertProductAsync(It.IsAny<Product>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.ProcessFileAsync(bucketName, key);

            // Assert
            _s3ServiceMock.Verify(s => s.GetObjectAsync(bucketName, key), Times.Once);
            _readerMock.Verify(r => r.ReadProducts(stream), Times.Once);
            _dynamoServiceMock.Verify(d => d.UpsertProductAsync(It.Is<Product>(p => p.SKU == "123")), Times.Once);
            _dynamoServiceMock.Verify(d => d.UpsertProductAsync(It.Is<Product>(p => p.SKU == "456")), Times.Once);
        }

        [Fact]
        public async Task ProcessFileAsync_Should_Not_Upsert_When_No_Products()
        {
            // Arrange
            const string bucketName = "test-bucket";
            const string key = "empty.xlsx";

            var stream = new MemoryStream();
            var products = new List<Product>();

            _s3ServiceMock
                .Setup(s => s.GetObjectAsync(bucketName, key))
                .ReturnsAsync(stream);

            _readerMock
                .Setup(r => r.ReadProducts(stream))
                .Returns(products);

            // Act
            await _service.ProcessFileAsync(bucketName, key);

            // Assert
            _dynamoServiceMock.Verify(d => d.UpsertProductAsync(It.IsAny<Product>()), Times.Never);
        }
    }
}