using MultiSourceTorrentDownloader.Data;
using MultiSourceTorrentDownloader.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MultiSourceTorrentDownloader.Interfaces
{
    public interface IThePirateBaySource
    {
        Task<IEnumerable<TorrentEntry>> GetTorrents(string searchFor, int page = 0, ThePirateBayFilter filterOption = ThePirateBayFilter.SeedersDesc);
    }
}
