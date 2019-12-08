using MultiSourceTorrentDownloader.Data;
using MultiSourceTorrentDownloader.Enums;
using MultiSourceTorrentDownloader.Interfaces;
using MultiSourceTorrentDownloader.Mapping;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MultiSourceTorrentDownloader.Services
{
    public class LeetxSource : SourceBase, ILeetxSource
    {
        private readonly ILogService _logger;
        private readonly ILeetxParser _parser;

        public LeetxSource(ILogService logger, ILeetxParser parser)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _parser = parser ?? throw new ArgumentNullException(nameof(parser));

            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromMilliseconds(5000);
            _baseUrl = ConfigurationManager.AppSettings["LeetxUrl"];
            _searchEndpoint = Path.Combine(_baseUrl, ConfigurationManager.AppSettings["LeetxSearchEndpoint"]);
        }

        public async Task<TorrentQueryResult> GetTorrents(string searchFor, int page, Sorting sorting)
        {
            var mapperSorting = SortingMapper.SortingToLeetxSorting(sorting);

            var fullUrl = Path.Combine(_searchEndpoint, searchFor, mapperSorting.SortedBy, mapperSorting.Order, page.ToString()) + Path.DirectorySeparatorChar;
            var response = await _httpClient.GetAsync(fullUrl);
            response.EnsureSuccessStatusCode();

            var contents = await response.Content.ReadAsStringAsync();

            return await _parser.ParsePageForTorrentEntries(contents);
        }

        public async Task<string> GetTorrentMagnet(string detailsUri)
        {
            var fullUrl = Path.Combine(_baseUrl, detailsUri);
            var response = await _httpClient.GetAsync(fullUrl);
            response.EnsureSuccessStatusCode();

            var contents = await response.Content.ReadAsStringAsync();

            return await _parser.ParsePageForMagnet(contents);
        }
    }
}
