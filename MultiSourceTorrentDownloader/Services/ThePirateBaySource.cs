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
    public class ThePirateBaySource : SourceBase, IThePirateBaySource 
    {
        private readonly ILogService _logger;
        private readonly IThePirateBayParser _parser;

        public ThePirateBaySource(ILogService logger, IThePirateBayParser parser)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _parser = parser ?? throw new ArgumentNullException(nameof(parser));

            _baseUrl = ConfigurationManager.AppSettings["ThePirateBayUrl"];

            _searchResource = ConfigurationManager.AppSettings["ThePirateBaySearchEndpoint"];
            _searchEndpoint = Path.Combine(_baseUrl, _searchResource);

            _mirrors = new[]// TODO: FROM CONFIG
            {
                "https://tpb.party/",
                "https://thepiratebay0.org/",
                "https://thepiratebay10.org/",
                "https://piratebay1.live/"
            };
        }

        public IEnumerable<string> GetSources()
        {
            return BaseGetSources();
        }

        public void UpdateUsedSource(string newBaseUrl)
        {
            BaseUpdateUsedSource(newBaseUrl);
        }
        public async Task<TorrentQueryResult> GetTorrentsAsync(string searchFor, int page, Sorting sorting)
        {
            var mappedSortOption = SortingMapper.SortingToThePirateBaySorting(sorting);
            var fullUrl = Path.Combine(_searchEndpoint, searchFor, page.ToString(), mappedSortOption.ToString(), "0");

            var contents = await UrlGetResponseString(fullUrl);

            return await _parser.ParsePageForTorrentEntriesAsync(contents);
        }

        public async Task<string> GetTorrentMagnetAsync(string detailsUri)// TPB has magnets on search page
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetTorrentDescriptionAsync(string detailsUri)
        {
            var fullUrl = detailsUri.Contains(_baseUrl) ? detailsUri : Path.Combine(_baseUrl, detailsUri);//mirrors have full url while original has it without baseUrl
            return await BaseGetTorrentDescriptionAsync(fullUrl, _parser);
        }

        public async Task<TorrentQueryResult> GetTorrentsByCategoryAsync(string searchFor, int page, Sorting sorting, TorrentCategory category)
        {
            var mappedSortOption = SortingMapper.SortingToThePirateBaySorting(sorting);
            var mappedCategorySearch = TorrentCategoryMapper.ToThePirateBayCategory(category);
            var fullUrl = Path.Combine(_searchEndpoint, searchFor, page.ToString(), mappedSortOption.ToString(), mappedCategorySearch);

            var contents = await UrlGetResponseString(fullUrl);

            return await _parser.ParsePageForTorrentEntriesAsync(contents);
        }

        public string FullTorrentUrl(string uri) => uri.Contains(_baseUrl) ? uri : TorrentUrl(uri);

        public async IAsyncEnumerable<SourceState> GetSourceStates()
        {
            await foreach (var source in BaseGetSourceStates(() => GetTorrentsAsync(searchFor: "demo", page: 1, Sorting.SeedersDesc)))
                yield return source;
        }
    }
}
