using System;

namespace Domain.Queues;

public class QueueMessage
{
	//public Guid Id { get; set; }
	public string Message { get; set; }
	public DateTime DateTime { get; set; }
}
