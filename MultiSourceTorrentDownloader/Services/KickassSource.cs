using MultiSourceTorrentDownloader.Data;
using MultiSourceTorrentDownloader.Enums;
using MultiSourceTorrentDownloader.Interfaces;
using MultiSourceTorrentDownloader.Mapping;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace MultiSourceTorrentDownloader.Services
{
    public class KickassSource : SourceBase, IKickassSource
    {
        private readonly ILogService _logger;
        private readonly IKickassParser _parser;

        private RestClient _restClient;

        public KickassSource(ILogService logger, IKickassParser parser)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _parser = parser ?? throw new ArgumentNullException(nameof(parser));

            _baseUrl = ConfigurationManager.AppSettings["KickassUrl"];
            _searchResource = ConfigurationManager.AppSettings["KickassSearchEndpoint"];

            _searchEndpoint = Path.Combine(_baseUrl, _searchResource);

            _restClient = new RestClient(_baseUrl);
        }

        public string FullTorrentUrl(string uri) => TorrentUrl(uri);

        public IEnumerable<string> GetSources()
        {
            return BaseGetSources();
        }

        public async IAsyncEnumerable<SourceState> GetSourceStates()
        {
            await foreach (var source in BaseGetSourceStates(() => GetTorrentsAsync(searchFor: "demo", page: 1, Sorting.SeedersDesc)))
                yield return source;
        }

        public async Task<string> GetTorrentDescriptionAsync(string detailsUri)
        {
            var fullUrl = Path.Combine(_baseUrl, detailsUri);
            var response = await HttpGetAsync(fullUrl);

            return await _parser.ParsePageForDescriptionHtmlAsync(response.Content);
        }

        public async Task<string> GetTorrentMagnetAsync(string detailsUri)
        {
            var fullUrl = Path.Combine(_baseUrl, detailsUri);
            var response = await HttpGetAsync(fullUrl);
            return await _parser.ParsePageForMagnetAsync(response.Content);
        }

        public async Task<TorrentQueryResult> GetTorrentsAsync(string searchFor, int page, Sorting sorting)
        {
            var mappedSortOption = SortingMapper.SortingToKickassSorting(sorting);
            var fullUrl = Path.Combine(_searchEndpoint, searchFor, page.ToString(), $"?sortby={mappedSortOption.SortBy}&sort={mappedSortOption.Sort}");

            var response = await HttpGetAsync(fullUrl);

            return await _parser.ParsePageForTorrentEntriesAsync(response.Content);
        }

        public async Task<TorrentQueryResult> GetTorrentsByCategoryAsync(string searchFor, int page, Sorting sorting, TorrentCategory category)
        {
            var mappedSortOption = SortingMapper.SortingToKickassSorting(sorting);
            var mappedCategory = TorrentCategoryMapper.ToKickassCategory(category);

            var fullUrl = Path.Combine(_searchEndpoint, searchFor, $"category/{mappedCategory}", page.ToString(), $"?sortby={mappedSortOption.SortBy}&sort={mappedSortOption.Sort}");

            var response = await HttpGetAsync(fullUrl);

            return await _parser.ParsePageForTorrentEntriesAsync(response.Content);
        }

        public void UpdateUsedSource(string newBaseUrl)
        {
            BaseUpdateUsedSource(newBaseUrl);
        }

        private async Task<RestResponse> HttpGetAsync(string fullUrl)
        {
            var request = new RestRequest
            {
                Method = Method.Get,
                Resource = fullUrl,
                Timeout = 7 * 1000
            };

            return await _restClient.ExecuteAsync(request);
        }
    }
}
