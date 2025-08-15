using Xunit;
using TacxArtExplorer.Models;
using TacxArtExplorer.Models.DTO;
using System.Collections.Generic;

namespace TAETesting.Models.DTO;

/// <summary>
/// Tests for DTOMapper:
/// - Maps ThumbnailDto to Thumbnail
/// - Maps ArtworkDto to ArtPiece
/// - Maps ArtworksApiResponseDto to IEnumerable<ArtPiece>
/// - Handles null and empty DTOs
/// </summary>
public class DTOMapperTests
{
    [Fact]
    public void ToThumbnail_MapsPropertiesCorrectly()
    {
        var dto = new ThumbnailDto
        {
            AltText = "alt",
            Width = 100,
            Height = 200
        };

        var thumbnail = DTOMapper.ToThumbnail(dto);

        Assert.Equal("alt", thumbnail.AltText);
        Assert.Equal((100, 200), thumbnail.Dimensions);
    }

    [Fact]
    public void ToThumbnail_ThrowsOnNull()
    {
        Assert.Throws<System.ArgumentNullException>(() => DTOMapper.ToThumbnail(null));
    }

    [Fact]
    public void ToArtPiece_MapsPropertiesCorrectly()
    {
        var dto = new ArtworkDto
        {
            Title = "Art Title",
            Thumbnail = new ThumbnailDto { Lqip = "img", AltText = "alt", Width = 10, Height = 20 },
            ArtistId = 42,
            ArtistTitle = "Artist Name",
            DisplayDate = "2024",
            ArtworkTypeTitle = "Painting",
            PlaceOfOrigin = "Origin",
            ShortDescription = "Short",
            Description = "Long",
            ImageId = "imgid"
        };

        var artPiece = DTOMapper.ToArtPiece(dto);

        Assert.Equal("Art Title", artPiece.Title);
        Assert.Equal(42, artPiece.Artist.Id);
        Assert.Equal("Artist Name", artPiece.Artist.Name);
        Assert.Equal("2024", artPiece.DisplayDate);
        Assert.Equal("Painting", artPiece.ArtworkType);
        Assert.Equal("Origin", artPiece.PlaceOfOrigin);
        Assert.Equal("Short", artPiece.ShortDescription);
        Assert.Equal("Long", artPiece.LongDescription);
        Assert.Equal("imgid", artPiece.ImageID);
        Assert.NotNull(artPiece.Thumbnail);
    }

    [Fact]
    public void ToArtPiece_ThrowsOnNull()
    {
        Assert.Throws<System.ArgumentNullException>(() => DTOMapper.ToArtPiece(null));
    }

    [Fact]
    public void ToArtPieces_MapsListCorrectly()
    {
        var dtoList = new List<ArtworkDto>
        {
            new ArtworkDto { Title = "A", Thumbnail = new ThumbnailDto() },
            new ArtworkDto { Title = "B", Thumbnail = new ThumbnailDto() }
        };
        var apiResponse = new ArtworksApiResponseDto { Data = dtoList };

        var result = DTOMapper.ToArtPieces(apiResponse);

        Assert.Equal(2, System.Linq.Enumerable.Count(result));
        Assert.Contains(result, a => a.Title == "A");
        Assert.Contains(result, a => a.Title == "B");
    }

    [Fact]
    public void ToArtPieces_ReturnsEmptyOnNullData()
    {
        var apiResponse = new ArtworksApiResponseDto { Data = null! };
        var result = DTOMapper.ToArtPieces(apiResponse);
        Assert.Empty(result);
    }

    [Fact]
    public void ToArtPieces_ThrowsOnNullApiResponse()
    {
        Assert.Throws<System.ArgumentNullException>(() => DTOMapper.ToArtPieces(null));
    }
}