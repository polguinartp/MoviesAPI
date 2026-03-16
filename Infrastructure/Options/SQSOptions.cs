namespace Infrastructure.Options;

public class SQSOptions
{
	public string Region { get; set; }
	public string QueueUrl { get; set; }
	public string AwsAccessKey { get; set; }
	public string AwsSecretKey { get; set; }
}
