using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using ProductCatalogLambda.Interfaces;
using ProductCatalogLambda.Models;
using System.Globalization;

namespace ProductCatalogLambda.Aws;

public class DynamoService : IDynamoService
{
    private readonly IAmazonDynamoDB _dynamoClient;
    private readonly string _tableName = "ProductCatalog";

    public DynamoService(IAmazonDynamoDB dynamoClient)
    {
        _dynamoClient = dynamoClient;
    }

    public async Task UpsertProductAsync(Product product)
    {
        var item = new Dictionary<string, AttributeValue>
        {
            ["SKU"] = new AttributeValue { S = product.SKU },
            ["Name"] = new AttributeValue { S = product.Name },
            ["Description"] = new AttributeValue { S = product.Description },
            ["Price"] = new AttributeValue { N = product.Price.ToString("F2", CultureInfo.InvariantCulture) },
            ["Category"] = new AttributeValue { S = product.Category }
        };

        var request = new PutItemRequest
        {
            TableName = _tableName,
            Item = item
        };

        await _dynamoClient.PutItemAsync(request);
    }
}