using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TacxArtExplorer.Models
{
    public enum ImageFormat
    {
        JPG,
        GIF,
        PNG
    }

    public static class ImageFormatExtensions
    {
        public static string ToFormatString(this ImageFormat format)
        {
            return format switch
            {
                ImageFormat.JPG => "jpg",
                ImageFormat.GIF => "gif",
                ImageFormat.PNG => "png",
                _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
            };
        }
    }
}
