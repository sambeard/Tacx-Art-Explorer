using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Navigation;
using TacxArtExplorer.Models;
using TacxArtExplorer.Services;

namespace TacxArtExplorer.ViewModels
{
    public class ArtListViewModel : ObservableObject
    {
        private readonly IArtService _artService;
        private readonly INavigationService _nav;
        private readonly IArtCacheService _cacheService;

        // set to standard artist for now
        private Artist _artist = new Artist(35809, "Claude Monet");
        private bool _isLoading;
        private ArtPieceViewModel? _focusedArtPiece;

        public ObservableCollection<ArtPieceViewModel> ArtPieces { get; } = new();

        public string SelectedArtistName => _artist.Name;

        public bool IsLoading
        {
            get => _isLoading;
            private set
            {
                if (SetProperty(ref _isLoading, value))
                    LoadArtPiecesCommand.NotifyCanExecuteChanged();
            }
        }

        public ArtPieceViewModel? FocusedArtPiece
        {
            get => _focusedArtPiece;
            set  
            {
                if (value == null && ArtPieces.Any())
                    value = ArtPieces.First(); // fallback to first item if null
                if (SetProperty(ref _focusedArtPiece, value)) {
                    FocusArtPieceCommand.NotifyCanExecuteChanged();
                } 
            }
        }
        public IAsyncRelayCommand LoadArtPiecesCommand { get; }
        public IAsyncRelayCommand InvalidateCacheCommand { get; }
        public IRelayCommand<ArtPieceViewModel> SelectArtPieceCommand { get; }
        public IRelayCommand FocusArtPieceCommand { get; }

        public ArtListViewModel(INavigationService nav, [FromKeyedServices("ArtStore")] IArtService artService, IArtCacheService cacheService)
        {
            _artService = artService;
            _cacheService = cacheService;
            _nav = nav;

            LoadArtPiecesCommand = new AsyncRelayCommand(
                execute: LoadArtPiecesAsync,
                canExecute: () => !IsLoading && _artist != null
            );

            InvalidateCacheCommand = new AsyncRelayCommand(
                execute: InvalidateCache
                );

            SelectArtPieceCommand = new RelayCommand<ArtPieceViewModel>(
                execute: (selectedItem) =>
                {

                    _nav.NavigateToDetail(selectedItem!.Model, ArtPieces.Select(vm=>vm.Model).ToList());
                },
                canExecute: (selectedItem) => selectedItem != null
            );
            FocusArtPieceCommand = new RelayCommand(
                execute: () =>
                {
                    // empty for now, but could be used later to scroll to the focused art piece in the UI
                    return;
                },
                canExecute: () => FocusedArtPiece != null
            );
        }
        private async Task InvalidateCache() {
            await _cacheService.RemoveArtPiecesForArtistAsync(_artist);
            foreach (var ap in ArtPieces) {
                await _cacheService.RemoveImagesByIdAsync(ap.Model.ImageID);
            }
            ArtPieces.Clear();
            await LoadArtPiecesAsync();
        }
        private async Task LoadArtPiecesAsync()
        {
            IsLoading = true;
            try
            {

                var list = await _artService.GetArtPiecesByArtistAsync(_artist);
                ArtPieces.Clear();
                foreach (var ap in list)
                    ArtPieces.Add(new ArtPieceViewModel(ap)); 

                if (FocusedArtPiece != null && FocusedArtPiece.Model != null)
                {
                    var id = FocusedArtPiece.Model.Id;
                    var match = ArtPieces.FirstOrDefault(a => a.Model.Id == id) ?? ArtPieces.FirstOrDefault();
                    FocusedArtPiece = match;
                }
            }
            catch
            {
                // TODO: expose an ErrorMessage property and set it here
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
