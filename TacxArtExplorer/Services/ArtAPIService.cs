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


    internal interface IArtAPIService : IArtService { }

    internal class ArtAPIService : IArtAPIService
    {
        private readonly ArticImageClient _imClient;
        private readonly ArticArtworkClient _apClient;
        private readonly ILogger<ArtAPIService> _logger;

        public ArtAPIService(ArticArtworkClient apClient, ArticImageClient imClient, ILogger<ArtAPIService> logger)
        {
            _imClient = imClient;
            _apClient = apClient;
            _logger = logger;
        }

        public async Task<IEnumerable<ArtPiece>?> GetArtPiecesByArtistAsync(Artist artist, int page, int limit)
        {
            IEnumerable<ArtPiece>? aps = null;
            try
            {
                _logger.LogInformation("Fetching ArtPieces for artist {Artist} from API", artist.Name);
                var endpoint = _apClient.GetArtworksEndpoint(artist, page, limit);
                var response = (ArtworksApiResponseDto)await _apClient.GetAsync(endpoint);
                if (response != null && response.Data != null && response.Data.Any())
                {
                    aps = DTOMapper.ToArtPieces(response);
                    _logger.LogInformation("Fetched {Count} art pieces for artist {Artist}", aps.Count(), artist.Name);
                }
                else
                {
                    _logger.LogWarning("No art pieces found for artist {Artist}", artist.Name);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching or parsing ArtPieces for {Artist}", artist.Name);
            }
            return await Task.FromResult(aps);
        }

        public async Task<ArtPieceImage?> GetImageByIdAsync(string imageId, SizeOption? size, ImageFormat format)
        {
            throw new NotImplementedException();
            //var _size = size ?? SizeOption.Default;
            //var url = imageSearchPath(imageId, _size, format);
            //try
            //{
            //    var response = await _http.GetAsync(url);
            //    response.EnsureSuccessStatusCode();

            //    var data = await response.Content.ReadAsByteArrayAsync();
            //    ArtPieceImage image = new(imageId, format, _size, data);
            //    return await Task.FromResult(image);
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "Error fetching Image with id {ImageId}", imageId);
            //    return await Task.FromResult<ArtPieceImage?>(null);
            //}
        }
        
    
        private string imageSearchPath(string imageId, SizeOption size, ImageFormat format) => $"images/{Uri.EscapeDataString(imageId)}/full/{size.ToString()}/0/default.{format.ToString().ToLower()}";
    }
}
