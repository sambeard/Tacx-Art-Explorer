using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TacxArtExplorer.Models.DTO;

namespace TacxArtExplorer.Services.HTTPClients
{
    class ArticImageClient :ArticApiClient
    {
        private const string _baseUrl = "https://www.artic.edu/iiif/2/";

        public ArticImageClient(HttpClient httpClient) : base(httpClient, new Uri(_baseUrl)) { }

        public override Task<IResultDTO> GetAsync(string endpoint, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
