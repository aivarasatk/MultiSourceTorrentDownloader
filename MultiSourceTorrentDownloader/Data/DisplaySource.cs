using MultiSourceTorrentDownloader.Enums;
using MultiSourceTorrentDownloader.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiSourceTorrentDownloader.Data
{
    public class DisplaySource
    {
        public bool Selected { get; set; }
        public string SourceName { get; set; }
        public TorrentSource Source { get; set; }
    }
}
