using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TacxArtExplorer.Models
{
    public sealed record ArtPiece
    {
        public int Id { get; init; }
        public string Title { get; init; }
        public Artist Artist { get; init; }
        public string? LongDescription { get; init; }
        public string? ShortDescription { get; init; }
        public string ImageID { get; init; }
        public string? DisplayDate { get; init; }
        public string? ArtworkType { get; init; }
        public string? PlaceOfOrigin { get; init; }
        public Thumbnail Thumbnail { get; init; }

    }
}
