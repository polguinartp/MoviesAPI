using AutoMapper;
using Infrastructure.Database;
using Mediator;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.DTOs.Responses;
using MoviesAPI.Requests;
using System.Threading;
using System.Threading.Tasks;

namespace MoviesAPI.Handlers
{
	public class GetShowtimeByIdHandler(IMapper mapper, CinemaDbContext dbContext) : IRequestHandler<GetShowtimeByIdRequest, ShowtimeResponse>
	{
		public async ValueTask<ShowtimeResponse> Handle(GetShowtimeByIdRequest request, CancellationToken cancellationToken)
		{
			var showtime = await dbContext.Showtimes.AsNoTracking().Include(x => x.Movie).FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
			return showtime is null ? null! : mapper.Map<ShowtimeResponse>(showtime);
		}
	}
}
