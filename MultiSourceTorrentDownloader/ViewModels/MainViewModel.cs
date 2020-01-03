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
using System.Windows.Threading;

namespace MultiSourceTorrentDownloader.ViewModels
{
    public class MainViewModel
    {
        private readonly ILogService _logger;
        private readonly ILeetxSource _leetxSource;
        private TorrentInfoDialogViewModel _torrentInfoDialogViewModel;

        private string _loadMoreString = string.Empty;
        private Dictionary<TorrentSource, SourceInformation> _torrentSourceDictionary;

        private List<TorrentEntry> _unfilteredTorrentEntries;

        private readonly DispatcherTimer _timer;

        public MainModel Model { get; private set; }


        public MainViewModel(IThePirateBaySource thePirateBaySource, ILogService logger, ILeetxSource leetxSource,
            TorrentInfoDialogViewModel torrentInfoDialogViewModel)
        {
            _torrentSourceDictionary = new Dictionary<TorrentSource, SourceInformation>();
            _timer = new DispatcherTimer();
            SetupStatusBarTimer();

            _unfilteredTorrentEntries = new List<TorrentEntry>();

            Model = new MainModel();
            Model.AvailableSources = new ObservableCollection<DisplaySource>();

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _leetxSource = leetxSource ?? throw new ArgumentNullException(nameof(leetxSource));
            _torrentInfoDialogViewModel = torrentInfoDialogViewModel ?? throw new ArgumentNullException(nameof(torrentInfoDialogViewModel));

            AddTorrentSource(TorrentSource.ThePirateBay, thePirateBaySource, startPage: 0, sourceName: "The Pirate Bay");
            AddTorrentSource(TorrentSource.Leetx, leetxSource, startPage: 1, sourceName: "1337X");

            InitializeViewModel();
        }

        private void SetupStatusBarTimer()
        {
            _timer.Interval = TimeSpan.FromSeconds(10);
            _timer.Tick += OnStatusBarMessageExpired;
        }

        private void InitializeViewModel()
        {
            Model.Filters = Filters();
            Model.SelectedFilter = Model.Filters.First();
            Model.SearchCommand = new Command(OnSearch, CanExecuteSearch);
            Model.LoadMoreCommand = new Command(OnLoadMore, CanLoadMore);
            Model.OpenTorrentInfoCommand = new Command(OnOpenTorrentInfoCommand);

            Model.MessageType = MessageType.Empty;
            Model.StatusBarMessageObservable.Subscribe(OnStatusBarMessageChanged);
            Model.TorrentFilterObservable.Subscribe(ApplyTorrentFilter);
        }

        private void ApplyTorrentFilter(string obj = "")
        {
            if (Model.TorrentFilter == null) return;

            var entries = _unfilteredTorrentEntries
                               .Where(entry => entry.Title.ToLower().Contains(Model.TorrentFilter.ToLower()))
                               .ToList();

            Model.TorrentEntries.Clear();
            foreach (var entry in entries)
                Model.TorrentEntries.Add(entry);

            if(_unfilteredTorrentEntries.Any())
                ShowStatusBarMessage(MessageType.Information, $"Filter yields {entries.Count}/{_unfilteredTorrentEntries.Count} torrents");
        }

        private void OnStatusBarMessageChanged(string obj)
        {
            if (string.IsNullOrEmpty(Model.StatusBarMessage)) return; // cutting the loop when status bar is reset
            RestartStatusBarTimer();
        }

        private void RestartStatusBarTimer()
        {
            _timer.Stop();
            _timer.Start();
        }

        private void OnStatusBarMessageExpired(object sender, EventArgs e)
        {
            _timer.Stop();
            Model.StatusBarMessage = string.Empty;
            Model.MessageType = MessageType.Empty;
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
                _logger.Warning($"Failed to get magnet or details from selected torrent uri: {Model.SelectedTorrent.TorrentUri}", ex);
                ShowStatusBarMessage(MessageType.Error, $"Could not details from torrent link: {ex.Message}");
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

            _unfilteredTorrentEntries.AddRange(Model.TorrentEntries.Where(e => !_unfilteredTorrentEntries.Contains(e)));
            ApplyTorrentFilter();

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
            Model.TorrentFilter = null;
            ResetPagings();

            await LoadSourceData();
            
            _unfilteredTorrentEntries = new List<TorrentEntry>(Model.TorrentEntries);
            ApplyTorrentFilter();

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
                ShowDataLoadResultMessage(errorList);

                ReorderTorrentEntries();

                Model.LoadMoreCommand.RaiseCanExecuteChanged();//BETTRE PLACE FOR THIS?
            }
            catch (Exception ex)
            {
                _logger.Warning("Could not complete torrent search", ex);
                ShowStatusBarMessage(MessageType.Error, $"Could not complete torrent search: {ex.Message}");
            }
        }

        private void ShowDataLoadResultMessage(IList<string> errorList)
        {
            if (errorList.Any())
            {
                var errorMessages = string.Join(Environment.NewLine, errorList);
                _logger.Information($"Data connention issues:{Environment.NewLine}{errorMessages}");

                var message = $"Connection issues. Messages: {string.Join("; ", errorList)}";
                ShowStatusBarMessage(MessageType.Error, message);
            }
            else if (SourcesReachedLastPage())
                ShowStatusBarMessage(MessageType.Information, "No more records to load");
            else
                ShowStatusBarMessage(MessageType.Information, $"Loaded {Model.TorrentEntries.Count} torrents");
        }

        private async Task<IEnumerable<string>> LoadDataFromSelectedSources()
        {
            var sourcesToProcess = SourcesToProcess();
            try
            {
                await Task.WhenAll(sourcesToProcess.Select(x => x.TaskToPerform));
            }
            catch { } // error will be visible in tasks exceptions

            return ProcessTaskResults(sourcesToProcess);
        }

        private IEnumerable<SourceToProcess> SourcesToProcess()
        {
            var sources = new List<SourceToProcess>();
            foreach (var source in Model.AvailableSources)
            {
                var sourceInfo = _torrentSourceDictionary[source.Source];
                if (!source.Selected || sourceInfo.LastPage) continue;
                
                sources.Add(new SourceToProcess
                {
                    SourceName = source.SourceName,
                    SourceInformation = sourceInfo,
                    TaskToPerform = LoadFromTorrentSource(sourceInfo.DataSource, sourceInfo.CurrentPage)
                });
            }
            return sources;
        }

        private IEnumerable<string> ProcessTaskResults(IEnumerable<SourceToProcess> sourceToProcess)
        {
            foreach (var task in sourceToProcess)
            {
                if (task.TaskToPerform.Status != TaskStatus.RanToCompletion)
                {
                    task.SourceInformation.LastPage = true;
                    yield return ($"Could not load data from {task.SourceName}: {task.TaskToPerform.Exception?.Message ?? task.TaskToPerform.Status.ToString()}.");
                }
                else
                {
                    if (task.TaskToPerform.Result)
                        task.SourceInformation.LastPage = true;
                    else
                        task.SourceInformation.CurrentPage++;
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

        private Task<bool> LoadFromTorrentSource(ITorrentDataSource source, int currentPage)
        {
            return Task.Run(async () =>
            {
                TorrentQueryResult pirateResult = null;
                if(Model.SelectedTorrentCategory == TorrentCategory.All)
                    pirateResult = await source.GetTorrentsAsync(Model.SearchValue, currentPage, Model.SelectedFilter.Key);
                else
                    pirateResult = await source.GetTorrentsByCategoryAsync(Model.SearchValue, currentPage, Model.SelectedFilter.Key, Model.SelectedTorrentCategory);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    foreach (var entry in pirateResult.TorrentEntries)
                        Model.TorrentEntries.Add(entry);
                });

                return pirateResult.IsLastPage;
            });
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
            Model.MessageType = messageType;
            Model.StatusBarMessage = message;
        }
    }
}
