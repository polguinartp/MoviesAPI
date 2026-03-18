using MoviesAPI.DTOs.IMDB;
using MoviesAPI.Options;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace MoviesAPI.WebClients;

public class IMDBWebApiClient(IMDBWebApiClientOptions options, HttpClient httpClient) : IIMDBWebApiClient
{
	public async Task<IMDBMovieInfo> GetMovieInfoAsync(string imdbId)
	{
		//HttpResponseMessage httpResponseMessage;
		//var path = $"{options.Url}/{options.ApiKey}/{imdbId}";

		//httpResponseMessage = await httpClient.GetAsync(path);

		//if (httpResponseMessage.IsSuccessStatusCode)
		//{
		//	var result = await httpResponseMessage.Content.ReadFromJsonAsync<IMDBMovieInfo>();
		//	return result!;
		//}

		// Return sample objects because IMDB website is not available anymore.
		return imdbId switch
		{
			"tt0111161" => new IMDBMovieInfo(
				Title: "The Shawshank Redemption",
				ReleaseDate: new DateTime(1994, 9, 22),
				ImdbId: "tt0111161",
				Stars: "Tim Robbins, Morgan Freeman, Bob Gunton"
			),
			"tt0068646" => new IMDBMovieInfo(
				Title: "The Godfather",
				ReleaseDate: new DateTime(1972, 3, 24),
				ImdbId: "tt0068646",
				Stars: "Marlon Brando, Al Pacino, James Caan"
			),
			"tt0468569" => new IMDBMovieInfo(
				Title: "The Dark Knight",
				ReleaseDate: new DateTime(2008, 7, 18),
				ImdbId: "tt0468569",
				Stars: "Christian Bale, Heath Ledger, Aaron Eckhart"
			),
			_ => null!
		};
	}

	public async Task<HttpStatusCode> GetStatusAsync()
	{
		try
		{
			//var path = $"{options.Url}/{options.ApiKey}";
			//var httpResponseMessage = await httpClient.GetAsync(path);
			//return httpResponseMessage.StatusCode;

			// Return 200 OK because IMDB website is not available anymore.
			return HttpStatusCode.OK;
		}
		catch (HttpRequestException)
		{
			return HttpStatusCode.NotFound;
		}
	}
}
