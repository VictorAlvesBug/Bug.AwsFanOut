using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Bug.Domain.Entities.DrawNumber;
using Bug.Helpers.Extensions;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Bug.DiscoveredNumber;

public class Function
{
	/// <summary>
	/// This method is called for every Lambda invocation. This method takes in an SQS event object and can be used 
	/// to respond to SQS messages.
	/// </summary>
	/// <param name="evnt">The event for the Lambda function handler to process.</param>
	/// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
	/// <returns></returns>
	public async Task FunctionHandler(SQSEvent evnt, ILambdaContext context)
	{
		context.Logger.LogInformation("Chegou à DiscoveredNumber");

		foreach (var message in evnt.Records)
		{
			await ProcessMessageAsync(message, context);
		}
	}

	private async Task ProcessMessageAsync(SQSEvent.SQSMessage message, ILambdaContext context)
	{
		var payload = message.Body.SafeParse<DrawNumberPayload>();
		context.Logger.LogInformation($"Você acertou na {payload.NumberOfAttempts}º tentativa! O número sorteado foi {payload.Guess}.");
	}
}