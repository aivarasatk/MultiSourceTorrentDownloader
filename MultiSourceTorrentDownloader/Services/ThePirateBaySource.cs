using MultiSourceTorrentDownloader.Data;
using MultiSourceTorrentDownloader.Enums;
using MultiSourceTorrentDownloader.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace MultiSourceTorrentDownloader.Services
{
    public class ThePirateBaySource : IThePirateBaySource
    {
        private readonly ILogService _logger;
        private readonly IThePirateBayParser _parser;

        private HttpClient _httpClient;
        private string _baseUrl;
        private string _searchEndpoint;

        public ThePirateBaySource(ILogService logger, IThePirateBayParser parser)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _parser = parser ?? throw new ArgumentNullException(nameof(parser));

            _httpClient = new HttpClient();
            _baseUrl = ConfigurationManager.AppSettings["ThePirateBayUrl"];
            _searchEndpoint = Path.Combine(_baseUrl, ConfigurationManager.AppSettings["ThePirateBaySearchEndpoint"]);
        }

        public async Task<TorrentQueryResult> GetTorrents(string searchFor, ThePirateBayFilter filterOption, int page = 0)
        {
            var fullUrl = Path.Combine(_searchEndpoint, searchFor, page.ToString(), filterOption.ToString("D"));
            var response = await _httpClient.GetAsync(fullUrl);
            response.EnsureSuccessStatusCode();

            var contents = await response.Content.ReadAsStringAsync();

            return await _parser.ParsePageForTorrentEntries(contents);
        }

    }
}
