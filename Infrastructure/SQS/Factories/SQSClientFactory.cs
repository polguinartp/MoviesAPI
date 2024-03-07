using Amazon;
using Amazon.SQS;
using Infrastructure.Options;
using Microsoft.Extensions.Options;
using System;

namespace Infrastructure.SQS.Factories
{
    public class SQSClientFactory : ISQSClientFactory
    {
        private readonly SQSOptions _sqsOptions;

        public SQSClientFactory(IOptions<SQSOptions> sqsOptions)
        {
            ArgumentNullException.ThrowIfNull(sqsOptions);

            _sqsOptions = sqsOptions.Value;
        }

        public string QueueUrl => _sqsOptions.QueueUrl;

        public IAmazonSQS CreateSQSClient()
        {
            var config = new AmazonSQSConfig()
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(_sqsOptions.Region),
                ServiceURL = _sqsOptions.QueueUrl
            };

            return new AmazonSQSClient(config);
        }
    }
}
