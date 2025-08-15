using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TacxArtExplorer.Models;
using TacxArtExplorer.Services;

namespace TacxArtExplorer.ViewModels
{
    public sealed class ArtPieceViewModel : INotifyPropertyChanged
    {
        public ArtPiece Model { get; }
        private ImageSource _thumb;
        private ImageSource _full;
        public ArtPieceViewModel(ArtPiece model) => Model = model;

        public int Id => Model.Id;
        public string Title => Model.Title;
        public string ArtistName => Model.Artist?.Name ?? "Unknown";
        public string DisplayDate => Model.DisplayDate ?? "";
        public string LongDescription => Model.LongDescription ?? "";
        public string ArtworkType => Model.ArtworkType ?? "";
        public string PlaceOfOrigin => Model.PlaceOfOrigin ?? "";

        public ImageSource ThumbnailImage => _thumb ??= IImageSourceLoader.LoadFromThumbnail(Model.Thumbnail!);

        public string Summary => Model!.ShortDescription switch
        {
            null => Model!.Thumbnail?.AltText ?? "A piece by " + ArtistName,
            ""   => Model!.Thumbnail?.AltText ?? "A piece by " + ArtistName,
            string s when s.Length > 500 => s[..500] + "...",
            _ => Model.ShortDescription
        };





        public event PropertyChangedEventHandler PropertyChanged;
    }
}
