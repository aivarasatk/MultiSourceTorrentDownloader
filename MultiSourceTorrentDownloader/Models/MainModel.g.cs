using MultiSourceTorrentDownloader.Data;
using MultiSourceTorrentDownloader.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Subjects;

namespace MultiSourceTorrentDownloader.Models
{
    public partial class MainModel : INotifyPropertyChanged, IDataErrorInfo
    {
        public bool IsLoading
        {
            get => _isLoading;
            set => this.MutateVerbose(ref _isLoading, value, RaisePropertyChanged());
        }

        public string SearchValue
        {
            get => _searchValue;
            set
            {
                this.MutateVerbose(ref _searchValue, value, RaisePropertyChanged());
                SearchCommand?.RaiseCanExecuteChanged();
                LoadMoreCommand?.RaiseCanExecuteChanged();
            }
        }

        public MessageType MessageType
        {
            get => _messageType;
            set => this.MutateVerbose(ref _messageType, value, RaisePropertyChanged());
        }

        public IEnumerable<KeyValuePair<Sorting, string>> Filters
        {
            get => _filters;
            set => this.MutateVerbose(ref _filters, value, RaisePropertyChanged());
        }

        public KeyValuePair<Sorting, string> SelectedFilter
        {
            get => _selectedFilter;
            set => this.MutateVerbose(ref _selectedFilter, value, RaisePropertyChanged());
        }

        public TorrentEntry SelectedTorrent
        {
            get => _selectedTorrent;
            set
            {
                if(value != _selectedTorrent)
                {
                    this.MutateVerbose(ref _selectedTorrent, value, RaisePropertyChanged());
                    _selectedTorrentObservable.OnNext(value);
                }
            }
        }

        private ISubject<TorrentEntry> _selectedTorrentObservable = new Subject<TorrentEntry>();

        public ISubject<TorrentEntry> SelectedTorrentObservable
        {
            get => _selectedTorrentObservable;
            set
            {
                if (value != _selectedTorrentObservable)
                    _selectedTorrentObservable = value;
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }
        public string Error => throw new NotImplementedException();

        public string this[string columnName] => throw new NotImplementedException();
    }
}
