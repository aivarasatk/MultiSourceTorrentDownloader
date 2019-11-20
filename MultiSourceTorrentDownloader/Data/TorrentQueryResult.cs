using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiSourceTorrentDownloader.Data
{
    public class TorrentQueryResult
    {
        public TorrentQueryResult()
        {
            TorrentEntries = new List<TorrentEntry>();
        }
        public IEnumerable<TorrentEntry> TorrentEntries { get; set; }
        public bool LastPage { get; set; }
    }
}
