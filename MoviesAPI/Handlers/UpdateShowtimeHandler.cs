using AutoMapper;
using Domain.Entities;
using Infrastructure.Database;
using Mediator;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.DTOs.Responses;
using MoviesAPI.Requests;
using MoviesAPI.WebClients;
using System.Threading;
using System.Threading.Tasks;

namespace MoviesAPI.Handlers;

public class UpdateShowtimeHandler(IMapper mapper, CinemaDbContext dbContext, IIMDBWebApiClient webClient) : IRequestHandler<UpdateShowtimeRequest, ShowtimeResponse>
{
	public async ValueTask<ShowtimeResponse> Handle(UpdateShowtimeRequest request, CancellationToken cancellationToken)
	{
		var showtime = await dbContext.Showtimes.Include(x => x.Movie).FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
		if (showtime is null)
		{
			return null!;
		}

		mapper.Map(request.ShowtimeRequest, showtime);

		if (request.ShowtimeRequest.MovieId is not null && request.ShowtimeRequest.MovieId != showtime.Movie.ImdbId)
		{
			var movieInfo = await webClient.GetMovieInfoAsync(request.ShowtimeRequest.MovieId);
			showtime.Movie = mapper.Map<Movie>(movieInfo);
		}

		await dbContext.SaveChangesAsync(cancellationToken);

		return mapper.Map<ShowtimeResponse>(showtime);
	}
}
