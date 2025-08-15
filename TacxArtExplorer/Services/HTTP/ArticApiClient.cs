using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TacxArtExplorer.Models.DTO;

namespace TacxArtExplorer.Services.HTTPClients
{
    public interface IRequest
    {
        string BuildUri();
    }
    public interface IArticApiClient
    {
        public Task<ResultDTO> GetFromJSONEndpointAsync<ResultDTO>(string endpoint, CancellationToken ct = default);
    }


    public class ArticApiClient : HttpClient, IArticApiClient
    {
        private static readonly JsonSerializerOptions JsonOpts = new()
        {
            PropertyNameCaseInsensitive = true // provider uses snake/camel mix
        };
        public ArticApiClient(string baseUrl) : base()
        { 
            BaseAddress = new Uri(baseUrl);
            DefaultRequestHeaders.UserAgent.ParseAdd("TacxArtExplorer/1.0");
            DefaultRequestHeaders.Accept.ParseAdd("application/json");
        }

        /// <summary>
        /// Retrieves data from the specified endpoint. Returns a result DTO that implements IResultDTO.
        /// </summary>
        /// <param name="endpoint">Relative address, possibly including a query, at which the record is retreived</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>A result DTO</returns>
        public async Task<ResultDTO> GetFromJSONEndpointAsync<ResultDTO>(string endpoint, CancellationToken ct = default) {
            using var resp = await GetAsync(endpoint, HttpCompletionOption.ResponseHeadersRead, ct);
            resp.EnsureSuccessStatusCode();

            using var stream = await resp.Content.ReadAsStreamAsync(); // widely available
            var dto = await JsonSerializer.DeserializeAsync<ResultDTO>(stream, JsonOpts, ct)
                      ?? throw new InvalidOperationException("Invalid or empty response");
            return dto;
        }
    }
}
