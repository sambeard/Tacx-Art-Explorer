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
            var artist = new Artist(dto.ArtistId ?? 0, dto.ArtistTitle ?? "");
            return new ArtPiece(
                id: 0, // ID is not in DTO, will be set by the service
                title: dto.Title,
                artist: artist,
                creationDate: DateTime.MinValue, // TODO: Creation date not in DTO
                description: dto.Description ?? "",
                imageId: "", // TODIO: Image ID not in DTO
                thumbnail: thumbnail);
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
