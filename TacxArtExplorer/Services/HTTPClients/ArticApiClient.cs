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
    public interface IArticApiClient
    {
        Task<IResultDTO> GetAsync(string endpoint, CancellationToken ct = default);
    }

    public abstract class ArticApiClient : IArticApiClient
    {
        protected HttpClient _httpClient { get; }
        public ArticApiClient(HttpClient httpClient, Uri baseUri)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = baseUri;
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("TacxArtExplorer/1.0");
            _httpClient.DefaultRequestHeaders.Accept.ParseAdd("application/json");
        }

        /// <summary>
        /// Retrieves data from the specified endpoint. Returns a result DTO that implements IResultDTO.
        /// </summary>
        /// <param name="endpoint">Relative address, possibly including a query, at which the record is retreived</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>A result DTO</returns>
        public abstract Task<IResultDTO> GetAsync(string endpoint, CancellationToken ct = default);
    }
}
