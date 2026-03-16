using System;

namespace MoviesAPI.DTOs.API.Responses;

public class MovieResponse
{
	public string Title { get; set; }

	public string ImdbId { get; set; }

	public string Stars { get; set; }

	public DateTime ReleaseDate { get; set; }
}
