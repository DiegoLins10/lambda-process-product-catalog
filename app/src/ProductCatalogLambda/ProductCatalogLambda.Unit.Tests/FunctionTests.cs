using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using ProductCatalogLambda.Interfaces;
using System.Text.Json;

namespace ProductCatalogLambda.Unit.Tests
{
    public class FunctionTests
    {
        private readonly Mock<IProductProcessingService> _processingServiceMock;
        private readonly Mock<ILambdaContext> _contextMock;
        private readonly Mock<ILambdaLogger> _loggerMock;

        public FunctionTests()
        {
            _processingServiceMock = new Mock<IProductProcessingService>();
            _contextMock = new Mock<ILambdaContext>();
            _loggerMock = new Mock<ILambdaLogger>();

            _contextMock.Setup(c => c.Logger).Returns(_loggerMock.Object);
        }

        private Function CreateFunctionWithMockedService()
        {
            // Injeta manualmente o mock de IProductProcessingService
            var services = new ServiceCollection();
            services.AddSingleton(_processingServiceMock.Object);
            var provider = services.BuildServiceProvider();

            // Cria instância da Function usando o mock
            return new FunctionForTest(provider.GetRequiredService<IProductProcessingService>());
        }

        // Classe interna herdando de Function para permitir injeção direta no construtor
        private class FunctionForTest : Function
        {
            public FunctionForTest(IProductProcessingService service)
            {
                typeof(Function)
                    .GetField("_processingService", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                    .SetValue(this, service);
            }
        }

        [Fact]
        public async Task FunctionHandler_Should_Process_S3_Event_Successfully()
        {
            // Arrange
            var function = CreateFunctionWithMockedService();

            var eventJson = """
            {
              "source": "aws.s3",
              "detail-type": "Object Created: Put",
              "detail": {
                "bucket": { "name": "catalogo-produtos-uploads" },
                "object": { "key": "produtos_fornecedor_XYZ.xlsx" }
              }
            }
            """;

            dynamic eventObj = JsonSerializer.Deserialize<JsonElement>(eventJson);

            _processingServiceMock
                .Setup(p => p.ProcessFileAsync("catalogo-produtos-uploads", "produtos_fornecedor_XYZ.xlsx"))
                .Returns(Task.CompletedTask);

            // Act
            var response = await function.FunctionHandler(eventObj, _contextMock.Object);

            // Assert
            Assert.Equal(201, response.StatusCode);
            Assert.True(response.Sucess);
            Assert.Equal("OK", response.Status);
            _processingServiceMock.Verify(p => p.ProcessFileAsync("catalogo-produtos-uploads", "produtos_fornecedor_XYZ.xlsx"), Times.Once);
        }

        [Fact]
        public async Task FunctionHandler_Should_Return_BadRequest_For_NonEventBridge()
        {
            // Arrange
            var function = CreateFunctionWithMockedService();

            var invalidEventJson = """{ "someKey": "value" }""";
            dynamic eventObj = JsonSerializer.Deserialize<JsonElement>(invalidEventJson);

            // Act
            var response = await function.FunctionHandler(eventObj, _contextMock.Object);

            // Assert
            Assert.Equal(400, response.StatusCode);
            Assert.False(response.Sucess);
            Assert.Equal("BadRequest", response.Status);
            _processingServiceMock.Verify(p => p.ProcessFileAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task FunctionHandler_Should_Handle_Exception_Gracefully()
        {
            // Arrange
            var function = CreateFunctionWithMockedService();

            var eventJson = """
            {
              "source": "aws.s3",
              "detail-type": "Object Created: Put",
              "detail": {
                "bucket": { "name": "catalogo-produtos-uploads" },
                "object": { "key": "produtos_fornecedor_XYZ.xlsx" }
              }
            }
            """;

            dynamic eventObj = JsonSerializer.Deserialize<JsonElement>(eventJson);

            _processingServiceMock
                .Setup(p => p.ProcessFileAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Erro simulado"));

            // Act
            var response = await function.FunctionHandler(eventObj, _contextMock.Object);

            // Assert
            Assert.Equal(500, response.StatusCode);
            Assert.False(response.Sucess);
            Assert.Equal("Error", response.Status);
            Assert.Contains("Erro no processamento", response.Message);
        }
    }
}