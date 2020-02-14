using MultiSourceTorrentDownloader.Data;
using MultiSourceTorrentDownloader.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive.Subjects;

namespace MultiSourceTorrentDownloader.Models
{
    public partial class MainModel
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
                _searchValueSubject.OnNext(value);
            }
        }

        private ISubject<string> _searchValueSubject = new Subject<string>();

        public ISubject<string> SearchValueObservable
        {
            get => _searchValueSubject;
            set
            {
                if (value != _searchValueSubject)
                    _searchValueSubject = value;
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

        public bool SaveSearchSortOrder
        {
            get => _saveSearchSortOrder;
            set => this.MutateVerbose(ref _saveSearchSortOrder, value, RaisePropertyChanged());
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

        public int PagesToLoadBySearch
        {
            get => _pagesToLoadBySearch;
            set => this.MutateVerbose(ref _pagesToLoadBySearch, value, RaisePropertyChanged());
        }

        public ObservableCollection<int> SelectablePages
        {
            get => _selectablePages;
            set => this.MutateVerbose(ref _selectablePages, value, RaisePropertyChanged());
        }
    }
}
