using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime.CredentialManagement;
using Bug.Domain.Entities;
using Bug.Helpers.Extensions;
using Newtonsoft.Json;

namespace Bug.Infrastructure.Services;

public class DynamoDbService
{
	private readonly AmazonDynamoDBClient _dynamoDbClient;
	private const string TableName = "bug-integration";

	public DynamoDbService()
	{
		var chain = new CredentialProfileStoreChain();

		if (chain.TryGetAWSCredentials("default", out var credentials))
		{
			_dynamoDbClient = new AmazonDynamoDBClient(credentials, RegionEndpoint.USEast1);
		}

		throw new Exception("Não foi possível utilizar as credenciais para conectar à AWS");
	}

	public async Task<Integration?> GetItemAsync(string pk, decimal sk)
	{
		try
		{
			var strSk = sk.ToString().Replace(",", ".");

			var request = new GetItemRequest
			{
				TableName = TableName,
				Key = new Dictionary<string, AttributeValue>
				{
					{ nameof(Integration.PK), new AttributeValue { S = pk } },
					{ nameof(Integration.SK), new AttributeValue { N = strSk } },
				}
			};

			var response = await _dynamoDbClient.GetItemAsync(request);

			var json = response.Item.ToJson();

			return JsonConvert.DeserializeObject<Integration>(json);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Erro ao recuperar item: {ex.Message}");
		}

		return null;
	}

	public async Task PutItemAsync(Integration integration)
	{
		try
		{
			var item = new Dictionary<string, AttributeValue>
			{
				{ nameof(Integration.PK), new AttributeValue { S = integration.PK } },
				{ nameof(Integration.SK), new AttributeValue { N = integration.SK.ToString().Replace(",", ".") } },
				{ nameof(Integration.Status), new AttributeValue { N = ((int)integration.Status).ToString() } },
				{ nameof(Integration.Body), new AttributeValue { S = integration.PK } },
				{ nameof(Integration.FifoMessageGroupId), new AttributeValue { S = integration.FifoMessageGroupId } },
			};

			var request = new PutItemRequest
			{
				TableName = TableName,
				Item = item
			};

			await _dynamoDbClient.PutItemAsync(request);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Erro ao salvar item: {ex.Message}");
		}
	}
}
