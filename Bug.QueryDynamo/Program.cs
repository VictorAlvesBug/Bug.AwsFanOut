using Bug.Domain.Entities;
using Bug.Infrastructure.Services;
using Newtonsoft.Json;

var _dynamoDbService = new DynamoDbService();

await SaveIntegrationAsync();

//await GetIntegrationAsync();


async Task SaveIntegrationAsync()
{
	var messageGroupId = "groupId-0";

	var itemToPut = new Integration
	{
		PK = Guid.NewGuid().ToString(),
		SK = (decimal)DateTime.Now.ToOADate(),
		Status = IntegrationStatus.Pending,
		Body = JsonConvert.SerializeObject(new { MessageGroupId = messageGroupId, AnotherField = DateTime.Now }),
		FifoMessageGroupId = messageGroupId
	};

	await _dynamoDbService.PutItemAsync(itemToPut);


	var item = await _dynamoDbService.GetItemAsync(itemToPut.PK, itemToPut.SK);
}

async Task GetIntegrationAsync()
{
	var item = await _dynamoDbService.GetItemAsync("9cf1ce02-a776-4179-bbbc-e38758468ce6", (decimal)45689.7937208565);

	Console.WriteLine(JsonConvert.SerializeObject(item, Formatting.Indented));
}
