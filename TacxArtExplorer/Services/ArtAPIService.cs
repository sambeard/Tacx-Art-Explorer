using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using TacxArtExplorer.Models;
using Microsoft.Extensions.Logging;
using TacxArtExplorer.Services.HTTPClients;
using TacxArtExplorer.Models.DTO;

namespace TacxArtExplorer.Services
{


    public interface IArtAPIService : IArtService { }

    public class ArtAPIService : IArtAPIService
    {
        const int DEFAULT_PAGE_SIZE = 10;

        private readonly IArticImageClient _imClient;
        private readonly IArticArtworkClient _apClient;
        private readonly ILogger<ArtAPIService> _logger;

        public ArtAPIService(IArticArtworkClient apClient, IArticImageClient imClient, ILogger<ArtAPIService> logger)
        {
            _imClient = imClient;
            _apClient = apClient;
            _logger = logger;
        }

        public async Task<IEnumerable<ArtPiece>?> GetArtPiecesByArtistAsync(Artist artist)
        {
            // mock slow connection
            await Task.Delay(2500);
            IEnumerable<ArtPiece>? aps = [];
            try
            {
                _logger.LogInformation("Fetching artworks for artist {Artist} from API", artist.Name);
                bool shouldQueryApi = true;
                int page = 0;
                while (shouldQueryApi) {
                    // 1-based index pagination
                    var p = new PaginationDTO { Page = ++page, Limit = DEFAULT_PAGE_SIZE };
                    var response = await _apClient.GetArtworksFromArtistId(artist.Id, p);
                    if (response != null && response.Data != null && response.Data.Any())
                    {
                        aps = aps.Concat(DTOMapper.ToArtPieces(response));
                        _logger.LogInformation("Fetched {Count} artworks for artist {Artist}", aps.Count(), artist.Name);
                    }
                    else
                    {
                        shouldQueryApi = false; // no more artworks to fetch
                        _logger.LogWarning("No artworks found for artist {Artist} and page {Page}", artist.Name, page);
                        continue;
                    }
                    shouldQueryApi &= response.Pagination.CurrentPage < response.Pagination.TotalPages; // continue if there are more pages
                }
            }
            catch (Exception ex)
            {
                aps = null;
                _logger.LogError(ex, "Error fetching or parsing artWorks for {Artist} from API", artist.Name);
            }
            return aps;
        }

        public async Task<ArtPieceImage?> GetImageByIdAsync(string imageId, SizeOption? size, ImageFormat format)
        {
            // mock slow connection
            await Task.Delay(1500);

            try
            {
                return await _imClient.GetImageById(imageId, size, format);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Image with id {ImageId}", imageId);
                return null;
            }
        }
    }
}
