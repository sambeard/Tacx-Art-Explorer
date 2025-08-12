using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TacxArtExplorer.Models
{
    public record Thumbnail
    {
        public string LQIP { get; set; }
        public string AltText { get; set; }
        public (int,int) Dimensions { get; set; }

        public Thumbnail(string lqip, string altText, (int, int) dimensions)
        {
            LQIP = lqip;
            AltText = altText;
            Dimensions = dimensions;
        }
    }
}
