using MoviesAPI.DTOs.IMDB;
using System.Net;
using System.Threading.Tasks;

namespace MoviesAPI.WebClients;

public interface IIMDBWebApiClient
{
	Task<IMDBMovieInfo> GetMovieInfoAsync(string imdbId);

	Task<HttpStatusCode> GetStatusAsync();
}
