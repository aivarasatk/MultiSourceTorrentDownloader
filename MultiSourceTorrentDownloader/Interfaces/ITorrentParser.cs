using MultiSourceTorrentDownloader.Data;
using System.Threading.Tasks;

namespace MultiSourceTorrentDownloader.Interfaces
{
    public interface ITorrentParser
    {
        Task<TorrentQueryResult> ParsePageForTorrentEntriesAsync(string pageContents);
        Task<string> ParsePageForMagnetAsync(string pageContents);
        Task<string> ParsePageForDescriptionHtmlAsync(string pageContents);
    }
}
