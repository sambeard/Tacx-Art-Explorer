using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace TacxArtExplorer.Models
{
    internal record ArtPieceImage
    {
        public string Id { get; set; }
        public ImageFormat Format { get; set; }
        public SizeOption  SizeOption { get; set; }
        public byte[] Data { get; set; }

        public ArtPieceImage(string id, ImageFormat format, SizeOption sizeOption, byte[] data)
        {
            Id = id;
            Format = format;
            SizeOption = sizeOption;
            Data = data;
        }
    }
}
