using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Navigation;
using TacxArtExplorer.Models;
using TacxArtExplorer.Services;

namespace TacxArtExplorer.ViewModels
{
    internal class ArtListViewModel : ObservableObject
    {
        private readonly IArtService _artService;
        private readonly INavigationService _nav;

        // set to standard artist for now
        private Artist _artist = new Artist(35809, "Claude Monet");
        private bool _isLoading;
        private ArtPieceViewModel? _selectedArtPiece;
        private ArtPieceViewModel? _focusedArtPiece;

        public ObservableCollection<ArtPieceViewModel> ArtPieces { get; } = new();

        public bool IsLoading
        {
            get => _isLoading;
            private set
            {
                if (SetProperty(ref _isLoading, value))
                    LoadArtPiecesCommand.NotifyCanExecuteChanged();
            }
        }
        public ArtPieceViewModel? SelectedArtPiece
        {
            get => _selectedArtPiece;
            set
            {
                if (SetProperty(ref _selectedArtPiece, value))
                    SelectArtPieceCommand.NotifyCanExecuteChanged();
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
        public IRelayCommand SelectArtPieceCommand { get; }
        public IRelayCommand FocusArtPieceCommand { get; }

        public ArtListViewModel(ArtService artService, INavigationService nav)
        {
            _artService = artService;
            _nav = nav;

            LoadArtPiecesCommand = new AsyncRelayCommand(
                execute: LoadArtPiecesAsync,
                canExecute: () => !IsLoading && _artist != null
            );

            SelectArtPieceCommand = new RelayCommand(
                execute: () =>
                {
                    _nav.NavigateToDetail(SelectedArtPiece!.Model, ArtPieces.Select(vm=>vm.Model).ToList());
                },
                canExecute: () => SelectedArtPiece != null
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
                    SelectedArtPiece = match;
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
