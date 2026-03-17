using AutoMapper;
using Domain.Entities;
using Infrastructure.Database;
using Mediator;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.DTOs.Responses;
using MoviesAPI.Requests;
using MoviesAPI.WebClients;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MoviesAPI.Handlers
{
	public class CreateShowtimeHandler(IMapper mapper, CinemaDbContext dbContext, IIMDBWebApiClient webClient) : IRequestHandler<CreateShowtimeRequest, ShowtimeResponse>
	{
		public async ValueTask<ShowtimeResponse> Handle(CreateShowtimeRequest request, CancellationToken cancellationToken)
		{
			var showtimeEntity = mapper.Map<Showtime>(request.ShowtimeRequest);

			var movieInfo = await webClient.GetMovieInfoAsync(request.ShowtimeRequest.MovieId);
			showtimeEntity.Movie = mapper.Map<Movie>(movieInfo);

			var existingMovie = await dbContext.Movies.AsNoTracking().FirstOrDefaultAsync(x => x.Title.Equals(showtimeEntity.Movie.Title, StringComparison.OrdinalIgnoreCase), cancellationToken);
			if (existingMovie != null)
			{
				showtimeEntity.Movie = existingMovie;
			}

			await dbContext.Showtimes.AddAsync(showtimeEntity, cancellationToken);
			await dbContext.SaveChangesAsync(cancellationToken);

			return mapper.Map<ShowtimeResponse>(showtimeEntity);
		}
	}
}
