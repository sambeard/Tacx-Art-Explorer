using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TacxArtExplorer.Models;

namespace TacxArtExplorer.Services
{
    public class BackgroundSyncer : BackgroundService
    {
        private IArtCacheService _cacheService;
        private IArtAPIService _apiService;
        private ILogger<BackgroundSyncer> _logger;

        public BackgroundSyncer(IArtCacheService cacheService, IArtAPIService apiService, ILogger<BackgroundSyncer> logger) {
            _cacheService = cacheService;
            _apiService = apiService;
            _logger = logger;
        }

        private async Task<int> performSync()
        {
            IEnumerable<Artist> artists;
            try
            {
                _logger.LogInformation("Retreiving saved artists from db");
                artists = await _cacheService.GetArtistsAsync();
            }
            catch (Exception ex){
                _logger.LogError(ex,"Could not retreive artist from db, aborting sync");
                return -1;
            }
            var count = 0;
            foreach (var artist in artists)
            {
                try
                {
                    var artpieces = await _apiService.GetArtPiecesByArtistAsync(artist);
                    count += await _cacheService.InsertOrUpdateArtPiecesAsync(artpieces??[]);
                }
                catch (Exception ex){
                    _logger.LogError(ex, "Could not retreive artpieces for  from db, aborting sync");
                    continue;
                }
            }
            return count;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromMinutes(5));
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    _logger.LogInformation("Starting background sync at {Time}", DateTimeOffset.Now);
                    await performSync();
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { }
                catch (Exception ex) { _logger.LogError(ex, "Background tick failed"); }
            }
        }
    }
}
