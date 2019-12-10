using MultiSourceTorrentDownloader.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiSourceTorrentDownloader.Models
{
    public partial class TorrentInfoDialogModel
    {
        private string _title;
        private DateTime _date;
        private string _size;
        private string _uploader;
        private int _seeders;
        private int _leechers;
        private string _torrentMagnet;
        private string _description;


        public Command DownloadTorrentCommand { get; set; }
    }
}
