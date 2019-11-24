using MultiSourceTorrentDownloader.Enums;
using MultiSourceTorrentDownloader.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiSourceTorrentDownloader.Data
{
    public class Source
    {
        public ITorrentDataSource DataSource { get; set; }

        public int CurrentPage { get; set; }
        public bool LastPage { get; set; }

        public TorrentSource TorrentSource { get; set; }


    }
}
