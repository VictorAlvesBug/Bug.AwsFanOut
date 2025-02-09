﻿using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Runtime.CredentialManagement;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bug.Infrastructure.Services
{
	public class SnsService
	{
		private readonly AmazonSimpleNotificationServiceClient _snsClient;

		public SnsService()
		{
			var chain = new CredentialProfileStoreChain();

			if (chain.TryGetAWSCredentials("default", out var credentials))
			{
				_snsClient = new AmazonSimpleNotificationServiceClient(credentials, RegionEndpoint.USEast1);
			}
		}

		public Task PublishMessageAsync(string topicArn, string message)
		{
			PublishFifoMessageAsync(topicArn, message, null, null);
		}

		public async Task PublishFifoMessageAsync(string topicArn, string message, string messageGroupId)
		{
			var request = new PublishRequest
			{
				TopicArn = topicArn,
				Message = message,
				MessageGroupId = messageGroupId,
				MessageDeduplicationId = Guid.NewGuid().ToString()
			};

			var response = await _snsClient.PublishAsync(request);
			Console.WriteLine($"Mensagem enviada! MessageId: {response.MessageId}");
		}
	}
}
