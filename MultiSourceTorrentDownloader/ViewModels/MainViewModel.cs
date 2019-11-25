using MultiSourceTorrentDownloader.Common;
using MultiSourceTorrentDownloader.Data;
using MultiSourceTorrentDownloader.Enums;
using MultiSourceTorrentDownloader.Interfaces;
using MultiSourceTorrentDownloader.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MultiSourceTorrentDownloader.ViewModels
{
    public class MainViewModel
    {
        private readonly ILogService _logger;

        private string _loadMoreString = string.Empty;

        public MainModel Model { get; private set; }


        public MainViewModel(IThePirateBaySource thePirateBaySource, ILogService logger, ILeetxSource leetxSource)
        {
            Model = new MainModel();

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            Model.AvailableSources = new ObservableCollection<Source>
            {
                new Source
                {
                    Selected = true,
                    SourceName = "The Pirate Bay",
                    DataSource = thePirateBaySource ?? throw new ArgumentNullException(nameof(thePirateBaySource)),
                    CurrentPage = 0,
                    StartPage = 0,
                    LastPage = false
                },
                new Source
                {
                    Selected = true,
                    SourceName = "1337X",
                    DataSource = leetxSource ?? throw new ArgumentNullException(nameof(leetxSource)),
                    CurrentPage = 1,
                    StartPage = 0,
                    LastPage = false
                },
            };

            InitializeViewModel();
        }

        private void InitializeViewModel()
        {
            Model.Filters = ThePirateBayFilters();
            Model.SelectedFilter = Model.Filters.First();
            Model.SearchCommand = new Command(OnSearch, CanExecuteSearch);
            Model.LoadMoreCommand = new Command(OnLoadMore, CanLoadMore);
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
                && Model.AvailableSources.Any(s => s.Selected && !s.LastPage);
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
                source.CurrentPage = source.StartPage;
                source.LastPage = false;
            }
        }

        private async Task LoadSourceData()
        {
            try
            {
                foreach(var source in Model.AvailableSources)
                {
                    if (!source.Selected) continue;

                    var isLastPage = await LoadFromTorrentSource(source.DataSource, source.CurrentPage);
                    if (isLastPage)
                        source.LastPage = true;
                    else
                        source.CurrentPage++;
                }

                ShowStatusBarMessage(MessageType.Information, $"{Model.TorrentEntries.Count} - torrents");
                if (Model.AvailableSources.Where(s => s.Selected).All(src => src.LastPage))
                    ShowStatusBarMessage(MessageType.Information, "No more records to load");

                Model.LoadMoreCommand.RaiseCanExecuteChanged();//BETTRE PLACE FOR THIS?
            }
            catch (Exception ex)
            {
                _logger.Warning("Could not complete torrent search", ex);
                ShowStatusBarMessage(MessageType.Error, "Could not complete torrent search");
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
            Model.Message = message;
        }
    }
}
