using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TacxArtExplorer.Models;
using TacxArtExplorer.Models.DTO;

namespace TacxArtExplorer.Services.HTTPClients
{
    public interface IArticImageClient {
        public Task<ArtPieceImage?> GetImageById(string imageId, SizeOption? size = default, ImageFormat? format = default);

    }
    public class ArticImageClient : IArticImageClient
    {
        internal record ImageByIdRequest : IRequest
        {
            private string _imageId;
            private SizeOption _size;
            private ImageFormat _format;

            public ImageByIdRequest(string imageId, SizeOption size, ImageFormat format)
            {
                _imageId = imageId;
                _size = size;
                _format = format;
            }
            public string BuildUri() => $"{Uri.EscapeDataString(_imageId)}/full/{_size.ToString()}/0/default.{_format.ToString().ToLower()}";
        }

        private readonly ArticApiClient _articClient;
        private readonly ILogger<ArticImageClient> _logger;
        public ArticImageClient([FromKeyedServices("ImageClient")] IArticApiClient articApiClient, ILogger<ArticImageClient> logger)
        {
            _articClient = (ArticApiClient)articApiClient;
            _logger = logger;
        }

        public async Task<ArtPieceImage?> GetImageById(string imageId, SizeOption? size = null, ImageFormat? format=ImageFormat.JPG)
        {
            if (string.IsNullOrWhiteSpace(imageId))
                throw new ArgumentException("Image ID cannot be null or empty", nameof(imageId));
            // ignore null options
            size ??= SizeOption.Default;
            format ??= ImageFormat.JPG;
            var request = new ImageByIdRequest(imageId, size.Value, format.Value);
            ArtPieceImage image = null;
            try
            {
                _logger.LogInformation("Fetching image with id {Artist} from API", imageId);
                var response = await _articClient.GetAsync(request.BuildUri());
                response.EnsureSuccessStatusCode();
                var data = await response.Content.ReadAsByteArrayAsync();
                if (data == null || data.Length == 0)
                    throw new InvalidOperationException($"No data returned for image ID {imageId}");
                image = new ArtPieceImage { Id = imageId, Format = format.Value, SizeOption = size.Value, Data = data };
                }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching image with id {ImageId} from API", imageId);
            }
            return image;
        }
    }
}
