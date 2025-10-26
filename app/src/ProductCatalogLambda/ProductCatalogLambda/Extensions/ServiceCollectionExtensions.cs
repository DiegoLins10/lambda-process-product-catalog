using Amazon.DynamoDBv2;
using Amazon.S3;
using Microsoft.Extensions.DependencyInjection;
using ProductCatalogLambda.Aws;
using ProductCatalogLambda.Interfaces;

namespace ProductCatalogLambda.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddProductCatalogServices(this IServiceCollection services)
    {
        // AWS Clients
        services.AddSingleton<IAmazonS3, AmazonS3Client>();
        services.AddSingleton<IAmazonDynamoDB, AmazonDynamoDBClient>();

        // AWS Services
        services.AddSingleton<IS3Service, S3Service>();
        services.AddSingleton<IDynamoService, DynamoService>();

        // Helper XLSX
        services.AddSingleton<XlsxHelper>();

        // Processing Service
        services.AddSingleton<ProductProcessingService>();

        return services;
    }
}