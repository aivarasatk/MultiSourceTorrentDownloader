using MultiSourceTorrentDownloader.Common;
using MultiSourceTorrentDownloader.Data;
using System;

namespace MultiSourceTorrentDownloader.Models
{
    public partial class TorrentInfoDialogModel : ModelBase
    {
        private bool _isLoading;

        private string _title;
        private DateTime _date;
        private SizeEntity _size;
        private string _uploader;
        private int _seeders;
        private int _leechers;
        private string _torrentMagnet;
        private string _torrentLink;
        private string _description;
        private bool _magnetDownloaded;

        public Command DownloadTorrentCommand { get; set; }
        public Command CopyTorrentLinkCommand { get; set; }
    }
}
