using System.Collections.Generic;

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
