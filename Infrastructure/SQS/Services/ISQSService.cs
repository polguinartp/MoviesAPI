using Domain.Queues;
using System.Threading.Tasks;

namespace Infrastructure.SQS.Services
{
    public interface ISQSService
    {
        Task EnqueueMessageAsync(QueueMessage message);
    }
}
