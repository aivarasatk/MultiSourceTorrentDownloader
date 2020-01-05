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

        public string TorrentFilter
        {
            get => _torrentFilter;
            set
            {
                this.MutateVerbose(ref _torrentFilter, value, RaisePropertyChanged());
                _torrentFilterSubject.OnNext(value);
            }
        }

        private ISubject<string> _torrentFilterSubject = new Subject<string>();

        public ISubject<string> TorrentFilterObservable
        {
            get => _torrentFilterSubject;
            set
            {
                if (value != _torrentFilterSubject)
                    _torrentFilterSubject = value;
            }
        }

        public string StatusBarMessage
        {
            get => _statusBarMessage;
            set
            {
                this.MutateVerbose(ref _statusBarMessage, value, RaisePropertyChanged());
                _statusBarMessageSubject.OnNext(value);
            }
        }

        private ISubject<string> _statusBarMessageSubject = new Subject<string>();

        public ISubject<string> StatusBarMessageObservable
        {
            get => _statusBarMessageSubject;
            set
            {
                if (value != _statusBarMessageSubject)
                    _statusBarMessageSubject = value;
            }
        }

        public MessageType MessageType
        {
            get => _messageType;
            set => this.MutateVerbose(ref _messageType, value, RaisePropertyChanged());
        }

        public IEnumerable<KeyValuePair<Sorting, string>> AvailableSortOrders
        {
            get => _availableSortOrders;
            set => this.MutateVerbose(ref _availableSortOrders, value, RaisePropertyChanged());
        }

        private ISubject<KeyValuePair<Sorting, string>> _selectedSearchSortOrderSubject = new Subject<KeyValuePair<Sorting, string>>();

        public ISubject<KeyValuePair<Sorting, string>> SelectedSearchSortOrderObservable
        {
            get => _selectedSearchSortOrderSubject;
            set
            {
                if (value != _selectedSearchSortOrderSubject)
                    _selectedSearchSortOrderSubject = value;
            }
        }

        public KeyValuePair<Sorting, string> SelectedSearchSortOrder
        {
            get => _selectedSearchSortOrder;
            set
            {
                this.MutateVerbose(ref _selectedSearchSortOrder, value, RaisePropertyChanged());
                _selectedSearchSortOrderSubject.OnNext(value);
            }
        }

        public TorrentEntry SelectedTorrent
        {
            get => _selectedTorrent;
            set => this.MutateVerbose(ref _selectedTorrent, value, RaisePropertyChanged());
        }

        public TorrentCategory SelectedTorrentCategory
        {
            get => _selectedTorrentCategory;
            set => this.MutateVerbose(ref _selectedTorrentCategory, value, RaisePropertyChanged());
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
