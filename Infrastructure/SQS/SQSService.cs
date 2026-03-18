using Amazon.SQS.Model;
using Domain.Queues;
using Infrastructure.SQS.Factories;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.SQS.Services;

public class SQSService(ISQSClientFactory sqsFactory, SQSOptions options) : ISQSService
{
	public async Task EnqueueMessageAsync(QueueMessage queueMessage)
	{
		var request = new SendMessageRequest()
		{
			MessageGroupId = Guid.NewGuid().ToString(),
			MessageDeduplicationId = Guid.NewGuid().ToString(),
			MessageBody = JsonSerializer.Serialize(queueMessage),
			QueueUrl = options.QueueUrl
		};

		var sqsClient = sqsFactory.CreateSQSClient();
		await sqsClient.SendMessageAsync(request);
	}
}
