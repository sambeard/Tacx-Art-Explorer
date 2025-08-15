using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using TacxArtExplorer.Models;
using TacxArtExplorer.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TacxArtExplorer.Tests.Services;

/// <summary>
/// Tests for ArtService functionality:
/// - Returns cached art pieces if available
/// - Fetches from API if cache is empty
/// - Stores fetched art pieces in cache
/// </summary>
public class ArtServiceTests
{
    [Fact]
    public async Task GetArtPiecesByArtistAsync_ReturnsCached_WhenAvailable()
    {
        // Arrange
        var artist = new Artist(1, "Test Artist");
        var cached = new List<ArtPiece> { new ArtPiece { Id = 1, Title = "Cached" } };
        var cacheMock = new Mock<IArtCacheService>();
        cacheMock.Setup(c => c.GetArtPiecesByArtistAsync(artist)).ReturnsAsync(cached);
        var apiMock = new Mock<IArtAPIService>();
        var loggerMock = new Mock<ILogger<ArtService>>();
        var artService = new ArtService(apiMock.Object, cacheMock.Object, loggerMock.Object);

        // Act
        var result = await artService.GetArtPiecesByArtistAsync(artist);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Cached", result.First().Title);
        // checks if API service was called
        apiMock.Verify(a => a.GetArtPiecesByArtistAsync(It.IsAny<Artist>()), Times.Never);
    }

    [Fact]
    public async Task GetArtPiecesByArtistAsync_FetchesFromApi_IfCacheEmpty()
    {
        // Arrange
        var artist = new Artist(2, "API Artist");
        var apiResult = new List<ArtPiece> { new ArtPiece { Id = 2, Title = "API" } };
        var cacheMock = new Mock<IArtCacheService>();
        cacheMock.Setup(c => c.GetArtPiecesByArtistAsync(artist)).ReturnsAsync((IEnumerable<ArtPiece>?)null);
        var apiMock = new Mock<IArtAPIService>();
        apiMock.Setup(a => a.GetArtPiecesByArtistAsync(artist)).ReturnsAsync(apiResult);
        var loggerMock = new Mock<ILogger<ArtService>>();
        var service = new ArtService(apiMock.Object, cacheMock.Object, loggerMock.Object);

        // Act
        var result = await service.GetArtPiecesByArtistAsync(artist);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("API", result.First().Title);
        cacheMock.Verify(c => c.InsertOrUpdateArtPiecesAsync(apiResult), Times.Once);
    }
}