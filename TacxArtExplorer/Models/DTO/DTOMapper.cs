using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TacxArtExplorer.Models.DTO
{
    static class DTOMapper
    {
        public static Thumbnail ToThumbnail(ThumbnailDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            return new Thumbnail(
                lqip: dto.Lqip ?? "",
                altText: dto.AltText ?? "",
                dimensions: (dto.Width, dto.Height));
        }

        public static ArtPiece ToArtPiece(ArtworkDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            var thumbnail = ToThumbnail(dto.Thumbnail);
            return new ArtPiece {
                Id= dto.Id, 
                Title= dto.Title,
                Artist= new Artist(dto.ArtistId??-1, dto.ArtistTitle??""),
                DisplayDate= dto.DisplayDate ?? "",
                ArtworkType= dto.ArtworkTypeTitle ?? "",
                PlaceOfOrigin= dto.PlaceOfOrigin ?? "",
                ShortDescription= dto.ShortDescription ?? "",
                LongDescription= dto.Description ?? "",
                ImageID= dto.ImageId ?? "",
                Thumbnail= thumbnail
            };
        }

        public static IEnumerable<ArtPiece> ToArtPieces(ArtworksApiResponseDto apiResponseDto)
        {
            if (apiResponseDto == null) throw new ArgumentNullException(nameof(apiResponseDto));
            if (apiResponseDto.Data == null || !apiResponseDto.Data.Any())
            {
                return Enumerable.Empty<ArtPiece>();
            }
            return apiResponseDto.Data.Select(ToArtPiece);
        }

    }
}
