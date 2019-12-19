using MultiSourceTorrentDownloader.Data;
using System.Threading.Tasks;

namespace MultiSourceTorrentDownloader.Interfaces
{
    public interface ITorrentParser
    {
        Task<TorrentQueryResult> ParsePageForTorrentEntries(string pageContents);
        Task<string> ParsePageForMagnet(string pageContents);
        Task<string> ParsePageForDescriptionHtml(string pageContents);
    }
}
