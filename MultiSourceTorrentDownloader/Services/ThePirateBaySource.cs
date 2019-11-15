using MultiSourceTorrentDownloader.Interfaces;
using System;
using System.Configuration;
using System.IO;
using System.Net.Http;

namespace MultiSourceTorrentDownloader.Services
{
    public class ThePirateBaySource : ITorrentSource
    {
        private readonly ILogService _logger;

        private HttpClient _thePirateBayClient;
        private string _baseUrl;
        private string _searchEndpoint;

        public ThePirateBaySource(ILogService logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _thePirateBayClient = new HttpClient();
            _baseUrl = ConfigurationManager.AppSettings["ThePirateBayUrl"];
            _searchEndpoint = Path.Combine(_baseUrl, ConfigurationManager.AppSettings["ThePirateBaySearchEndpoint"]);
        }
    }
}
