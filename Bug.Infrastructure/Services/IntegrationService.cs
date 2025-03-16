using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime.CredentialManagement;
using Amazon.SimpleNotificationService;
using Bug.Domain.Entities.Integration;
using Bug.Helpers.Extensions;
using Bug.Helpers.Utils;
using Newtonsoft.Json;

namespace Bug.Infrastructure.Services;

public class IntegrationService
{
	private readonly AmazonDynamoDBClient _dynamoDbClient;
	private const string TableName = "bug-integration";

	public IntegrationService()
	{
		/*var chain = new CredentialProfileStoreChain();

		if (!chain.TryGetAWSCredentials("default", out var credentials))
			throw new Exception("Não foi possível utilizar as credenciais para conectar à AWS");*/

		_dynamoDbClient = new AmazonDynamoDBClient();
	}

	public async Task<List<Integration>> GetAllAsync()
	{
		try
		{
			var request = new ScanRequest
			{
				TableName = TableName
			};

			var response = await _dynamoDbClient.ScanAsync(request);

			return response.Items
				.ConvertAll(item => JsonConvert.DeserializeObject<Integration>(item.ToJson()) ?? new Integration());
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Erro ao recuperar item: {ex.Message}");
		}

		return [];
	}

	public async Task<Integration?> GetAsync(string pk, string sk)
	{
		try
		{
			var request = new GetItemRequest
			{
				TableName = TableName,
				Key = new Dictionary<string, AttributeValue>
				{
					{ nameof(Integration.PK), new AttributeValue { S = pk } },
					{ nameof(Integration.SK), new AttributeValue { S = sk } },
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

	public async Task PutAsync(IntegrationToPut integrationToPut)
	{
		try
		{
			var integration = new Integration(integrationToPut);

			var item = new Dictionary<string, AttributeValue>
			{
				{ nameof(Integration.PK), integration.PK.ConvertToAttributeValue() },
				{ nameof(Integration.SK), integration.SK.ConvertToAttributeValue() },
				{ nameof(Integration.Status), integration.Status.ConvertToAttributeValue() },
				{ nameof(Integration.Type), integration.Type.ConvertToAttributeValue() },
				{ nameof(Integration.Body), integration.Body.ConvertToAttributeValue() },
				{ nameof(Integration.FifoMessageGroupId), integration.FifoMessageGroupId.ConvertToAttributeValue() },
				{ nameof(Integration.CreatedAt), integration.CreatedAt.ToString("o").ConvertToAttributeValue() },
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

	public async Task UpdateAttributesAsync(Integration integration, List<string> attributesToUpdate = null)
	{
		try
		{
			if (attributesToUpdate == null)
			{
				attributesToUpdate = integration.GetType().GetProperties()
					.Select(prop => prop.Name)
					.Where(propName => !new List<string> { nameof(Integration.PK), nameof(Integration.SK) }.Contains(propName))
					.ToList();
			}

			var request = new UpdateItemRequest
			{
				TableName = TableName,
				Key = new Dictionary<string, AttributeValue>
				{
					{ nameof(Integration.PK), integration.PK.ConvertToAttributeValue() },
					{ nameof(Integration.SK), integration.SK.ConvertToAttributeValue() }
				},
				UpdateExpression = attributesToUpdate.BuildUpdateExpression(),
				ExpressionAttributeNames = attributesToUpdate.BuildExpressionAttributeNames(),
				ExpressionAttributeValues = attributesToUpdate.BuildExpressionAttributeValues(integration)
			};

			await _dynamoDbClient.UpdateItemAsync(request);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Erro ao atualizar atributo: {ex.Message}");
		}
	}

}
