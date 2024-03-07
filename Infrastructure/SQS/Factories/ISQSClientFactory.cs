using Amazon.SQS;

namespace Infrastructure.SQS.Factories
{
    public interface ISQSClientFactory
    {
        string QueueUrl { get; }
        IAmazonSQS CreateSQSClient();
    }
}
