using Amazon.SQS.Model;
using Domain.Queues;
using Infrastructure.SQS.Factories;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.SQS.Services
{
    public class SQSService : ISQSService
    {
        private readonly ISQSClientFactory _sqsFactory;

        public SQSService(ISQSClientFactory sqsFactory)
        {
            ArgumentNullException.ThrowIfNull(sqsFactory);

            _sqsFactory = sqsFactory;
        }

        public async Task EnqueueMessageAsync(QueueMessage queueMessage)
        {
            var request = new SendMessageRequest()
            {
                MessageGroupId = Guid.NewGuid().ToString(),
                MessageDeduplicationId = Guid.NewGuid().ToString(),
                MessageBody = JsonSerializer.Serialize(queueMessage),
                QueueUrl = _sqsFactory.QueueUrl
            };

            var sqsClient = _sqsFactory.CreateSQSClient();
            await sqsClient.SendMessageAsync(request);
        }
    }
}
