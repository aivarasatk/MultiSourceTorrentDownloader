using MultiSourceTorrentDownloader.Common;
using MultiSourceTorrentDownloader.Data;
using MultiSourceTorrentDownloader.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MultiSourceTorrentDownloader.Models
{
    public partial class MainModel
    {
        public MainModel()
        {
            TorrentEntries = new ObservableCollection<TorrentEntry>();
            Filters = new List<KeyValuePair<Sorting, string>>();
            AvailableSources = new ObservableCollection<Source>();
        }

        private bool _isLoading;
        private string _searchValue;

        private MessageType _messageType;
        private string _message;

        public ObservableCollection<Source> AvailableSources { get; set; }
        public ObservableCollection<TorrentEntry> TorrentEntries { get; set; }

        private IEnumerable<KeyValuePair<Sorting, string>> _filters;
        private KeyValuePair<Sorting, string> _selectedFilter;

        public Command LoadMoreCommand { get; set; }
        public Command SearchCommand { get; set; }
    }
}
