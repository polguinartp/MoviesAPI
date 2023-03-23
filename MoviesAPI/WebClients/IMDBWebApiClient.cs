using Microsoft.Extensions.Options;
using MoviesAPI.DTOs.IMDB;
using MoviesAPI.Options;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MoviesAPI.WebClients
{
    public class IMDBWebApiClient : IIMDBWebApiClient
    {
        private readonly WebApiClientOptions _options;
        private readonly HttpClient _httpClient;

        public IMDBWebApiClient(IOptions<WebApiClientOptions> options, HttpClient httpClient)
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<IMDBMovieInfo> GetMovieInfoAsync(string imdbId)
        {
            HttpResponseMessage httpResponseMessage;
            var path = $"{_options.IMDBUrl}/{_options.IMDBApiKey}/{imdbId}";
            httpResponseMessage = await _httpClient.GetAsync(path);

            httpResponseMessage.EnsureSuccessStatusCode();

            await using var stream = await httpResponseMessage.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<IMDBMovieInfo>(stream);
        }

        public async Task<HttpStatusCode> GetStatus()
        {
            HttpResponseMessage httpResponseMessage;
            using (var httpClient = new HttpClient())
            {
                var path = $"{_options.IMDBUrl}/{_options.IMDBApiKey}";
                httpResponseMessage = await httpClient.GetAsync(path);
            }

            return httpResponseMessage.StatusCode;
        }
    }
}
