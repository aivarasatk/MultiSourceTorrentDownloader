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

        public async Task<TorrentQueryResult> GetTorrentsAsync(string searchFor, int page, Sorting sorting)
        {
            var mappedSortOption = SortingMapper.SortingToThePirateBaySorting(sorting);
            var fullUrl = Path.Combine(_searchEndpoint, searchFor, page.ToString(), mappedSortOption.ToString());

            var contents = await UrlGetResponseString(fullUrl);

            return await _parser.ParsePageForTorrentEntriesAsync(contents);
        }

        public async Task<string> GetTorrentMagnetAsync(string detailsUri)// TPB has magnets on search page
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetTorrentDescriptionAsync(string detailsUri)
        {
            var fullUrl = Path.Combine(_baseUrl, detailsUri);

            var contents = await UrlGetResponseString(fullUrl);

            return await _parser.ParsePageForDescriptionHtmlAsync(contents);
        }

        public async Task<TorrentQueryResult> GetTorrentsByCategoryAsync(string searchFor, int page, Sorting sorting, TorrentCategory category)
        {
            var mappedSortOption = SortingMapper.SortingToThePirateBaySorting(sorting);
            var mappedCategorySearch = TorrentCategoryMapper.ToThePirateBayCategory(category);
            var fullUrl = Path.Combine(_searchEndpoint, searchFor, page.ToString(), mappedSortOption.ToString(), mappedCategorySearch);

            var contents = await UrlGetResponseString(fullUrl);

            return await _parser.ParsePageForTorrentEntriesAsync(contents);
        }

        public string FullTorrentUrl(string uri) => TorrentUrl(uri);
    }
}
