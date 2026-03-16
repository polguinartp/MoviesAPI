using MoviesAPI.DTOs.API.Requests;
using MoviesAPI.DTOs.API.Responses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoviesAPI.Services;

public interface IShowtimeService
{
	Task<List<ShowtimeResponse>> GetAsync(DateTime? date, string? movieTitle);

	Task<ShowtimeResponse> GetByIdAsync(int id);

	Task<ShowtimeResponse> CreateAsync(ShowtimeRequest entity);

	Task<ShowtimeResponse> UpdateAsync(int id, ShowtimeRequest entity);

	Task DeleteAsync(int id);
}
