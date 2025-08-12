using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TacxArtExplorer.Models.DTO
{
    public interface IResultDTO {}

    /// <summary>
    /// Represents a artwork thumbnail image data as received from the artic API, with low-quality image placeholder (LQIP), dimensions, and alternative text.
    /// </summary>
    public sealed record ThumbnailDto
    {
        [JsonPropertyName("lqip")] public string? Lqip { get; init; }
        [JsonPropertyName("width")] public int Width { get; init; }
        [JsonPropertyName("height")] public int Height { get; init; }
        [JsonPropertyName("alt_text")] public string? AltText { get; init; }
    }

    /// <summary>
    /// Represents artwork data as received from the artic API, including metadata such as title, description, artist information, and a thumbnail.
    /// </summary>
    /// <remarks>This data transfer object is  used to encapsulate artwork-related information for
    /// serialization and communication between the Artic API and TacxArtExplorer. </remarks>
    public sealed record ArtworkDto
    {
        [JsonPropertyName("title")] public string Title { get; init; } = "";
        [JsonPropertyName("thumbnail")] public ThumbnailDto? Thumbnail { get; init; }
        [JsonPropertyName("description")] public string? Description { get; init; }
        [JsonPropertyName("artist_id")] public int? ArtistId { get; init; }
        [JsonPropertyName("artist_title")] public string? ArtistTitle { get; init; }
    }

    /// <summary>
    /// Represents the result of a paginated query, including metadata about the pagination state.
    /// </summary>
    /// <remarks>This type provides information about the total number of items, the pagination limits,  and
    /// the current page state. It is part of the result object from the artic artworks api response</remarks>
    public sealed record PaginationResultDto
    {
        [JsonPropertyName("total")] public int Total { get; init; }
        [JsonPropertyName("limit")] public int Limit { get; init; }
        [JsonPropertyName("offset")] public int Offset { get; init; }
        [JsonPropertyName("total_pages")] public int TotalPages { get; init; }
        [JsonPropertyName("current_page")] public int CurrentPage { get; init; }
    }

    /// <summary>
    /// Represents pagination parameters for querying artworks, including the current page number and the number of items per page.
    /// </summary>
    /// <remarks>
    /// This differs from the PaginationResultDto in that it is passed to the Artic API instead of received as part of the response.
    /// </remarks>
    public sealed record PaginationDTO
    {
        public int Page { get; init; }
        public int Limit { get; init; }
        public string ToQueryString()
        {
            return $"page={Page}&limit={Limit}";
        }
    }

    /// <summary>
    /// Encapsulating DTO for the response from the Artworks API, containing pagination information and a list of artworks.
    /// </summary>
    public sealed record class ArtworksApiResponseDto : IResultDTO
    {
        [JsonPropertyName("pagination")] public PaginationResultDto Pagination { get; init; } = default!;
        [JsonPropertyName("data")] public List<ArtworkDto> Data { get; init; } = new();
    }
}
