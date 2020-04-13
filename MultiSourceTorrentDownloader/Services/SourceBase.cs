using MultiSourceTorrentDownloader.Data;
using MultiSourceTorrentDownloader.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MultiSourceTorrentDownloader.Services
{
    public abstract class SourceBase
    {
        protected HttpClient _httpClient;
        protected string _baseUrl;
        protected string _searchEndpoint;
        protected string _searchResource;

        protected IEnumerable<string> _mirrors;

        public SourceBase()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromMilliseconds(7000);

            _mirrors = new List<string>();
        }

        protected async Task<string> UrlGetResponseString(string url)
        {
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        protected string TorrentUrl(string torrentUri)
        {
            return Path.Combine(_baseUrl, torrentUri);
        }

        protected async Task<string> BaseGetTorrentDescriptionAsync(string fullUrl, ITorrentParser parser)
        {
            var contents = await UrlGetResponseString(fullUrl);
            return await parser.ParsePageForDescriptionHtmlAsync(contents);
        }

        protected async Task<string> BaseGetTorrentMagnetAsync(string detailsUri, ITorrentParser parser)
        {
            var fullUrl = Path.Combine(_baseUrl, detailsUri);
            var contents = await UrlGetResponseString(fullUrl);
            return await parser.ParsePageForMagnetAsync(contents);
        }

        protected IEnumerable<string> BaseGetSources()
        {
            var allSources = new List<string>();
            allSources.Add(_baseUrl);
            allSources.AddRange(_mirrors);
            return allSources;
        }

        protected void BaseUpdateUsedSource(string newBaseUrl)
        {
            _baseUrl = newBaseUrl;
            _searchEndpoint = Path.Combine(_baseUrl, _searchResource);
        }

        /// <summary>
        /// Checks if each sources are active or not
        /// </summary>
        /// <typeparam name="T">Type of result the func yields</typeparam>
        /// <param name="func">Function that throws an exception if source is dead</param>
        /// <returns></returns>
        protected async IAsyncEnumerable<SourceState> BaseGetSourceStates(Func<Task<TorrentQueryResult>> func)
        {
            var sources = BaseGetSources();

            foreach (var source in sources)
            {
                _searchEndpoint = Path.Combine(source, _searchResource);
                var sourceActivity = await GetSourceActivity(source, func);
                yield return new SourceState(source, sourceActivity);
            }
            _searchEndpoint = Path.Combine(_baseUrl, _searchResource);
        }

        private async Task<bool> GetSourceActivity(string source, Func<Task<TorrentQueryResult>> func)
        {
            try
            {
                var res = await func.Invoke();
                if(res.TorrentEntries.Any())
                    return true;
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }
    }
}
