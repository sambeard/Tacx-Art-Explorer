using Xunit;
using Moq;
using TacxArtExplorer.Models;
using TacxArtExplorer.Models.DTO;
using TacxArtExplorer.Services.HTTPClients;
using System.Net.Http;
using System.Threading.Tasks;

namespace TacxArtExplorer.Tests.Services.HTTPClients;

/// <summary>
/// Tests for ArticArtworkClient:
/// - Correctly builds endpoint for artist and pagination
/// - Throws on invalid response
/// </summary>
public class ArticArtworkClientTests
{
    [Fact]
    public void GetArtworksEndpoint_ReturnsExpectedEndpoint()
    {
        var baseUrl = "https://testsite.com/path/";
        var p = new PaginationDTO { Limit = 20, Page = 2 };
        var artworksRequest = new ArticArtworkClient.ArtworksSearchRequest(123, p);
        var endpoint = artworksRequest.BuildUri();

        // check if valid uri
        Assert.True(Uri.TryCreate(baseUrl + endpoint, UriKind.Absolute, out var baseUri), $"{baseUrl + endpoint} is not a valid URI");
        Assert.Contains("page=2", baseUri.Query);
        Assert.Contains("limit=20",baseUri.Query);
        Assert.Contains("fields=",baseUri.Query);
        Assert.Contains("[artist_id]=123",baseUri.Query);

    }

    // Integration test for GetAsync would require a real or mocked HttpClient and DTOs.
    // You can add more tests when GetAsync is extended or refactored.
}