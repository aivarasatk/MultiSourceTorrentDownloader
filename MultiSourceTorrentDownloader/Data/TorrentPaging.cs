using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiSourceTorrentDownloader.Data
{
    public class TorrentPaging
    {
        public int ThePirateBayCurrentPage { get; set; } = 0;
        public bool ThePirateBayPagingEnded { get; set; }

        public bool AllSourcesReachedEnd()
        {
            return ThePirateBayPagingEnded;
        }
    }
}
