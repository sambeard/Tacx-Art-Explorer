using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace TacxArtExplorer.Models
{
    public sealed record ArtPieceImage
    {
        public required string Id { get; init; }
        public ImageFormat Format { get; init; }
        public SizeOption  SizeOption { get; init; }
        public required byte[] Data { get; init; }
    }
}
