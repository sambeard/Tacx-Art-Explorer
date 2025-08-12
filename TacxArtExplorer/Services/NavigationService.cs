using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TacxArtExplorer.Models;
using TacxArtExplorer.ViewModels;

namespace TacxArtExplorer.Services
{

    public interface INavigationService
    {
        /// <summary>
        /// The currently active ViewModel (bound by MainViewModel to the ContentControl in the Window).
        /// </summary>
        object CurrentViewModel { get; }

        /// <summary>
        /// Fired whenever CurrentViewModel changes.
        /// </summary>
        event Action<object>? CurrentViewModelChanged;

        /// <summary>
        /// Show the list screen, focusing on the last viewed artwork if available.
        /// </summary>
        void NavigateToList(ArtPiece? lastViewed = null);

        /// <summary>
        /// Show the detail screen for the selected artwork, passing in the full list for prev/next.
        /// </summary>
        void NavigateToDetail(ArtPiece selected, List<ArtPiece> contextList);
    }

    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _provider;
        private object _currentViewModel;

        public object CurrentViewModel
        {
            get => _currentViewModel;
            private set
            {
                _currentViewModel = value;
                CurrentViewModelChanged?.Invoke(value);
            }
        }

        public event Action<object>? CurrentViewModelChanged;

        public NavigationService(IServiceProvider provider)
        {
            _provider = provider;
            _currentViewModel = new { };    // set empty initial value to handle DI chain loop

        }

        public void NavigateToList(ArtPiece? lastViewed=null)
        {
            var listVm = getViewModel<ArtListViewModel>();
            listVm.FocusedArtPiece = new ArtPieceViewModel(lastViewed);
            listVm.LoadArtPiecesCommand.ExecuteAsync(null); // load the list of artworks
            CurrentViewModel = listVm;
        }

        public void NavigateToDetail(ArtPiece selected, List<ArtPiece> contextList)
        {

            var detailVm = getViewModel<ArtDetailViewModel>();
            detailVm.Initialize(selected, contextList);
            CurrentViewModel = detailVm;
        }

        private T getViewModel<T>() where T:ObservableObject{
            return (T)_provider.GetService(typeof(T))!;
        }
    }
}
