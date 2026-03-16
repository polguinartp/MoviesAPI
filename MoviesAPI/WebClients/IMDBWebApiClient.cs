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

		httpResponseMessage = await httpClient.GetAsync(path);

		if (httpResponseMessage.IsSuccessStatusCode)
		{
			await using var result = await httpResponseMessage.Content.ReadFromJsonAsync<IMDBMovieInfo>();
			return result!;
		}

		// Return dummy object if IMDB website is not available
		return imdbId switch
		{
			"tt0111161" => new IMDBMovieInfo()
			{
				ImdbId = "tt0111161",
				ReleaseDate = new DateTime(1994, 9, 22),
				Stars = "Tim Robbins, Morgan Freeman, Bob Gunton",
				Title = "The Shawshank Redemption"
			},
			"tt0068646" => new IMDBMovieInfo()
			{
				ImdbId = "tt0068646",
				ReleaseDate = new DateTime(1972, 3, 24),
				Stars = "Marlon Brando, Al Pacino, James Caan",
				Title = "The Godfather"
			},
			"tt0468569" => new IMDBMovieInfo()
			{
				ImdbId = "tt0468569",
				ReleaseDate = new DateTime(2008, 7, 18),
				Stars = "Christian Bale, Heath Ledger, Aaron Eckhart",
				Title = "The Dark Knight"
			}
		};
	}

	public async Task<HttpStatusCode> GetStatusAsync()
	{
		try
		{
			var path = $"{options.Url}/{options.ApiKey}";
			var httpResponseMessage = await httpClient.GetAsync(path);
			return httpResponseMessage.StatusCode;
		}
		catch (HttpRequestException)
		{
			return HttpStatusCode.NotFound;
		}
	}
}
