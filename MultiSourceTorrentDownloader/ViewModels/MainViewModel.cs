using MultiSourceTorrentDownloader.Common;
using MultiSourceTorrentDownloader.Data;
using MultiSourceTorrentDownloader.Enums;
using MultiSourceTorrentDownloader.Interfaces;
using MultiSourceTorrentDownloader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MultiSourceTorrentDownloader.ViewModels
{
    public class MainViewModel
    {
        private readonly IThePirateBaySource _thePirateBaySource;
        private readonly ILeetxSource _leetxSource;
        private readonly ILogService _logger;

        private TorrentPaging _torrentPaging;

        public MainModel Model { get; private set; }


        public MainViewModel(IThePirateBaySource thePirateBaySource, ILogService logger, ILeetxSource leetxSource)
        {
            Model = new MainModel();
            _torrentPaging = new TorrentPaging();

            _thePirateBaySource = thePirateBaySource ?? throw new ArgumentNullException(nameof(thePirateBaySource));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _leetxSource = leetxSource ?? throw new ArgumentNullException(nameof(leetxSource));

            InitializeViewModel();
        }

        private void InitializeViewModel()
        {
            Model.ThePirateBaySourceSelected = true;
            Model.LeetxSelected = true;

            Model.Filters = ThePirateBayFilters();
            Model.SelectedFilter = Model.Filters.First();
            Model.SearchCommand = new Command(OnSearch, CanExecuteSearch);
            Model.LoadMoreCommand = new Command(OnLoadMore, CanLoadMore);
        }

        private async void OnLoadMore(object obj)
        {
            Model.IsLoading = true;
            await LoadSourceData();
            Model.IsLoading = false;
        }

        private bool CanLoadMore(object obj)
        {
            return !_torrentPaging.AllSourcesReachedEnd() && Model.TorrentEntries.Count != 0 
                && (Model.ThePirateBaySourceSelected || Model.LeetxSelected);
        }

        private async void OnSearch(object obj)
        {
            Model.IsLoading = true;

            Model.TorrentEntries.Clear();
            _torrentPaging = new TorrentPaging();

            await LoadSourceData();
            
            Model.IsLoading = false;
        }

        private async Task LoadSourceData()
        {
            try
            {
                //TODO: show info if no results found
                if (Model.ThePirateBaySourceSelected)
                {
                    var lastPage = await LoadFromTorrentSource(_thePirateBaySource, _torrentPaging.ThePirateBayCurrentPage);
                    AdjustPaging(TorrentSource.ThePirateBay, lastPage);
                }

                if (Model.LeetxSelected)
                {
                    var lastPage = await LoadFromTorrentSource(_leetxSource, _torrentPaging.LeetxCurrentPage);
                    AdjustPaging(TorrentSource.Leetx, lastPage);
                }


                if (_torrentPaging.AllSourcesReachedEnd())//TODO: current setup does not allow flexible "end" for source combinations
                    MessageBox.Show("No more records to load", "End of data", MessageBoxButton.OK, MessageBoxImage.Information);//REPLACE WITH CLEANER

                Model.LoadMoreCommand.RaiseCanExecuteChanged();//BETTRE PLACE FOR THIS?
            }
            catch (Exception ex)
            {
                _logger.Warning("Could not complete torrent search", ex);
                MessageBox.Show(ex.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Error);// REPLACE WITH SOMETHING CLEANER
            }
        }

        private async Task<bool> LoadFromTorrentSource(ITorrentDataSource source, int currentPage)
        {
            var pirateResult = await source.GetTorrents(Model.SearchValue, currentPage, Model.SelectedFilter.Key);

            foreach (var entry in pirateResult.TorrentEntries)
                Model.TorrentEntries.Add(entry);

            return pirateResult.LastPage;
        }

        private void AdjustPaging(TorrentSource source, bool isLastPage)
        {
            switch (source)
            {
                case TorrentSource.ThePirateBay:
                    if (isLastPage)
                        _torrentPaging.ThePirateBayPagingEnded = true;
                    else
                        _torrentPaging.ThePirateBayCurrentPage++;
                    break;
                case TorrentSource.Leetx:
                    if (isLastPage)
                        _torrentPaging.LeetxPagingEnded = true;
                    else
                        _torrentPaging.LeetxCurrentPage++;
                    break;
            }
        }

        private bool CanExecuteSearch(object obj)
        {
            return !string.IsNullOrEmpty(Model.SearchValue) && (Model.ThePirateBaySourceSelected || Model.LeetxSelected);
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
    }
}
