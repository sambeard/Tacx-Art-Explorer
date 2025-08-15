using Xunit;
using Moq;
using TacxArtExplorer.ViewModels;
using TacxArtExplorer.Models;
using TacxArtExplorer.Services;
using System.Collections.Generic;

namespace TAETesting.ViewModels;

/// <summary>
/// Tests for ArtDetailViewModel:
/// - Initializes with selected art piece and context
/// - Moves to next/previous art piece with wrap-around
/// - GoBackCommand navigates to list
/// </summary>
public class ArtDetailViewModelTests
{
    [Fact]
    public void Initialize_SetsCurrentArtPieceAndContext()
    {
        var navMock = new Mock<INavigationService>();
        var imMock = new Mock<IImageRepsitory>();
        var vm = new ArtDetailViewModel(navMock.Object, imMock.Object);

        var art1 = new ArtPiece { Id = 1, Title = "A" };
        var art2 = new ArtPiece { Id = 2, Title = "B" };
        var context = new List<ArtPiece> { art1, art2 };

        vm.Initialize(art2, context);

        Assert.Equal(art2, vm.CurrentArtPiece);
        Assert.Equal("B", vm.Current.Model.Title);
    }

    [Fact]
    public void NextCommand_MovesToNextWithWrapAround()
    {
        var navMock = new Mock<INavigationService>();
        var imMock = new Mock<IImageRepsitory>();
        var vm = new ArtDetailViewModel(navMock.Object, imMock.Object);

        var art1 = new ArtPiece { Id = 1, Title = "A" };
        var art2 = new ArtPiece { Id = 2, Title = "B" };
        var context = new List<ArtPiece> { art1, art2 };

        vm.Initialize(art1, context);
        vm.NextCommand.Execute(null);

        Assert.Equal(art2, vm.CurrentArtPiece);

        vm.NextCommand.Execute(null);

        Assert.Equal(art1, vm.CurrentArtPiece); // wrap around
    }

    [Fact]
    public void GoBackCommand_NavigatesToList()
    {
        var navMock = new Mock<INavigationService>();
        var imMock = new Mock<IImageRepsitory>();
        var vm = new ArtDetailViewModel(navMock.Object, imMock.Object);

        var art = new ArtPiece { Id = 1, Title = "A" };
        vm.Initialize(art, new List<ArtPiece> { art });

        vm.GoBackCommand.Execute(null);

        navMock.Verify(n => n.NavigateToList(art), Times.Once);
    }
}