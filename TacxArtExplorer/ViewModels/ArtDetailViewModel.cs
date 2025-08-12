using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using TacxArtExplorer.Models;
using TacxArtExplorer.Services;

namespace TacxArtExplorer.ViewModels
{
    internal class ArtDetailViewModel : ObservableObject
    {
        private readonly INavigationService _nav;
        private ObservableCollection<ArtPiece> _context = new();
        private ArtPiece? _currentArtPiece;
        private ArtPieceViewModel? _currentVm;
        public ArtPieceViewModel? Current => _currentVm;

        public ArtPiece? CurrentArtPiece
        {
            get => _currentArtPiece;
            private set
            {
                if (SetProperty(ref _currentArtPiece, value))
                {
                    _currentVm = (value is null) ? null : new ArtPieceViewModel(value);
                    OnPropertyChanged(nameof(Current));
                    NextCommand.NotifyCanExecuteChanged();
                    PreviousCommand.NotifyCanExecuteChanged();
                }
            }
        }

        public IRelayCommand GoBackCommand { get; }
        public IRelayCommand NextCommand { get; }
        public IRelayCommand PreviousCommand { get; }

        public ArtDetailViewModel(INavigationService nav)
        {
            _nav = nav;

            GoBackCommand = new RelayCommand(() => _nav.NavigateToList(CurrentArtPiece));

            NextCommand = new RelayCommand(
                execute: () => Move(1),
                canExecute: () => _context.Count > 1 && CurrentArtPiece is not null
            );
            PreviousCommand = new RelayCommand(
                execute: () => Move(-1),
                canExecute: () => _context.Count > 1 && CurrentArtPiece is not null
            );
        }

        /// <summary>
        /// Call this immediately after construction to set the item and its context.
        /// </summary>
        public void Initialize(ArtPiece selected, List<ArtPiece> context)
        {
            _context.Clear();
            foreach (var a in context) _context.Add(a);
            CurrentArtPiece = selected;
        }

        /// <summary>
        /// Move with wrap around
        /// </summary>
        /// <param name="offset">integer offset from the current item</param>
        private void Move(int offset)
        {
            if (CurrentArtPiece is null || _context.Count() <= 1) return;
            var idx = WrapAroundIndex(CurrentIndex + offset);
            CurrentArtPiece = _context[idx];
        }

        private int CurrentIndex => (CurrentArtPiece is null) ? 0 : _context.IndexOf(CurrentArtPiece);

        private int WrapAroundIndex(int index)
        {
            if (_context.Count == 0) return 0;
            return (index + _context.Count) % _context.Count;
        }
    }
}