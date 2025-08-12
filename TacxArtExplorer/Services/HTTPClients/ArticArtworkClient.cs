using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TacxArtExplorer.Models;
using TacxArtExplorer.Models.DTO;
using static System.Net.WebRequestMethods;

namespace TacxArtExplorer.Services.HTTPClients
{
    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
    [JsonSerializable(typeof(ArtworksApiResponseDto))]
    internal partial class ArtworksApiJsonContext : JsonSerializerContext { }
    class ArticArtworkClient : ArticApiClient
    {
        private const string _baseUrl = "https://api.artic.edu/api/v1/";
        private const string _artworksSearchPath = "artworks/search/";
        private readonly string[] _queryFields = ["id", "title", "artist_id", "artist_title", "image_id", "thumbnail"];

        protected static readonly JsonSerializerOptions JsonOpts = new()
        {
            PropertyNameCaseInsensitive = true // provider uses snake/camel mix
        };
        public ArticArtworkClient(HttpClient httpClient) : base(httpClient, new Uri(_baseUrl)) { }

        // <inheritdoc />
        public override async Task<IResultDTO> GetAsync(string endpoint, CancellationToken ct = default)
        {
            using var resp = await _httpClient.GetAsync(endpoint, HttpCompletionOption.ResponseHeadersRead, ct);
            resp.EnsureSuccessStatusCode();

            using var stream = await resp.Content.ReadAsStreamAsync(); // widely available
            var dto = await JsonSerializer.DeserializeAsync<ArtworksApiResponseDto>(stream, JsonOpts, ct)
                      ?? throw new InvalidOperationException("Invalid or empty response");
            return dto;
        }

        private string artWorksQuery(int artistId, PaginationDTO? pagination = null) =>
            $"?query[term][artist_id]={Uri.EscapeDataString(artistId.ToString())}&fields={string.Join(',', _queryFields)}" 
            + (pagination is PaginationDTO ? "&" + pagination.ToQueryString() : "");

        /// <summary>
        /// Returns a URI for the artworks search endpoint with the specified artist ID and optional pagination.
        /// </summary>
        public string GetArtworksEndpoint(Artist artist, int? page = null, int? limit = null) { 
            var pagination = (page is int p && limit is int l) ? new PaginationDTO { Page = p, Limit = l } : null;
            string query = artWorksQuery(artist.Id, pagination);
            return _artworksSearchPath + query;
        }
    }
}
