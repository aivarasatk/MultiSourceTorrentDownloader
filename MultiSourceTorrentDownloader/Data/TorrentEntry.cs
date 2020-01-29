using MultiSourceTorrentDownloader.Enums;
using MultiSourceTorrentDownloader.Models;
using System;
using System.ComponentModel;

namespace MultiSourceTorrentDownloader.Data
{
    public class TorrentEntry : INotifyPropertyChanged
    {
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public SizeEntity Size { get; set; }
        public string Uploader { get; set; }
        public int Seeders { get; set; }
        public int Leechers{ get; set; }
        public string DescriptionHtml{ get; set; }

        public string TorrentUri { get; set; }
        public string TorrentMagnet { get; set; }

        public TorrentSource Source { get; set; }

        private bool _magnetDownloaded;
        public bool MagnetDownloaded 
        {
            get => _magnetDownloaded;
            set => this.MutateVerbose(ref _magnetDownloaded, value, RaisePropertyChanged());
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }
    }
}
