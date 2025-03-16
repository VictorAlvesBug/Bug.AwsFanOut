using Amazon.Lambda.Core;
using Amazon.Lambda.DynamoDBEvents;
using Bug.Domain.Entities.Integration;
using Bug.Helpers.Extensions;
using Bug.Infrastructure.Services;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Bug.FwdIntegration;

public class Function
{
	public async Task FunctionHandler(DynamoDBEvent dynamoEvent, ILambdaContext context)
	{
		context.Logger.LogInformation("Chegou à FwdIntegration");

		var snsService = new SnsService();

		foreach (var record in dynamoEvent.Records)
		{
			if (record.EventName == "INSERT")
			{
				var newImage = record.Dynamodb.NewImage;

				var jsonIntegration = newImage.ToJson();
				var integration = JsonConvert.DeserializeObject<Integration>(jsonIntegration);

				if (integration == null)
				{
					throw new Exception("Integração não encontrada");
				}

				context.Logger.LogLine($"Novo item inserido: {jsonIntegration}");

				var topicArn = "arn:aws:sns:us-east-1:571600868266:Bug-FwdIntegrationTopic.fifo";

				var message = JsonConvert.SerializeObject(integration);

				await snsService.PublishFifoMessageAsync(topicArn, integration.Type, message, integration.FifoMessageGroupId);
			}
		}
	}
}
