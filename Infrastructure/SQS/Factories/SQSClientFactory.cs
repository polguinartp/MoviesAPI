using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using Infrastructure.Options;

namespace Infrastructure.SQS.Factories;

public class SQSClientFactory(SQSOptions options) : ISQSClientFactory
{
	public string QueueUrl => options.QueueUrl;

	public IAmazonSQS CreateSQSClient()
	{
		var credentials = new BasicAWSCredentials(options.AwsAccessKey, options.AwsSecretKey);
		var config = new AmazonSQSConfig()
		{
			RegionEndpoint = RegionEndpoint.GetBySystemName(options.Region),
			ServiceURL = options.QueueUrl
		};

		return new AmazonSQSClient(credentials, config);
	}
}
