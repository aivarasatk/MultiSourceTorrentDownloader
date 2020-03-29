using MultiSourceTorrentDownloader.Data;
using MultiSourceTorrentDownloader.Enums;
using MultiSourceTorrentDownloader.Interfaces;
using MultiSourceTorrentDownloader.Mapping;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace MultiSourceTorrentDownloader.Services
{
    public class LeetxSource : SourceBase, ILeetxSource
    {
        private readonly ILogService _logger;
        private readonly ILeetxParser _parser;

        private readonly string _categorySearchEndpoint;

        public LeetxSource(ILogService logger, ILeetxParser parser)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _parser = parser ?? throw new ArgumentNullException(nameof(parser));

            _baseUrl = ConfigurationManager.AppSettings["LeetxUrl"];
            _searchEndpoint = Path.Combine(_baseUrl, ConfigurationManager.AppSettings["LeetxSearchEndpoint"]);
            _categorySearchEndpoint = Path.Combine(_baseUrl, ConfigurationManager.AppSettings["LeetxCategorySearchEndpoint"]); 
        }

        public async Task<TorrentQueryResult> GetTorrentsAsync(string searchFor, int page, Sorting sorting)
        {
            var mapperSorting = SortingMapper.SortingToLeetxSorting(sorting);
            var fullUrl = Path.Combine(_searchEndpoint, searchFor, mapperSorting.SortedBy, mapperSorting.Order, page.ToString()) + Path.DirectorySeparatorChar;
            var contents = await UrlGetResponseString(fullUrl);
            return await _parser.ParsePageForTorrentEntriesAsync(contents);
        }

        public async Task<string> GetTorrentMagnetAsync(string detailsUri)
        {
            return await BaseGetTorrentMagnetAsync(detailsUri, _parser);
        }

        public async Task<string> GetTorrentDescriptionAsync(string detailsUri)
        {
            return await BaseGetTorrentDescriptionAsync(Path.Combine(_baseUrl, detailsUri), _parser);
        }

        public async Task<TorrentQueryResult> GetTorrentsByCategoryAsync(string searchFor, int page, Sorting sorting, TorrentCategory category)
        {
            var mapperSorting = SortingMapper.SortingToLeetxSorting(sorting);
            var mappedCategory = TorrentCategoryMapper.ToLeetxCategory(category);

            var fullUrl = Path.Combine(_categorySearchEndpoint, searchFor, mappedCategory, mapperSorting.SortedBy, mapperSorting.Order, page.ToString()) + Path.DirectorySeparatorChar;
            var contents = await UrlGetResponseString(fullUrl);
            return await _parser.ParsePageForTorrentEntriesAsync(contents);
        }

        public string FullTorrentUrl(string uri) => TorrentUrl(uri);

        public async IAsyncEnumerable<SourceState> GetSourceStates()
        {
            await foreach (var source in BaseGetSourceStates(_ => GetTorrentsAsync(searchFor: "demo", page: 1, Sorting.SeedersDesc)))
            {
                yield return source;
            }
        }
    }
}
