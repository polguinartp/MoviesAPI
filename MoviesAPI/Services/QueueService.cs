using Infrastructure.SQS.Services;
using System;
using System.Threading.Tasks;

namespace MoviesAPI.Services;

public class QueueService : IQueueService
{
    private readonly ISQSService _sQSService;

    public QueueService(ISQSService sqsService)
    {
        ArgumentNullException.ThrowIfNull(sqsService);

        _sQSService = sqsService;
    }

    public async Task SendAsync(QueueMessage queueMessage)
    {
        await _sQSService.EnqueueMessageAsync(queueMessage);
    }
}
