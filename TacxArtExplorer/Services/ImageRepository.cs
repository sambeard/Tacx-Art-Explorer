using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TacxArtExplorer.Models;

namespace TacxArtExplorer.Services
{
    public interface IImageSourceLoader
    {
        private const string DefaultLQIP = "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQAAAQABAAD/2wBDABcQERQRDhcUEhQaGBcbIjklIi4uLzUrRk5bY3h4eY+Tn6KqqrS0trrCwsjExczQ1dfHxv///w8PDy8vLzMzMzP/2wBDAhgaGiwjLzIzMzMzMzMzMzMzMzMzMzMzMzMzMzMzMzMzMzMzMzMzMzMzMzMzMzMzMzMzMzMzMzP/wAARCAARAEgDASIAAhEBAxEB/8QAGQAAAgMBAAAAAAAAAAAAAAAAAwQCBQYB/8QAJxAAAgEDAwMDBQAAAAAAAAAAAQIDAAQREiExBVEGExQiQWGBoRT/xAAXAQEBAQEAAAAAAAAAAAAAAAAAAwIE/8QAHBEAAgIDAQAAAAAAAAAAAAAAAAECERIhAxMx/9oADAMBAAIRAxEAPwCksgkD4V5SNuJRr6z6s+Rb6NnV4EitFoUJpLW7H1c/Ipm4iZpZqjMaYQuN6cjqcR9Jym1sErNo/fp4hhI9lGh0EM0C1GBuMXhEXDNr64aFuIYzq3nYY6pVu3nq7WRhMYh3TgSAtH9rKkQuKY5Vq2ne6QSBpTbV8FrZ5ITbUs0+X9c8X6yCjveSm3nWzCBPYGpPqMHZ+J7mPNXnKcJq7M6ae+3btFbcaNKW4H82wHEj6ey6VhY7jtDP3l0TTqKWhrblYaTSyRVG3QrdTOhNX0uzxEo0U4yHikvAka9omIq47hXNeaTnd+0QqMdUXEdUZV2/iaMx3MFvn5DybTUcz4ahpGbe3nhaRx06zZzvve+W3AZsuMZMzM1TT5XjfbUpRF1S7o4qDZ2XhaD0PstbyopnmWypfjbEKjTFdUBZHRf/2Q==";
        private static ImageSource? createImage(byte[] bytes) {
            try
            {
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

        private static ImageSource? createImage(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;

            // handle data URIs
            var comma = s.IndexOf(',');
            if (comma > 0 && s.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
                s = s[(comma + 1)..];
            try
            {
                var bytes = Convert.FromBase64String(s);
                return createImage(bytes);
            }
            catch {
                return null;
            }

        }
        public static ImageSource? LoadFromThumbnail(Thumbnail? thumbnail) => createImage(thumbnail?.LQIP??DefaultLQIP);
        public static ImageSource? LoadFromArtPieceImage(ArtPieceImage image) => createImage(image.Data);

    }
    public interface IImageRepsitory {
        public bool HasImage(string id);

        public Task<ImageSource?> GetImageSource(string id, SizeOption? size = default);
    }

    public class ImageRepository : IImageRepsitory
    {
        private ImageFormat _defaultFormat = ImageFormat.PNG;
        private IArtService _artService;

        private Dictionary<string, IEnumerable<(ArtPieceImage,ImageSource)>> _images;

        public ImageRepository([FromKeyedServices("ArtStore")] IArtService artService) {
            _artService = artService;
            _images = new Dictionary<string, IEnumerable<(ArtPieceImage,ImageSource)>>();
        }

        public async Task<ImageSource?> GetImageSource(string id, SizeOption? size)
        {
            // lookup the image in the dictionary
            if (HasImage(id) && _images.TryGetValue(id, out var imageTuples)) {
                var match = imageTuples.FirstOrDefault(i => i.Item1.SizeOption == size);
                return match.Item2 ?? imageTuples.First().Item2;
            }
            // if not present load from artservice
            var apImage = await _artService.GetImageByIdAsync(id, size, _defaultFormat);
            if (apImage is null) return null;
            var imSource = IImageSourceLoader.LoadFromArtPieceImage(apImage);
            if(imSource is not null) {
                // add to dictionary
                _images.Add(id, new[] { (apImage, imSource!) });
                return imSource;
            }
            return null;
        }

        public bool HasImage(string id) => _images.ContainsKey(id) && _images[id].Any(e=>e.Item2 is not null);
    }

}
