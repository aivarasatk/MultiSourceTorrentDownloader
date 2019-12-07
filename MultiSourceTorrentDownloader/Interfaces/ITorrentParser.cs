using MultiSourceTorrentDownloader.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiSourceTorrentDownloader.Interfaces
{
    public interface ITorrentParser
    {
        Task<TorrentQueryResult> ParsePageForTorrentEntries(string pageContents);
        Task<string> ParsePageForMagnet(string pageContents);
    }
}
