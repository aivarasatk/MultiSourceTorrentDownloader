using MultiSourceTorrentDownloader.Common;
using MultiSourceTorrentDownloader.Data;
using System;

namespace MultiSourceTorrentDownloader.Models
{
    public partial class TorrentInfoDialogModel
    {
        private bool _isLoading;

        private string _title;
        private DateTime _date;
        private SizeEntity _size;
        private string _uploader;
        private int _seeders;
        private int _leechers;
        private string _torrentMagnet;
        private string _description;

        public Command DownloadTorrentCommand { get; set; }
    }
}
