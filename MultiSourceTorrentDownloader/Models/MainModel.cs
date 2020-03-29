using MaterialDesignThemes.Wpf;
using MultiSourceTorrentDownloader.Common;
using MultiSourceTorrentDownloader.Data;
using MultiSourceTorrentDownloader.Enums;
using MultiSourceTorrentDownloader.Models.SubModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MultiSourceTorrentDownloader.Models
{
    public partial class MainModel : ModelBase
    {
        public MainModel()
        {
            TorrentEntries = new ObservableCollection<TorrentEntry>();
            AvailableSortOrders = new List<KeyValuePair<Sorting, string>>();
            Settings = new MainModelSettings();
        }

        private MainModelSettings _settings;

        private bool _isLoading;
        private string _searchValue;
        private string _torrentFilter;      

        public ObservableCollection<TorrentEntry> TorrentEntries { get; set; }

        private KeyValuePair<Sorting, string> _selectedSearchSortOrder;
        private IEnumerable<KeyValuePair<Sorting, string>> _availableSortOrders;

        private TorrentEntry _selectedTorrent;
        private TorrentCategory _selectedTorrentCategory;

        public Command LoadMoreCommand { get; set; }
        public Command SearchCommand { get; set; }
        public Command OpenTorrentInfoCommand { get; set; }
        public Command DownloadMagnetCommand { get; set; }
        public Command CopyTorrentLinkCommand { get; set; }
    }
}
