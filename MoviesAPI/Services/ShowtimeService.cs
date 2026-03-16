using AutoMapper;
using Domain.Entities;
using Domain.Queues;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.DTOs.API.Requests;
using MoviesAPI.DTOs.API.Responses;
using MoviesAPI.WebClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.Services;

public class ShowtimeService(IIMDBWebApiClient webClient, IMapper mapper, IQueueService queueService, CinemaDbContext dbContext) : IShowtimeService
{
	public async Task<List<ShowtimeResponse>> GetAsync(DateTime? date, string? movieTitle)
	{
		if (date is null && string.IsNullOrEmpty(movieTitle))
		{
			mapper.Map<List<ShowtimeResponse>>(await dbContext.Showtimes.AsNoTracking().Include(x => x.Movie).ToListAsync());
		}

		var showtimesQuery = dbContext.Showtimes.AsNoTracking().Include(x => x.Movie);
		var query = dbContext.Showtimes.AsNoTracking().Include(x => x.Movie).AsQueryable();
		if (date.HasValue)
		{
			query = query.Where(x => x.StartDate.Date >= date && x.EndDate.Date <= date);
		}
		if (!string.IsNullOrEmpty(movieTitle))
		{
			query = query.Where(x => x.Movie.Title.Contains(movieTitle, StringComparison.OrdinalIgnoreCase));
		}

		var result = await query.ToListAsync();
		return mapper.Map<List<ShowtimeResponse>>(await query.ToListAsync());
	}

	public async Task<ShowtimeResponse> GetByIdAsync(int id)
	{
		var showtime = await dbContext.Showtimes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
		return showtime is null ? null! : mapper.Map<ShowtimeResponse>(showtime);
	}

	public async Task<ShowtimeResponse> CreateAsync(ShowtimeRequest showtimeRequest)
	{
		var showtimeEntity = mapper.Map<Showtime>(showtimeRequest);

		var movieInfo = await webClient.GetMovieInfoAsync(showtimeRequest.MovieId);
		showtimeEntity.Movie = mapper.Map<Movie>(movieInfo);

		var existingMovie = await dbContext.Movies.AsNoTracking().FirstOrDefaultAsync(x => x.Equals(showtimeEntity.Movie));
		if (existingMovie != null)
		{
			showtimeEntity.Movie = existingMovie;
		}

		await dbContext.Showtimes.AddAsync(showtimeEntity);
		await dbContext.SaveChangesAsync();
		
		return mapper.Map<ShowtimeResponse>(showtimeEntity);
	}

	public async Task<ShowtimeResponse> UpdateAsync(ShowtimeRequest showtimeRequest)
	{
		var showtime = await dbContext.Showtimes.FirstOrDefaultAsync(x => x.Id == showtimeRequest.Id);
		if (showtime is null)
		{
			return null!;
		}

		mapper.Map(showtimeRequest, showtime);

		if (showtimeRequest.MovieId is not null && showtimeRequest.MovieId != showtime.Movie.ImdbId)
		{
			var movieInfo = await webClient.GetMovieInfoAsync(showtimeRequest.MovieId);
			showtime.Movie = mapper.Map<Movie>(movieInfo);
		}

		await dbContext.SaveChangesAsync();

		return mapper.Map<ShowtimeResponse>(showtime);
	}

	public async Task DeleteAsync(int id)
	{
		await dbContext.Showtimes.Where(x => x.Id == id).ExecuteDeleteAsync();

		// aws free tier finished -> every 1M messages costs
		var queueMessage = new QueueMessage()
		{
			Message = $"Showtime {id} has been deleted.",
			DateTime = DateTime.Now
		};
		await queueService.SendAsync(queueMessage);
	}
}
