using Domain.Queues;
using System.Threading.Tasks;

namespace MoviesAPI.Services;

public interface IQueueService
{
    Task SendAsync(QueueMessage queueMessage);
}
