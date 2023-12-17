using System;

namespace Infrastructure.SQS.Services
{
    public class QueueMessage
    {
        public Guid Id { get; set; }
        public string Message { get; set; }
        public DateTime DateTime { get; set; }
    }
}
