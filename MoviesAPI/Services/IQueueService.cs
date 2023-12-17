using Infrastructure.SQS.Services;
using System.Threading.Tasks;

namespace MoviesAPI.Services;

public interface IQueueService
{
    Task SendAsync(QueueMessage queueMessage);
}
