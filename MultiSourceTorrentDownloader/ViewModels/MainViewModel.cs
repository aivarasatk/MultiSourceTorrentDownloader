using MaterialDesignThemes.Wpf;
using MultiSourceTorrentDownloader.Common;
using MultiSourceTorrentDownloader.Data;
using MultiSourceTorrentDownloader.Enums;
using MultiSourceTorrentDownloader.Interfaces;
using MultiSourceTorrentDownloader.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MultiSourceTorrentDownloader.ViewModels
{
    public class MainViewModel
    {
        private readonly ILogService _logger;
        private readonly ILeetxSource _leetxSource;

        private string _loadMoreString = string.Empty;
        private Dictionary<TorrentSource, SourceInformation> _torrentSourceDictionary;

        public MainModel Model { get; private set; }


        public MainViewModel(IThePirateBaySource thePirateBaySource, ILogService logger, ILeetxSource leetxSource)
        {
            _torrentSourceDictionary = new Dictionary<TorrentSource, SourceInformation>();

            Model = new MainModel();
            Model.AvailableSources = new ObservableCollection<DisplaySource>();

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _leetxSource = leetxSource ?? throw new ArgumentNullException(nameof(leetxSource));

            AddTorrentSource(TorrentSource.ThePirateBay, thePirateBaySource, startPage: 0, sourceName: "The Pirate Bay");
            AddTorrentSource(TorrentSource.Leetx, leetxSource, startPage: 1, sourceName: "1337X");

            InitializeViewModel();
        }

        private void InitializeViewModel()
        {
            Model.Filters = ThePirateBayFilters();
            Model.SelectedFilter = Model.Filters.First();
            Model.MessageQueue = new SnackbarMessageQueue();
            Model.SearchCommand = new Command(OnSearch, CanExecuteSearch);
            Model.LoadMoreCommand = new Command(OnLoadMore, CanLoadMore);

            Model.SelectedTorrentObservable.Subscribe(OnTorrentSeleceted);

        }

        private async void OnTorrentSeleceted(object obj)
        {
            if (Model.SelectedTorrent == null) return;
            Model.IsLoading = true;

            try
            {
                if (!string.IsNullOrEmpty(Model.SelectedTorrent.TorrentMagnet))
                {
                    Process.Start(Model.SelectedTorrent.TorrentMagnet);
                }
                else
                {
                    var magnetLink = await GetMagnetLinkFromTorrentEntry(Model.SelectedTorrent);
                    Process.Start(magnetLink);
                }
            }
            catch(Exception ex)
            {
                _logger.Warning($"Failed to get magnet from selected torrent uri: {Model.SelectedTorrent.TorrentUri}", ex);
                ShowStatusBarMessage(MessageType.Error, "Could not load magnet from torrent link");
            }

            Model.IsLoading = false;
        }

        private async Task<string> GetMagnetLinkFromTorrentEntry(TorrentEntry selectedTorrent)
        {
            switch (selectedTorrent.Source)
            {
                case TorrentSource.Leetx:
                    return await _leetxSource.GetTorrentMagnet(selectedTorrent.TorrentUri);
                default:
                    throw new Exception("Source not defined for getting magnet link");
            }        
        }

        private void AddTorrentSource(TorrentSource source, ITorrentDataSource dataSource, int startPage, string sourceName)
        {
            Model.AvailableSources.Add(new DisplaySource
            {
                Selected = true,
                SourceName = sourceName,
                Source = source,
            });

            _torrentSourceDictionary.Add(source, new SourceInformation
            {
                DataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource)),
                CurrentPage = startPage,
                StartPage = startPage,
                LastPage = false
            });
        }

        private async void OnLoadMore(object obj)
        {
            Model.IsLoading = true;
            Model.SearchValue = _loadMoreString;
            await LoadSourceData();
            Model.IsLoading = false;
        }

        private bool CanLoadMore(object obj)
        {
           return !string.IsNullOrEmpty(_loadMoreString) 
                && Model.TorrentEntries.Count != 0 
                && Model.AvailableSources.Any(s => s.Selected && !_torrentSourceDictionary[s.Source].LastPage);
        }

        private async void OnSearch(object obj)
        {
            _loadMoreString = Model.SearchValue;

            Model.IsLoading = true;

            Model.TorrentEntries.Clear();
            ResetPagings();

            await LoadSourceData();
            
            Model.IsLoading = false;
        }

        private void ResetPagings()
        {
            foreach (var source in Model.AvailableSources)
            {
                var sourceInfo = _torrentSourceDictionary[source.Source];
                sourceInfo.CurrentPage = sourceInfo.StartPage;
                sourceInfo.LastPage = false;
            }
        }

        private async Task LoadSourceData()
        {
            try
            {
                foreach(var source in Model.AvailableSources)
                {
                    var sourceInfo = _torrentSourceDictionary[source.Source];
                    if (!source.Selected) continue;

                    var isLastPage = await LoadFromTorrentSource(sourceInfo.DataSource, sourceInfo.CurrentPage);
                    if (isLastPage)
                        sourceInfo.LastPage = true;
                    else
                        sourceInfo.CurrentPage++;
                }

                ShowStatusBarMessage(MessageType.Information, $"Loaded {Model.TorrentEntries.Count} torrents");
                if (Model.AvailableSources.Where(s => s.Selected).All(src => _torrentSourceDictionary[src.Source].LastPage))
                    ShowStatusBarMessage(MessageType.Information, "No more records to load");

                Model.LoadMoreCommand.RaiseCanExecuteChanged();//BETTRE PLACE FOR THIS?
            }
            catch (Exception ex)
            {
                _logger.Warning("Could not complete torrent search", ex);
                ShowStatusBarMessage(MessageType.Error, $"Could not complete torrent search: {ex.Message}");
            }
        }

        private async Task<bool> LoadFromTorrentSource(ITorrentDataSource source, int currentPage)
        {
            var pirateResult = await source.GetTorrents(Model.SearchValue, currentPage, Model.SelectedFilter.Key);

            foreach (var entry in pirateResult.TorrentEntries)
                Model.TorrentEntries.Add(entry);

            return pirateResult.LastPage;
        }

        private bool CanExecuteSearch(object obj)
        {
            return !string.IsNullOrEmpty(Model.SearchValue) && Model.AvailableSources.Any(s => s.Selected);
        }

        private IEnumerable<KeyValuePair<Sorting, string>> ThePirateBayFilters()
        {
            return new List<KeyValuePair<Sorting, string>>
            {
                new KeyValuePair<Sorting, string>(Sorting.SeedersDesc, "Seeders Desc. (Recommended)"),
                new KeyValuePair<Sorting, string>(Sorting.SeedersAsc, "Seeders Asc."),
                new KeyValuePair<Sorting, string>(Sorting.UploadedDesc, "Uploaded Desc."),
                new KeyValuePair<Sorting, string>(Sorting.UploadedAsc, "Uploaded Asc."),
                new KeyValuePair<Sorting, string>(Sorting.SizeDesc, "Size Desc."),
                new KeyValuePair<Sorting, string>(Sorting.SizeAsc, "Size Asc."),
                new KeyValuePair<Sorting, string>(Sorting.LeechersAsc, "Leechers Asc."),
                new KeyValuePair<Sorting, string>(Sorting.LeecherssDesc, "Leechers Desc."),
            };
        }

        private void ShowStatusBarMessage(MessageType messageType, string message)
        {
            Model.MessageType = messageType;
            Model.MessageQueue.Enqueue(message, true);
        }
    }
}
