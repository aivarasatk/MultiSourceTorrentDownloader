using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MultiSourceTorrentDownloader.Services
{
    public class SourceBase
    {
        protected HttpClient _httpClient;
        protected string _baseUrl;
        protected string _searchEndpoint;
    }
}
