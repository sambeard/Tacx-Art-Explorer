using Xunit;
using Moq;
using TacxArtExplorer.Models;
using TacxArtExplorer.Models.DTO;
using TacxArtExplorer.Services;
using TacxArtExplorer.Services.HTTPClients;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TacxArtExplorer.Tests.Services;

/// <summary>
/// Tests for ArtAPIService:
/// - Fetches art pieces from API client
/// - Handles empty and error responses
/// </summary>
public class ArtAPIServiceTests
{
    [Fact]
    public async Task GetArtPiecesByArtistAsync_ReturnsArtPieces_WhenApiReturnsData()
    {
        var artist = new Artist(1, "Test Artist");
        var apiResponse = new ArtworksApiResponseDto { Data = new List<ArtworkDto> { new ArtworkDto { Title = "Test" , Thumbnail= new ThumbnailDto { } } } };
        var apClientMock = new Mock<IArticArtworkClient>();
        var p = new PaginationDTO { Limit = 10, Page = 0 };
        apClientMock.Setup(c => c.GetArtworksFromArtistId(artist.Id, p)).ReturnsAsync(apiResponse);

        var imClientMock = new Mock<IArticImageClient>();
        var loggerMock = new Mock<ILogger<ArtAPIService>>();
        var service = new ArtAPIService(apClientMock.Object, imClientMock.Object, loggerMock.Object);

        var result = await service.GetArtPiecesByArtistAsync(artist);

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Test", result.First().Title);
    }

    [Fact]
    public async Task GetArtPiecesByArtistAsync_ReturnsNull_WhenApiThrows()
    {
        var artist = new Artist(1, "Test Artist");
        var pagination = new PaginationDTO { Page = 0, Limit = 10 };
        var apClientMock = new Mock<IArticArtworkClient>();
        apClientMock.Setup(c => c.GetArtworksFromArtistId(artist.Id, pagination)).Throws<Exception>();

        var imClientMock = new Mock<IArticImageClient>();
        var loggerMock = new Mock<ILogger<ArtAPIService>>();
        var service = new ArtAPIService(apClientMock.Object, imClientMock.Object, loggerMock.Object);

        var result = await service.GetArtPiecesByArtistAsync(artist);

        Assert.Null(result);
    }
}