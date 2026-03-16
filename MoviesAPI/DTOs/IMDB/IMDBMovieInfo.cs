using System;
using System.Threading.Tasks;

namespace MoviesAPI.DTOs.IMDB;

public class IMDBMovieInfo : IAsyncDisposable
{
	public string Title { get; set; }

	public DateTime ReleaseDate { get; set; }

	public string ImdbId { get; set; }

	public string Stars { get; set; }

	public ValueTask DisposeAsync()
	{
		return ValueTask.CompletedTask;
	}
}
