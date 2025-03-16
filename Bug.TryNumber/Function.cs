using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Bug.Domain.Entities.DrawNumber;
using Bug.Domain.Entities.Integration;
using Bug.Helpers.Extensions;
using Bug.Infrastructure.Services;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Bug.TryNumber;

public class Function
{
	private IntegrationService _integrationService;

	public Function()
	{
		_integrationService = new IntegrationService();
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
		var payload = message.Body.SafeParse<DrawNumberPayload>();
		payload.NumberOfAttempts++;

		// Thinking
		switch (payload.GuessFeedback)
		{
			case GuessFeedback.NotTriedYet:
				break;

			case GuessFeedback.TooSmall:
				payload.MinLimit = payload.Guess + 1;
				break;

			case GuessFeedback.TooBig:
				payload.MaxLimit = payload.Guess - 1;
				break;

			case GuessFeedback.NailedIt:
				context.Logger.LogInformation("Você já acertou, não precisava ter tentado novamente.");
				return;

		}

		// Guessing
		payload.Guess = new Random().Next(payload.MinLimit, payload.MaxLimit);

		// Getting Feedback
		if (payload.Guess == payload.DrawedNumber)
		{
			payload.GuessFeedback = GuessFeedback.NailedIt;

			context.Logger.LogInformation("Você acertou em cheio!");

			await _integrationService.PutAsync(new IntegrationToPut
			{
				Type = IntegrationType.DiscoveredNumber,
				Body = payload.ToJson()
			});

			return;
		}

		if (payload.Guess < payload.DrawedNumber)
		{
			payload.GuessFeedback = GuessFeedback.TooSmall;
			context.Logger.LogInformation($"{payload.Guess} é pouco, tente um número maior!");
		}
		
		else if (payload.Guess > payload.DrawedNumber)
		{
			payload.GuessFeedback = GuessFeedback.TooBig;
			context.Logger.LogInformation($"{payload.Guess} é muito, tente um número menor!");
		}

		await _integrationService.PutAsync(new IntegrationToPut
		{
			Type = IntegrationType.TryNumber,
			Body = payload.ToJson()
		});
	}
}