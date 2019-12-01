using MultiSourceTorrentDownloader.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
