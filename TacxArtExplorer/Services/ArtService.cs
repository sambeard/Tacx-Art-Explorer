    using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using TacxArtExplorer.Models;

namespace TacxArtExplorer.Services
{
    public interface IArtService
    {
        // Define methods that the ArtService should implement
        Task<IEnumerable<ArtPiece>?> GetArtPiecesByArtistAsync(Artist artist);
        Task<ArtPieceImage?> GetImageByIdAsync(string imageId, SizeOption? size = null, ImageFormat format = ImageFormat.JPG);
    }

    public class ArtService : IArtService
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
        public async Task<IEnumerable<ArtPiece>?> GetArtPiecesByArtistAsync(Artist artist)
        {
            IEnumerable<ArtPiece>? artPieces = null;
            try
            {
                artPieces = await _cacheService.GetArtPiecesByArtistAsync(artist);
                if (artPieces != null && artPieces.Any()) {
                    _logger.LogInformation("Returning cached art pieces for artist {Artist}", artist.Name);
                    return await Task.FromResult(artPieces);

                }
                _logger.LogInformation("No artpieces found in cache for artist {Artist}", artist.Name);
            }
            catch (Exception ex){
                _logger.LogError(ex, "Error while retreiving art pieces from cache for artist {Artist}", artist.Name);
            }
            // resort to API
            try
            {
                _logger.LogInformation("Fetching art pieces for artist {Artist} from API", artist.Name);
                artPieces = await _apiService.GetArtPiecesByArtistAsync(artist);
                if (artPieces != null && artPieces.Any())
                {
                    _logger.LogInformation("Storing fetched art pieces for artist {Artist} in cache", artist.Name);
                    // no need to await success of the store procedure
                    Task.Run(async () => {
                        try
                        {
                            var amnt = await _cacheService.InsertOrUpdateArtPiecesAsync(artPieces);
                            if (amnt == artPieces.Count())
                            {
                                _logger.LogInformation("Succesfully stored {count} art pieces for artist {Artist}", amnt, artist.Name);
                            }
                            else if (amnt > 0 ){
                                _logger.LogWarning("Only stored {count} / {total} art pieces", amnt, artPieces.Count());
                            }
                            else
                            {
                                throw new Exception("Could not insert or update art piece records");
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error while storing {Count} artpieces for artist {Artist}", artPieces?.Count() ?? -1, artist.Name);
                        }
                    });
                }
                else
                {
                    // return empty list if no art pieces found
                    _logger.LogInformation("No artpieces found for artist {Artist}", artist.Name);
                    artPieces = Enumerable.Empty<ArtPiece>();
                }
                return await Task.FromResult(artPieces.OrderBy(a => a.DisplayDate));

            }
            catch {
                return await Task.FromResult(artPieces);
            } 
        }

        public async Task<ArtPieceImage?> GetImageByIdAsync(string imageId, SizeOption? size = null, ImageFormat format = ImageFormat.JPG)
        {
            ArtPieceImage? image = null;
            try
            {
                image = await _cacheService.GetImageByIdAsync(imageId, size, format);

            }
            catch
            {
                image = null;
            }
            if (image is null) { 
                try
                {
                    image = await _apiService.GetImageByIdAsync(imageId, size, format);
                    Task.Run(async () =>
                    {
                        var amnt = await _cacheService.InsertOrUpdateImageAsync(image);
                        if (amnt > 0)
                            _logger.LogInformation("Succesfully inserted {count} image into cache", amnt);
                        else
                            _logger.LogWarning("Could not insert image with id {id} into cache", image.Id);
                    });
                }
                catch {
                    image = null;
                }
            }
            return image;
        }

    }
}
