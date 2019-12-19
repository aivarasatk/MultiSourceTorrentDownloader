using MultiSourceTorrentDownloader.Data;
using MultiSourceTorrentDownloader.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiSourceTorrentDownloader.Interfaces
{
    public interface ITorrentDataSource
    {
        Task<TorrentQueryResult> GetTorrentsAsync(string searchFor, int page, Sorting sorting);
        Task<string> GetTorrentMagnetAsync(string detailsUri);
        Task<string> GetTorrentDescriptionAsync(string detailsUri);
    }
}
