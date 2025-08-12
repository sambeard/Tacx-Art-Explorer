using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using TacxArtExplorer.Models;

namespace TacxArtExplorer.Services
{
    internal interface IArtService
    {
        const int DEFAULT_PAGE_SIZE = 10;
        // Define methods that the ArtService should implement
        Task<IEnumerable<ArtPiece>?> GetArtPiecesByArtistAsync(Artist artist, int page = 0, int limit = DEFAULT_PAGE_SIZE);
        Task<ArtPieceImage?> GetImageByIdAsync(string imageId, SizeOption? size = null, ImageFormat format = ImageFormat.JPG);
    }

    internal class ArtService : IArtService
    {
        // This class is a placeholder for the ArtService implementation.
        // It will contain methods to interact with the Art API, such as fetching art pieces, artists, etc.
        // The actual implementation will depend on the specific requirements of the application.
        
        // Example method signatures:

        private IArtCacheService _cacheService;
        private IArtAPIService _apiService;
        private ILogger<ArtService> _logger;

        public ArtService(IArtAPIService apiService, IArtCacheService cacheService, ILogger<ArtService> logger)
        {
            _apiService = apiService;
            _cacheService = cacheService;
            _logger = logger;
        }
        public async Task<IEnumerable<ArtPiece>?> GetArtPiecesByArtistAsync(Artist artist, int page = 0, int limit = 10)
        {
            // mock slow connection
            await Task.Delay(2500);
            if (await _cacheService.GetArtPiecesByArtistAsync(artist, page, limit) is IEnumerable<ArtPiece> cachedArtPieces &&
                cachedArtPieces != null && cachedArtPieces.Any()) 
            {    
                _logger.LogInformation("Returning cached art pieces for artist {Artist}", artist.Name);
                return await Task.FromResult(cachedArtPieces);
            }
            else {
                _logger.LogInformation("Fetching art pieces for artist {Artist} from API", artist.Name);
                var artPieces = await _apiService.GetArtPiecesByArtistAsync(artist, page, limit);
                if (artPieces != null && artPieces.Any())
                {
                    _logger.LogInformation("Storing fetched art pieces for artist {Artist} in cache", artist.Name);
                    // no need to await success of the store procedure
                    _cacheService.StoreArtPieces(artist, page * limit, (page + 1) * limit, artPieces);
                }
                else {
                    // return empty list if no art pieces found
                    artPieces = Enumerable.Empty<ArtPiece>();
                }
                return await Task.FromResult(artPieces);

            } 
        }

        public Task<ArtPieceImage?> GetImageByIdAsync(string imageId, SizeOption? size = null, ImageFormat format = ImageFormat.JPG)
        {
            throw new NotImplementedException();
        }

    }
}
