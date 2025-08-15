using Xunit;
using TacxArtExplorer.Models;
using TacxArtExplorer.Services;
using TacxArtExplorer.ViewModels;
using Moq;
using System.Collections.Generic;

namespace TacxArtExplorer.Tests.Services;

/// <summary>
/// Tests for NavigationService functionality:
/// - Navigates to list and sets CurrentViewModel
/// - Navigates to detail and sets CurrentViewModel
/// </summary>
public class NavigationServiceTests
{
    [Fact]
    public void NavigateToList_SetsCurrentViewModel()
    {
        var providerMock = new Mock<IServiceProvider>();
        var nav = new NavigationService(providerMock.Object);
        var listVm = new Mock<ArtListViewModel>(nav, null).Object;
        providerMock.Setup(p => p.GetService(typeof(ArtListViewModel))).Returns(listVm);

        nav.NavigateToList();

        Assert.Equal(listVm, nav.CurrentViewModel);
    }

    [Fact]
    public void NavigateToDetail_SetsCurrentViewModel()
    {
        var providerMock = new Mock<IServiceProvider>();
        var navigationMock = new Mock<INavigationService>();
        var nav = new NavigationService(providerMock.Object);
        var detailVm = new Mock<ArtDetailViewModel>(nav,null).Object;
        providerMock.Setup(p => p.GetService(typeof(ArtDetailViewModel))).Returns(detailVm);

        nav.NavigateToDetail(new ArtPiece(), new List<ArtPiece>());

        Assert.Equal(detailVm, nav.CurrentViewModel);
    }
}