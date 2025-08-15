using Xunit;
using Moq;
using TacxArtExplorer.ViewModels;
using TacxArtExplorer.Models;
using TacxArtExplorer.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TAETesting.ViewModels;

/// <summary>
/// Tests for ArtListViewModel:
/// - Loads art pieces via command
/// - Selects and focuses art pieces
/// - Navigates to detail on selection
/// </summary>
public class ArtListViewModelTests
{
    [Fact]
    public async Task LoadArtPiecesCommand_LoadsArtPieces()
    {
        var artPieces = new List<ArtPiece> { new ArtPiece { Id = 1, Title = "Test Art" } };
        var artServiceMock = new Mock<IArtService>();
        var artCacheMock = new Mock<IArtCacheService>();
        artServiceMock.Setup(s => s.GetArtPiecesByArtistAsync(It.IsAny<Artist>()))
            .ReturnsAsync(artPieces);

        var navMock = new Mock<INavigationService>();
        var viewModel = new ArtListViewModel(navMock.Object, artServiceMock.Object, artCacheMock.Object);

        await viewModel.LoadArtPiecesCommand.ExecuteAsync(null);

        Assert.Single(viewModel.ArtPieces);
        Assert.Equal("Test Art", viewModel.ArtPieces[0].Model.Title);
    }

    [Fact]
    public void SelectArtPieceCommand_NavigatesToDetail()
    {
        var artPiece = new ArtPiece { Id = 2, Title = "Detail Art" };
        var artServiceMock = new Mock<ArtService>(null,null,null);
        var navMock = new Mock<INavigationService>();
        var artCacheMock = new Mock<IArtCacheService>();

        var viewModel = new ArtListViewModel(navMock.Object, artServiceMock.Object, artCacheMock.Object);

        viewModel.ArtPieces.Add(new ArtPieceViewModel(artPiece));

        viewModel.SelectArtPieceCommand.Execute(viewModel.ArtPieces.First());

        navMock.Verify(n => n.NavigateToDetail(artPiece, It.IsAny<List<ArtPiece>>()), Times.Once);
    }

    [Fact]
    public void FocusArtPieceCommand_EnabledWhenFocused()
    {
        var artServiceMock = new Mock<ArtService>(null,null,null);
        var navMock = new Mock<INavigationService>();
        var artCacheMock = new Mock<IArtCacheService>();

        var viewModel = new ArtListViewModel(navMock.Object, artServiceMock.Object, artCacheMock.Object);

        var artPieceVm = new ArtPieceViewModel(new ArtPiece { Id = 3, Title = "Focus Art" });
        viewModel.ArtPieces.Add(artPieceVm);
        viewModel.FocusedArtPiece = artPieceVm;

        Assert.True(viewModel.FocusArtPieceCommand.CanExecute(null));
    }
}