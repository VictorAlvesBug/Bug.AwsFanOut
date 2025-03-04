using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Bug.Domain.Entities;
using Bug.Infrastructure.Services;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Bug.TryNumber;

public class Function
{
	private DynamoDbService _dynamoDbService;

	public Function()
	{
		_dynamoDbService = new DynamoDbService();
	}
	
	/// <summary>
	 /// This method is called for every Lambda invocation. This method takes in an SQS event object and can be used 
	 /// to respond to SQS messages.
	 /// </summary>
	 /// <param name="evnt">The event for the Lambda function handler to process.</param>
	 /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
	 /// <returns></returns>
	public async Task FunctionHandler(SQSEvent evnt, ILambdaContext context)
	{
		context.Logger.LogInformation("Chegou à TryNumber");

		foreach (var message in evnt.Records)
        {
            await ProcessMessageAsync(message, context);
        }
    }

    private async Task ProcessMessageAsync(SQSEvent.SQSMessage message, ILambdaContext context)
    {
        context.Logger.LogInformation($"Processed message {message.Body}");

		var itemToPut = new Integration
		{
			PK = Guid.NewGuid().ToString(),
			SK = (decimal)DateTime.Now.ToOADate(),
			Status = IntegrationStatus.Pending,
			Type = IntegrationType.DiscoveredNumber,
			Body = message.Body,
			CreatedAt = DateTime.Now
		};

		await _dynamoDbService.PutItemAsync(itemToPut);

		// TODO: Do interesting work based on the new message
		//await Task.CompletedTask;
	}
}