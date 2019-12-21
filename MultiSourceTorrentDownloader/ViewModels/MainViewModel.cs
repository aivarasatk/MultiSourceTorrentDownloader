using MaterialDesignThemes.Wpf;
using MultiSourceTorrentDownloader.Common;
using MultiSourceTorrentDownloader.Data;
using MultiSourceTorrentDownloader.Enums;
using MultiSourceTorrentDownloader.Interfaces;
using MultiSourceTorrentDownloader.Models;
using MultiSourceTorrentDownloader.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MultiSourceTorrentDownloader.ViewModels
{
    public class MainViewModel
    {
        private readonly ILogService _logger;
        private readonly ILeetxSource _leetxSource;
        private TorrentInfoDialogViewModel _torrentInfoDialogViewModel;

        private string _loadMoreString = string.Empty;
        private Dictionary<TorrentSource, SourceInformation> _torrentSourceDictionary;

        public MainModel Model { get; private set; }


        public MainViewModel(IThePirateBaySource thePirateBaySource, ILogService logger, ILeetxSource leetxSource,
            TorrentInfoDialogViewModel torrentInfoDialogViewModel)
        {
            _torrentSourceDictionary = new Dictionary<TorrentSource, SourceInformation>();

            Model = new MainModel();
            Model.AvailableSources = new ObservableCollection<DisplaySource>();

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _leetxSource = leetxSource ?? throw new ArgumentNullException(nameof(leetxSource));
            _torrentInfoDialogViewModel = torrentInfoDialogViewModel ?? throw new ArgumentNullException(nameof(torrentInfoDialogViewModel));

            AddTorrentSource(TorrentSource.ThePirateBay, thePirateBaySource, startPage: 0, sourceName: "The Pirate Bay");
            AddTorrentSource(TorrentSource.Leetx, leetxSource, startPage: 1, sourceName: "1337X");

            InitializeViewModel();
        }

        private void InitializeViewModel()
        {
            Model.Filters = Filters();
            Model.SelectedFilter = Model.Filters.First();
            Model.MessageQueue = new SnackbarMessageQueue();
            Model.SearchCommand = new Command(OnSearch, CanExecuteSearch);
            Model.LoadMoreCommand = new Command(OnLoadMore, CanLoadMore);
            Model.OpenTorrentInfoCommand = new Command(OnOpenTorrentInfoCommand);
        }

        private async void OnOpenTorrentInfoCommand(object obj)
        {
            if (Model.SelectedTorrent == null) return;
            Model.IsLoading = true;

            try
            {
                if (string.IsNullOrEmpty(Model.SelectedTorrent.TorrentMagnet))
                {
                    Model.SelectedTorrent.TorrentMagnet = await GetMagnetLinkFromTorrentEntry(Model.SelectedTorrent);
                }

                if (string.IsNullOrEmpty(Model.SelectedTorrent.DescriptionHtml))
                {
                    var source = _torrentSourceDictionary[Model.SelectedTorrent.Source];
                    Model.SelectedTorrent.DescriptionHtml = await source.DataSource.GetTorrentDescriptionAsync(Model.SelectedTorrent.TorrentUri);
                }
              
                await ShowDetailsDialog();
            }
            catch (Exception ex)
            {
                _logger.Warning($"Failed to get magnet from selected torrent uri: {Model.SelectedTorrent.TorrentUri}", ex);
                ShowStatusBarMessage(MessageType.Error, $"Could not load magnet from torrent link: {ex.Message}");
            }
            Model.IsLoading = false;
        }

        private async Task ShowDetailsDialog()
        {
            _torrentInfoDialogViewModel.Model.Date = Model.SelectedTorrent.Date;
            _torrentInfoDialogViewModel.Model.Seeders = Model.SelectedTorrent.Seeders;
            _torrentInfoDialogViewModel.Model.Size = Model.SelectedTorrent.Size;
            _torrentInfoDialogViewModel.Model.Description = Model.SelectedTorrent.DescriptionHtml;
            _torrentInfoDialogViewModel.Model.Leechers = Model.SelectedTorrent.Leechers;
            _torrentInfoDialogViewModel.Model.Title = Model.SelectedTorrent.Title;
            _torrentInfoDialogViewModel.Model.TorrentMagnet = Model.SelectedTorrent.TorrentMagnet;
            _torrentInfoDialogViewModel.Model.Uploader = Model.SelectedTorrent.Uploader;

            var view = new TorrentInfoDialogView
            {
                DataContext = _torrentInfoDialogViewModel.Model
            };
            await DialogHost.Show(view, "RootDialog");
        }

        private async Task<string> GetMagnetLinkFromTorrentEntry(TorrentEntry selectedTorrent)
        {
            switch (selectedTorrent.Source)
            {
                case TorrentSource.Leetx:
                    return await _leetxSource.GetTorrentMagnetAsync(selectedTorrent.TorrentUri);
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
                var errorList = (await LoadDataFromSelectedSources()).ToList();
                ReorderTorrentEntries();
                ShowDataLoadResultMessage(errorList);

                Model.LoadMoreCommand.RaiseCanExecuteChanged();//BETTRE PLACE FOR THIS?
            }
            catch (Exception ex)
            {
                _logger.Warning("Could not complete torrent search", ex);
                ShowStatusBarMessage(MessageType.Error, $"Could not complete torrent search: {ex.Message}");
            }
        }

        private void ShowDataLoadResultMessage(List<string> errorList)
        {
            if (errorList.Any())
            {
                var message = errorList.Count == 1 ? errorList.First() : "Multile source connection issues. Try again or unselect some sources.";
                ShowStatusBarMessage(MessageType.Error, message);
            }
            else if (SourcesReachedLastPage())
                ShowStatusBarMessage(MessageType.Information, "No more records to load");
            else
                ShowStatusBarMessage(MessageType.Information, $"Loaded {Model.TorrentEntries.Count} torrents");
        }

        private async Task<IEnumerable<string>> LoadDataFromSelectedSources()
        {
            var sourceAndTaskTuples = TuplesToProcess();
            
            try { await Task.WhenAll(sourceAndTaskTuples.Select(x => x.Value.Item2)); }
            catch { }//exceptions thown here do not matter

            return ProcessTaskResults(sourceAndTaskTuples);
        }

        private IDictionary<string,Tuple<SourceInformation, Task<bool>>> TuplesToProcess()
        {
            var sourceDictionary = new Dictionary<string, Tuple<SourceInformation, Task<bool>>>();
            foreach (var source in Model.AvailableSources)
            {
                if (!source.Selected) continue;

                var sourceInfo = _torrentSourceDictionary[source.Source];
                sourceDictionary.Add(source.SourceName, new Tuple<SourceInformation, Task<bool>>(sourceInfo,
                    LoadFromTorrentSource(sourceInfo.DataSource, sourceInfo.CurrentPage)));
            }
            return sourceDictionary;
        }

        private IEnumerable<string> ProcessTaskResults(IDictionary<string,Tuple<SourceInformation, Task<bool>>> sourceAndTaskTuples)
        {
            foreach (var tuple in sourceAndTaskTuples)
            {
                var value = tuple.Value;
                if (value.Item2.Status != TaskStatus.RanToCompletion)
                {
                    value.Item1.LastPage = true;
                    yield return ($"Could not load data from {tuple.Key}: {value.Item2.Exception?.Message ?? value.Item2.Status.ToString()}.");
                }
                else
                {
                    if (value.Item1.LastPage)
                        value.Item1.LastPage = true;
                    else
                        value.Item1.CurrentPage++;
                }
            }
        }

        private void ReorderTorrentEntries()
        {
            IOrderedEnumerable<TorrentEntry> reorderedList = null;
            var listToOrder = Model.TorrentEntries.ToList();//copy of elements

            switch (Model.SelectedFilter.Key)
            {
                case Sorting.TimeAsc:
                    reorderedList = listToOrder.OrderBy(e => e.Date);
                    break;
                case Sorting.TimeDesc:
                    reorderedList = listToOrder.OrderByDescending(e => e.Date);
                    break;
                case Sorting.SeedersAsc:
                    reorderedList = listToOrder.OrderBy(e => e.Seeders);
                    break;
                case Sorting.SeedersDesc:
                    reorderedList = listToOrder.OrderByDescending(e => e.Seeders);
                    break;
                case Sorting.LeechersAsc:
                    reorderedList = listToOrder.OrderBy(e => e.Leechers);
                    break;
                case Sorting.LeecherssDesc:
                    reorderedList = listToOrder.OrderByDescending(e => e.Leechers);
                    break;
                case Sorting.SizeAsc:
                    reorderedList = listToOrder.OrderBy(e => e.Size);
                    break;
                case Sorting.SizeDesc:
                    reorderedList = listToOrder.OrderByDescending(e => e.Size);
                    break;
            }

            //if sort option Size we cannot sort due to lack of parsing
            if(reorderedList != null)
            {
                Model.TorrentEntries.Clear();
                foreach (var entry in reorderedList) 
                    Model.TorrentEntries.Add(entry);
            }
        }

        private bool SourcesReachedLastPage()
        {
            return Model.AvailableSources
                        .Where(s => s.Selected)
                        .All(src => _torrentSourceDictionary[src.Source].LastPage);
        }

        private async Task<bool> LoadFromTorrentSource(ITorrentDataSource source, int currentPage)
        {
            var pirateResult = await source.GetTorrentsAsync(Model.SearchValue, currentPage, Model.SelectedFilter.Key);

            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (var entry in pirateResult.TorrentEntries)
                    Model.TorrentEntries.Add(entry);
            });

            return pirateResult.LastPage;
        }

        private bool CanExecuteSearch(object obj)
        {
            return !string.IsNullOrEmpty(Model.SearchValue) && Model.AvailableSources.Any(s => s.Selected);
        }

        private IEnumerable<KeyValuePair<Sorting, string>> Filters()
        {
            return new List<KeyValuePair<Sorting, string>>
            {
                new KeyValuePair<Sorting, string>(Sorting.SeedersDesc, "Seeders Desc. (Recommended)"),
                new KeyValuePair<Sorting, string>(Sorting.SeedersAsc, "Seeders Asc."),
                new KeyValuePair<Sorting, string>(Sorting.TimeDesc, "Time Desc."),
                new KeyValuePair<Sorting, string>(Sorting.TimeAsc, "Time Asc."),
                new KeyValuePair<Sorting, string>(Sorting.SizeDesc, "Size Desc."),
                new KeyValuePair<Sorting, string>(Sorting.SizeAsc, "Size Asc."),
                new KeyValuePair<Sorting, string>(Sorting.LeechersAsc, "Leechers Asc."),
                new KeyValuePair<Sorting, string>(Sorting.LeecherssDesc, "Leechers Desc."),
            };
        }

        private void ShowStatusBarMessage(MessageType messageType, string message)
        {
            //TODO: REPLACE WITH OLD STATUS BAR
            var test = new SnackbarMessageQueue();
            Model.MessageType = messageType;
            Model.MessageQueue.Enqueue(message, true);
        }
    }
}
