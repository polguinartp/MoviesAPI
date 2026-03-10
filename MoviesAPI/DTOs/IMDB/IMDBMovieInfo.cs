using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MoviesAPI.DTOs.IMDB;

public class IMDBMovieInfo : IAsyncDisposable
{
	[JsonPropertyName("title")]
	public string Title { get; set; }

	[JsonPropertyName("releaseDate")]
	public DateTime ReleaseDate { get; set; }

	[JsonPropertyName("id")]
	public string ImdbId { get; set; }

	[JsonPropertyName("stars")]
	public string Stars { get; set; }

	public ValueTask DisposeAsync()
	{
		return ValueTask.CompletedTask;
	}
}
