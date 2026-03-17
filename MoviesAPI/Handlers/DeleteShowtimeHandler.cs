using Infrastructure.Database;
using Mediator;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Requests;
using System.Threading;
using System.Threading.Tasks;

namespace MoviesAPI.Handlers
{
	public class DeleteShowtimeHandler(CinemaDbContext dbContext) : IRequestHandler<DeleteShowtimeRequest>
	{
		public async ValueTask<Unit> Handle(DeleteShowtimeRequest request, CancellationToken cancellationToken)
		{
			var showtime = await dbContext.Showtimes.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
			if (showtime is null)
			{
				return Unit.Value;
			}

			dbContext.Showtimes.Remove(showtime);
			await dbContext.SaveChangesAsync(cancellationToken);

			// IMPORTANT: AWS Free Tier is expired; every 1M messages is charged.
			//var queueMessage = new QueueMessage()
			//{
			//	Message = $"Showtime {request.Id} has been deleted.",
			//	DateTime = DateTime.Now
			//};
			//await queueService.SendAsync(queueMessage);

			return Unit.Value;
		}
	}
}
