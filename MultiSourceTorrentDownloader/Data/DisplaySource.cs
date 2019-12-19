using MultiSourceTorrentDownloader.Enums;

namespace MultiSourceTorrentDownloader.Data
{
    public class DisplaySource
    {
        public bool Selected { get; set; }
        public string SourceName { get; set; }
        public TorrentSource Source { get; set; }
    }
}
