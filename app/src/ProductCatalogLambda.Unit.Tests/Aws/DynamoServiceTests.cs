using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Moq;
using ProductCatalogLambda.Aws;
using ProductCatalogLambda.Models;
using Xunit;

namespace ProductCatalogLambda.Unit.Tests.Aws
{
    public class DynamoServiceTests
    {
        private readonly Mock<IAmazonDynamoDB> _dynamoMock;
        private readonly DynamoService _service;

        public DynamoServiceTests()
        {
            _dynamoMock = new Mock<IAmazonDynamoDB>();
            _service = new DynamoService(_dynamoMock.Object);
        }

        [Fact]
        public async Task UpsertProductAsync_Should_Call_PutItemAsync_With_Correct_Parameters()
        {
            // Arrange
            var product = new Product
            {
                SKU = "123",
                Name = "Produto Teste",
                Description = "Descrição de teste",
                Price = 99.99m,
                Category = "Eletrônicos"
            };

            PutItemRequest? capturedRequest = null;

            _dynamoMock
                .Setup(d => d.PutItemAsync(It.IsAny<PutItemRequest>(), It.IsAny<CancellationToken>()))
                .Callback<PutItemRequest, CancellationToken>((req, _) => capturedRequest = req)
                .ReturnsAsync(new PutItemResponse());

            // Act
            await _service.UpsertProductAsync(product);

            // Assert
            _dynamoMock.Verify(
                d => d.PutItemAsync(It.IsAny<PutItemRequest>(), It.IsAny<CancellationToken>()),
                Times.Once
            );

            Assert.NotNull(capturedRequest);
            Assert.Equal("ProductCatalog", capturedRequest!.TableName);

            var item = capturedRequest.Item;
            Assert.Equal(product.SKU, item["SKU"].S);
            Assert.Equal(product.Name, item["Name"].S);
            Assert.Equal(product.Description, item["Description"].S);
            Assert.Equal(product.Category, item["Category"].S);
            Assert.Equal(product.Price.ToString("F2", CultureInfo.InvariantCulture), item["Price"].N);
        }

        [Fact]
        public async Task UpsertProductAsync_Should_Throw_When_DynamoDB_Fails()
        {
            // Arrange
            var product = new Product
            {
                SKU = "999",
                Name = "Erro Teste",
                Description = "Produto com erro",
                Price = 10,
                Category = "Falha"
            };

            _dynamoMock
                .Setup(d => d.PutItemAsync(It.IsAny<PutItemRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new AmazonDynamoDBException("Erro simulado no DynamoDB"));

            // Act & Assert
            await Assert.ThrowsAsync<AmazonDynamoDBException>(() => _service.UpsertProductAsync(product));
        }
    }
}