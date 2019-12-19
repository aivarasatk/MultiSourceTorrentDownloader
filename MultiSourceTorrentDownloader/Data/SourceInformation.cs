using MultiSourceTorrentDownloader.Interfaces;

namespace MultiSourceTorrentDownloader.Data
{
    public class SourceInformation
    {
        public ITorrentDataSource DataSource { get; set; }
        public int CurrentPage { get; set; }
        public int StartPage { get; set; }
        public bool LastPage { get; set; }
    }
}
