using MoviesAPI.DTOs.IMDB;
using MoviesAPI.Options;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace MoviesAPI.WebClients;

public class IMDBWebApiClient(IMDBWebApiClientOptions options, HttpClient httpClient) : IIMDBWebApiClient
{
	public async Task<IMDBMovieInfo> GetMovieInfoAsync(string imdbId)
	{
		HttpResponseMessage httpResponseMessage;
		var path = $"{options.Url}/{options.ApiKey}/{imdbId}";

		try
		{
			httpResponseMessage = await httpClient.GetAsync(path);
			httpResponseMessage.EnsureSuccessStatusCode();

			await using var stream = await httpResponseMessage.Content.ReadFromJsonAsync<IMDBMovieInfo>();
			return stream;
		}
		catch
		{
			// return dummy object because imdb site is down
			return new IMDBMovieInfo()
			{
				ImdbId = "-1",
				ReleaseDate = DateTime.UtcNow,
				Stars = "Jack Daniels",
				Title = "booom! the movie"
			};
		}
	}

	public async Task<HttpStatusCode> GetStatusAsync()
	{
		try
		{
			var path = $"{options.Url}/{options.ApiKey}";
			HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(path);
			return httpResponseMessage.StatusCode;
		}
		catch (HttpRequestException)
		{
			return HttpStatusCode.NotFound;
		}
	}
}
