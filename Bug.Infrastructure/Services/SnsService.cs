using Amazon;
using Amazon.Runtime.CredentialManagement;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Bug.Domain.Entities.Integration;

namespace Bug.Infrastructure.Services
{
	public class SnsService
	{
		private readonly AmazonSimpleNotificationServiceClient _snsClient;

		public SnsService()
		{
			var chain = new CredentialProfileStoreChain();

			if (!chain.TryGetAWSCredentials("default", out var credentials))
			{
				throw new Exception("Credenciais não encontradas");
			}

			_snsClient = new AmazonSimpleNotificationServiceClient(credentials, RegionEndpoint.USEast1);
		}

		public async Task PublishMessageAsync(string topicArn, IntegrationType integrationType, string message)
		{
			await PublishFifoMessageAsync(topicArn, integrationType, message, null);
		}

		public async Task PublishFifoMessageAsync(string topicArn, IntegrationType integrationType, string message, string? messageGroupId)
		{
			var request = new PublishRequest
			{
				TopicArn = topicArn,
				Message = message,
				MessageGroupId = messageGroupId,
				MessageDeduplicationId = Guid.NewGuid().ToString(),
				MessageAttributes = new Dictionary<string, MessageAttributeValue>
				{
					{
						nameof(IntegrationType),
						new MessageAttributeValue
						{
							DataType = nameof(String),
							StringValue = integrationType.ToString()
						}
					}
				}
			};

			var response = await _snsClient.PublishAsync(request);
			Console.WriteLine($"Mensagem enviada! MessageId: {response.MessageId}");
		}
	}
}
