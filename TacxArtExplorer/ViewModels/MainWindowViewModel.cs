using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TacxArtExplorer.Models;
using TacxArtExplorer.Services;

namespace TacxArtExplorer.ViewModels;

public class MainWindowViewModel : ObservableObject {
    private readonly INavigationService _nav;
    public object CurrentViewModel => _nav.CurrentViewModel;

    public MainWindowViewModel(INavigationService nav)
    {
        _nav = nav;
        _nav.CurrentViewModelChanged += _ => OnPropertyChanged(nameof(CurrentViewModel));
    }
}


