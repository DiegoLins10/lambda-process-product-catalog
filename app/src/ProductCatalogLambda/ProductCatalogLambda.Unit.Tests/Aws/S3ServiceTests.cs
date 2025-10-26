using Amazon.S3;
using Amazon.S3.Model;
using Moq;
using ProductCatalogLambda.Aws;
using System.Text;

namespace ProductCatalogLambda.Unit.Tests.Aws
{
    public class S3ServiceTests
    {
        private readonly Mock<IAmazonS3> _s3ClientMock;
        private readonly S3Service _service;

        public S3ServiceTests()
        {
            _s3ClientMock = new Mock<IAmazonS3>();
            _service = new S3Service(_s3ClientMock.Object);
        }

        [Fact]
        public async Task GetObjectAsync_Should_Return_MemoryStream_With_Same_Content()
        {
            // Arrange
            var bucketName = "catalogo-produtos-uploads";
            var key = "produtos_fornecedor_XYZ.xlsx";
            var expectedContent = "conteudo de teste";

            var sourceStream = new MemoryStream(Encoding.UTF8.GetBytes(expectedContent));

            var response = new GetObjectResponse
            {
                BucketName = bucketName,
                Key = key,
                ResponseStream = sourceStream
            };

            _s3ClientMock
                .Setup(s => s.GetObjectAsync(bucketName, key, It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act
            var resultStream = await _service.GetObjectAsync(bucketName, key);

            // Assert
            Assert.NotNull(resultStream);
            resultStream.Position = 0;

            using var reader = new StreamReader(resultStream);
            var actualContent = await reader.ReadToEndAsync();

            Assert.Equal(expectedContent, actualContent);
            _s3ClientMock.Verify(s => s.GetObjectAsync(bucketName, key, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetObjectAsync_Should_Throw_When_S3_Fails()
        {
            // Arrange
            _s3ClientMock
                .Setup(s => s.GetObjectAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new AmazonS3Exception("Erro simulado no S3"));

            // Act & Assert
            await Assert.ThrowsAsync<AmazonS3Exception>(() => _service.GetObjectAsync("bucket", "key"));
        }
    }
}