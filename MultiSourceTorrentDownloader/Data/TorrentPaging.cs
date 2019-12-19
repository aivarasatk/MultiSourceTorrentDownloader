
namespace MultiSourceTorrentDownloader.Data
{
    public class TorrentPaging
    {
        public int ThePirateBayCurrentPage { get; set; } = 0;
        public bool ThePirateBayPagingEnded { get; set; }

        public int LeetxCurrentPage { get; set; } = 1;
        public bool LeetxPagingEnded { get; set; }

        public bool AllSourcesReachedEnd()
        {
            return ThePirateBayPagingEnded && LeetxPagingEnded;
        }
    }
}
