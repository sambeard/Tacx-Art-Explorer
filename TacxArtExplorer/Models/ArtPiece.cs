using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TacxArtExplorer.Models
{
    public record ArtPiece
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public Artist Artist { get; set; }
        public DateTime CreationDate { get; set; }
        public string Description { get; set; }
        public string ImageID { get; set; }
        public Thumbnail Thumbnail { get; set; }

        public ArtPiece(int id, string title, Artist artist, DateTime creationDate, string description, string imageId, Thumbnail thumbnail)
        {
            Id = id;
            Title = title;
            Artist = artist;
            CreationDate = creationDate;
            Description = description;
            ImageID = imageId;
            Thumbnail = thumbnail;

        }

    }
}
