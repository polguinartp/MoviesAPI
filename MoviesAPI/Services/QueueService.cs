using Infrastructure.SQS.Services;
using System;
using System.Threading.Tasks;

namespace MoviesAPI.Services;

public class QueueService : IQueueService
{
    private readonly ISQSService _sqsService;

    public QueueService(ISQSService sqsService)
    {
        ArgumentNullException.ThrowIfNull(sqsService);

        _sqsService = sqsService;
    }

    public async Task SendAsync(QueueMessage queueMessage)
    {
        await _sqsService.EnqueueMessageAsync(queueMessage);
    }
}
