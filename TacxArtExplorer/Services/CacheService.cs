using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TacxArtExplorer.Models;

namespace TacxArtExplorer.Services
{
    internal interface IArtCacheService : IArtService { 
    
        Task StoreArtPieces(Artist artist, int start, int stop, IEnumerable<ArtPiece> artPieces);
        Task StoreImage(string imageId, byte[] imageData, SizeOption? size = null, ImageFormat format = ImageFormat.JPG);
        Task StoreImage(ArtPieceImage image);
    }

    internal class CacheService : IArtCacheService
    {
        public async Task<IEnumerable<ArtPiece>?> GetArtPiecesByArtistAsync(Artist artist, int page = 0, int limit = 10)
        {
            // cache never hits for now
            return await Task.FromResult<IEnumerable<ArtPiece>?>(null);
        }

        public async Task<ArtPieceImage?> GetImageByIdAsync(string imageId, SizeOption? size = null, ImageFormat format = ImageFormat.JPG)
        {
            return await Task.FromResult<ArtPieceImage?>(null);
        }


        public async Task StoreArtPieces(Artist artist, int start, int stop, IEnumerable<ArtPiece> artPieces)
        {
            await Task.CompletedTask;
        }

        public async Task StoreImage(string imageId, byte[] imageData, SizeOption? size = null, ImageFormat format = ImageFormat.JPG)
        {
            await Task.CompletedTask;
        }

        public async Task StoreImage(ArtPieceImage image)
        {
            await StoreImage(image.Id, image.Data, image.SizeOption, image.Format);
        }
    }


}
