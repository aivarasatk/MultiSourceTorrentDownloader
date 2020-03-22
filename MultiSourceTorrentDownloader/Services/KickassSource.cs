using MultiSourceTorrentDownloader.Data;
using MultiSourceTorrentDownloader.Enums;
using MultiSourceTorrentDownloader.Interfaces;
using MultiSourceTorrentDownloader.Mapping;
using RestSharp;
using System;
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
            _searchEndpoint = Path.Combine(_baseUrl, ConfigurationManager.AppSettings["KickassSearchEndpoint"]);

            _restClient = new RestClient(_baseUrl);
            _restClient.Timeout = 10 * 1000;
        }

        public string FullTorrentUrl(string uri) => TorrentUrl(uri);

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

        private async Task<IRestResponse> HttpGetAsync(string fullUrl)
        {
            var request = new RestRequest
            {
                Method = Method.GET,
                Resource = fullUrl
            };

            return await _restClient.ExecuteAsync(request);
        }
    }
}
