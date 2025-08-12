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

namespace TacxArtExplorer.ViewModels
{
    public sealed class ArtPieceViewModel : INotifyPropertyChanged
    {
        private const string DefaultLQIP = "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQAAAQABAAD/2wBDABcQERQRDhcUEhQaGBcbIjklIi4uLzUrRk5bY3h4eY+Tn6KqqrS0trrCwsjExczQ1dfHxv///w8PDy8vLzMzMzP/2wBDAhgaGiwjLzIzMzMzMzMzMzMzMzMzMzMzMzMzMzMzMzMzMzMzMzMzMzMzMzMzMzMzMzMzMzMzMzP/wAARCAARAEgDASIAAhEBAxEB/8QAGQAAAgMBAAAAAAAAAAAAAAAAAwQCBQYB/8QAJxAAAgEDAwMDBQAAAAAAAAAAAQIDAAQREiExBVEGExQiQWGBoRT/xAAXAQEBAQEAAAAAAAAAAAAAAAAAAwIE/8QAHBEAAgIDAQAAAAAAAAAAAAAAAAECERIhAxMx/9oADAMBAAIRAxEAPwCksgkD4V5SNuJRr6z6s+Rb6NnV4EitFoUJpLW7H1c/Ipm4iZpZqjMaYQuN6cjqcR9Jym1sErNo/fp4hhI9lGh0EM0C1GBuMXhEXDNr64aFuIYzq3nYY6pVu3nq7WRhMYh3TgSAtH9rKkQuKY5Vq2ne6QSBpTbV8FrZ5ITbUs0+X9c8X6yCjveSm3nWzCBPYGpPqMHZ+J7mPNXnKcJq7M6ae+3btFbcaNKW4H82wHEj6ey6VhY7jtDP3l0TTqKWhrblYaTSyRVG3QrdTOhNX0uzxEo0U4yHikvAka9omIq47hXNeaTnd+0QqMdUXEdUZV2/iaMx3MFvn5DybTUcz4ahpGbe3nhaRx06zZzvve+W3AZsuMZMzM1TT5XjfbUpRF1S7o4qDZ2XhaD0PstbyopnmWypfjbEKjTFdUBZHRf/2Q==";
        public ArtPiece Model { get; }
        public ArtPieceViewModel(ArtPiece model) => Model = model;

        public int Id => Model.Id;
        public string Title => Model.Title;
        public string ArtistName => Model.Artist?.Name ?? "Unknown";

        public string Summary =>
            string.IsNullOrWhiteSpace(Model.Description)
                ? Model!.Thumbnail?.AltText ?? "A piece by " + ArtistName
                : Model!.Description;

        private ImageSource _thumb;
        public ImageSource ThumbnailImage => _thumb ??= CreateImage(Model.Thumbnail?.LQIP ?? DefaultLQIP);

        private static ImageSource CreateImage(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;

            // handle data URIs
            var comma = s.IndexOf(',');
            if (comma > 0 && s.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
                s = s[(comma + 1)..];

            try
            {
                var bytes = Convert.FromBase64String(s);
                using var ms = new MemoryStream(bytes);
                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.StreamSource = ms;
                bmp.EndInit();
                bmp.Freeze();
                return bmp;
            }
            catch { return null; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
