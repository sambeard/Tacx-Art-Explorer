using Xunit;
using Moq;
using TacxArtExplorer.ViewModels;
using TacxArtExplorer.Services;

namespace TAETesting.ViewModels;

/// <summary>
/// Tests for MainWindowViewModel:
/// - Exposes CurrentViewModel from INavigationService
/// - Raises property changed when navigation changes
/// </summary>
public class MainWindowViewModelTests
{
    [Fact]
    public void CurrentViewModel_ReflectsNavigationService()
    {
        var navMock = new Mock<INavigationService>();
        var expectedVm = new object();
        navMock.Setup(n => n.CurrentViewModel).Returns(expectedVm);

        var vm = new MainWindowViewModel(navMock.Object);

        Assert.Equal(expectedVm, vm.CurrentViewModel);
    }

    [Fact]
    public void PropertyChanged_RaisedOnNavigationChange()
    {
        var navMock = new Mock<INavigationService>();
        var vm = new MainWindowViewModel(navMock.Object);

        bool raised = false;
        vm.PropertyChanged += (s, e) => { if (e.PropertyName == nameof(vm.CurrentViewModel)) raised = true; };

        navMock.Raise(n => n.CurrentViewModelChanged += null, new object());

        Assert.True(raised);
    }
}