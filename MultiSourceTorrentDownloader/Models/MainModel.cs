using MultiSourceTorrentDownloader.Data;
using MultiSourceTorrentDownloader.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace MultiSourceTorrentDownloader.Models
{
    public partial class MainModel
    {
        public MainModel()
        {
            TorrentEntries = new ObservableCollection<TorrentEntry>();
            Filters = new List<KeyValuePair<ThePirateBayFilter, string>>();
        }

        private bool _isLoading;
        private string _searchValue;
        public ObservableCollection<TorrentEntry> TorrentEntries { get; set; }
        private IEnumerable<KeyValuePair<ThePirateBayFilter, string>> _filters;
        private KeyValuePair<ThePirateBayFilter, string> _selectedFilter;

        public ICommand SearchCommand { get; set; }
    }
}
