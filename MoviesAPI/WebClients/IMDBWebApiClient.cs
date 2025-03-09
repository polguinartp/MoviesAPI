using Microsoft.Extensions.Options;
using MoviesAPI.DTOs.IMDB;
using MoviesAPI.Options;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MoviesAPI.WebClients;

public class IMDBWebApiClient : IIMDBWebApiClient
{
    private readonly WebApiClientOptions _options;
    private readonly HttpClient _httpClient;

    public IMDBWebApiClient(IOptions<WebApiClientOptions> options, HttpClient httpClient)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(httpClient);

        _options = options.Value;
        _httpClient = httpClient;
    }

    public async Task<IMDBMovieInfo> GetMovieInfoAsync(string imdbId)
    {
        HttpResponseMessage httpResponseMessage;
        var path = $"{_options.IMDBUrl}/{_options.IMDBApiKey}/{imdbId}";

        try
        {
            httpResponseMessage = await _httpClient.GetAsync(path);
            httpResponseMessage.EnsureSuccessStatusCode();

            await using var stream = await httpResponseMessage.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<IMDBMovieInfo>(stream);
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

    public async Task<HttpStatusCode> GetStatus()
    {
        try
        {
            var path = $"{_options.IMDBUrl}/{_options.IMDBApiKey}";
            HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync(path);
            return httpResponseMessage.StatusCode;
        }
        catch(HttpRequestException)
        {
            return HttpStatusCode.NotFound;
        }
    }
}
