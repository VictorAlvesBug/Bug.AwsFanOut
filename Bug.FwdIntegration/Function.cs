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
	private IntegrationService _integrationService = new IntegrationService();

	public async Task FunctionHandler(DynamoDBEvent dynamoEvent, ILambdaContext context)
	{
		context.Logger.LogInformation("Chegou à FwdIntegration");

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

				context.Logger.LogLine($"Nova integração de {integration.Type} (PK: '{integration.PK}', SK '{integration.SK}')");

				ForwardIntegration(integration, context);
			}
		}
	}

	public async Task ForwardIntegration(Integration integration, ILambdaContext context)
	{
		try
		{
			var topicArn = "arn:aws:sns:us-east-1:571600868266:Bug-FwdIntegrationTopic.fifo";

			var message = JsonConvert.SerializeObject(integration);

			var snsService = new SnsService();

			await snsService.PublishFifoMessageAsync(topicArn, integration.Type, message, integration.FifoMessageGroupId);
			
			integration.Status = IntegrationStatus.Processing;
			integration.ResponseMessage = "A integração foi encaminhada";
			context.Logger.LogInformation(integration.ResponseMessage);

			_integrationService.UpdateAttributesAsync(integration, [nameof(Integration.Status), nameof(Integration.ResponseMessage)]);
		}
		catch (Exception)
		{
			integration.Status = IntegrationStatus.Failure;
			integration.ResponseMessage = "Erro ao encaminhar integração";
			context.Logger.LogError(integration.ResponseMessage);

			_integrationService.UpdateAttributesAsync(integration, [nameof(Integration.Status), nameof(Integration.ResponseMessage)]);
		}

		
	}
}
