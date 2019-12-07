using MultiSourceTorrentDownloader.Data;
using MultiSourceTorrentDownloader.Enums;
using MultiSourceTorrentDownloader.Interfaces;
using MultiSourceTorrentDownloader.Mapping;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace MultiSourceTorrentDownloader.Services
{
    public class ThePirateBaySource : SourceBase, IThePirateBaySource 
    {
        private readonly ILogService _logger;
        private readonly IThePirateBayParser _parser;

        public ThePirateBaySource(ILogService logger, IThePirateBayParser parser)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _parser = parser ?? throw new ArgumentNullException(nameof(parser));

            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromMilliseconds(5000)
            };
            _baseUrl = ConfigurationManager.AppSettings["ThePirateBayUrl"];
            _searchEndpoint = Path.Combine(_baseUrl, ConfigurationManager.AppSettings["ThePirateBaySearchEndpoint"]);
        }

        public async Task<TorrentQueryResult> GetTorrents(string searchFor, int page, Sorting sorting)
        {
            var mappedSortOption = SortingMapper.SortingToThePirateBaySorting(sorting);
            var fullUrl = Path.Combine(_searchEndpoint, searchFor, page.ToString(), mappedSortOption.ToString());
            var response = await _httpClient.GetAsync(fullUrl);
            response.EnsureSuccessStatusCode();

            var contents = await response.Content.ReadAsStringAsync();

            return await _parser.ParsePageForTorrentEntries(contents);
        }

        public async Task<string> GetTorrentMagnet(string detailsUri)// TPB has magnets on search page
        {
            throw new NotImplementedException();
        }

    }
}
