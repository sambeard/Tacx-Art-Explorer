using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
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
    public interface IArticArtworkClient {
        public Task<ArtworksApiResponseDto> GetArtworksFromArtistId(int artist_id, PaginationDTO? pagination);
    }
    public class ArticArtworkClient : IArticArtworkClient
    {
        internal record ArtworksSearchRequest : IRequest {
            private const string _artworksSearchPath = "artworks/search/";
            private int _aid;
            private PaginationDTO? _pagination;
            private static string[] _artwokQueryFields => typeof(ArtworkDto)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? "")
                .Where(n => !string.IsNullOrWhiteSpace(n)!)
                .ToArray();
            private string artWorksQuery(int artistId, PaginationDTO? pagination = null) =>
                $"?query[term][artist_id]={Uri.EscapeDataString(artistId.ToString())}" +
                $"&fields={string.Join(',', _artwokQueryFields)}"
                + (pagination is PaginationDTO ? "&" + pagination.ToQueryString() : "");

            public ArtworksSearchRequest(int artistId, PaginationDTO? pagination = null) {
                _aid = artistId;
                _pagination = pagination;
            }
            public string BuildUri() => $"{_artworksSearchPath}{artWorksQuery(_aid, _pagination)}";
        }

        private readonly IArticApiClient _articClient;
        //private const string _baseUrl = "https://api.artic.edu/api/v1/";
        
        // retrieving the query fields from the ArtworkDto properties using reflection

        public ArticArtworkClient([FromKeyedServices("ArtworkClient")] IArticApiClient articApiClient)  {
            _articClient = articApiClient;
        }

        public async Task<ArtworksApiResponseDto> GetArtworksFromArtistId(int artist_id, PaginationDTO? pagination) =>
            await _articClient.GetFromJSONEndpointAsync<ArtworksApiResponseDto>(new ArtworksSearchRequest(artist_id,pagination).BuildUri());

    }
}
