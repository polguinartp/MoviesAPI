using Amazon.SQS;

namespace Infrastructure.SQS.Factories
{
    public interface ISQSFactory
    {
        string QueueUrl { get; }
        IAmazonSQS CreateSQSClient();
    }
}
