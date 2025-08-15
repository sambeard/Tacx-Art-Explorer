using Xunit;
using TacxArtExplorer.Models;
using TacxArtExplorer.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using TacxArtExplorer.Services.SQL;

namespace TacxArtExplorer.Tests.Services;
/// <summary>
/// Tests for CacheService functionality:
/// - Always returns null for cache lookups (default implementation)
/// - Store methods complete without error
/// </summary>
public class CacheServiceTests
{
    private async Task<SqliteConnection> setupConnection(IEnumerable<Artist>? seed = default)
    {
        var dbConnection = new SqliteConnection("Data Source=:memory:");
        dbConnection.Open();
        await SqliteDbInitializer.InitializeAsync(dbConnection, seed);
        return dbConnection;
    }

    [Fact]
    public async Task GetArtPiecesByArtistAsync_AlwaysReturnsNullWhenEmpty()
    {
        var dbConnection = await setupConnection();

        var service = new CacheService(dbConnection);
        var result = await service.GetArtPiecesByArtistAsync(new Artist(1, "Test"));
        Assert.Null(result);
    }

    [Fact]
    public async Task InsertOrUpdateArtPieces_CompletesWithoutError()
    {
        var dbConnection = await setupConnection();
        var service = new CacheService(dbConnection);
        await service.InsertOrUpdateArtPiecesAsync(new List<ArtPiece>());
        // No exception means pass
    }

    [Fact]
    public async Task InsertOrUpdateImage_CompletesWithoutError()
    {
        var dbConnection = await setupConnection();
        var service = new CacheService(dbConnection);
        var image = new ArtPieceImage { Id = "img", Data = new byte[0], Format = ImageFormat.JPG };
        await service.InsertOrUpdateImageAsync(image);
        // No exception means pass
    }

    [Fact]
    public async Task InsertOrUpdateArtist_CompletesWithoutError()
    {

        var dbConnection = await setupConnection();
        var service = new CacheService(dbConnection);

        var artist = new Artist(1, "Test Artist");
        await service.InsertOrUpdateArtistAsync(artist);
        // No exception means pass
    }

    [Fact]
    public async Task InsertOrUpdateArtPieces_ContainsEntriesAfter()
    {
        // Arrange
        var artist = new Artist(1, "Test Artist");
        var dbConnection = await setupConnection([artist]);
        var service = new CacheService(dbConnection);
        var artPieces = new List<ArtPiece>
        {
            // Assuming ArtPiece has a constructor: ArtPiece(int id, string title, int artistId)
            new ArtPiece { Artist=artist, Id=3, ImageID = "img1", Title = "Test Art 1" },
        };

        // Act
        await service.InsertOrUpdateArtPiecesAsync(artPieces);

        var aps= await service.GetArtPiecesByArtistAsync(artist);
        Assert.True(aps?.Any(), "Expected at least one entry in ArtPieces table after insertion.");
    }
}