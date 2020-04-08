using MultiSourceTorrentDownloader.Data;
using MultiSourceTorrentDownloader.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MultiSourceTorrentDownloader.Interfaces
{
    public interface ITorrentDataSource
    {
        Task<TorrentQueryResult> GetTorrentsAsync(string searchFor, int page, Sorting sorting);
        Task<TorrentQueryResult> GetTorrentsByCategoryAsync(string searchFor, int page, Sorting sorting, TorrentCategory category);
        Task<string> GetTorrentMagnetAsync(string detailsUri);
        Task<string> GetTorrentDescriptionAsync(string detailsUri);

        IEnumerable<string> GetSources();
        IAsyncEnumerable<SourceState> GetSourceStates();
        void UpdateUsedSource(string newBaseUrl);

        string FullTorrentUrl(string uri);
    }
}
