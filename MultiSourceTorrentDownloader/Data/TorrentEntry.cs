using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiSourceTorrentDownloader.Data
{
    public class TorrentEntry
    {
        public string Title { get; set; }
        public string Date { get; set; }
        public string Size { get; set; }
        public string Uploader { get; set; }
        public int Seeders { get; set; }
        public int Leechers{ get; set; }
        public string TorrentUri { get; set; }
        public string TorrentMagnet { get; set; }
    }
}
