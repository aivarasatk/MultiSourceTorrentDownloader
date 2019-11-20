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
        Task<TorrentQueryResult> GetTorrents(string searchFor, ThePirateBayFilter filterOption, int page = 0);
    }
}
