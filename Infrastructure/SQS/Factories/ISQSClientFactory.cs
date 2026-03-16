using Amazon.SQS;

namespace Infrastructure.SQS.Factories;

public interface ISQSClientFactory
{
	IAmazonSQS CreateSQSClient();
}
