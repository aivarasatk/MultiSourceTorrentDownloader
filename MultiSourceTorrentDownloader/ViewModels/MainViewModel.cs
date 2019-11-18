using MultiSourceTorrentDownloader.Common;
using MultiSourceTorrentDownloader.Enums;
using MultiSourceTorrentDownloader.Interfaces;
using MultiSourceTorrentDownloader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace MultiSourceTorrentDownloader.ViewModels
{
    public class MainViewModel
    {
        private readonly IThePirateBaySource _thePirateBaySource;
        private readonly ILogService _logger;
        public MainModel Model { get; private set; }

        public MainViewModel(IThePirateBaySource thePirateBaySource, ILogService logger)
        {
            Model = new MainModel();

            _thePirateBaySource = thePirateBaySource;
            _logger = logger;

            InitializeViewModel();
        }

        private void InitializeViewModel()
        {
            Model.Filters = ThePirateBayFilters();
            Model.SelectedFilter = Model.Filters.First();
            Model.SearchCommand = new Command(OnSearch, CanExecuteSearch);

        }

        private async void OnSearch(object obj)
        {
            Model.IsLoading = true;
            Model.TorrentEntries.Clear();

            try
            {
                var pirateEntries = await _thePirateBaySource.GetTorrents(Model.SearchValue, 0, Model.SelectedFilter.Key);

                foreach (var entry in pirateEntries)
                {
                    Model.TorrentEntries.Add(entry);
                }
            }
            catch(Exception ex)
            {
                _logger.Warning("Could not complete torrent search", ex);

                MessageBox.Show(ex.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Error);// REPLACE WITH SOMETHING CLEANER
            }
            
            Model.IsLoading = false;
        }

        private bool CanExecuteSearch(object obj) => !string.IsNullOrEmpty(Model.SearchValue);

        private IEnumerable<KeyValuePair<ThePirateBayFilter, string>> ThePirateBayFilters()
        {
            return new List<KeyValuePair<ThePirateBayFilter, string>>
            {
                new KeyValuePair<ThePirateBayFilter, string>(ThePirateBayFilter.SeedersDesc, "Seeders Desc. (Recommended)"),
                new KeyValuePair<ThePirateBayFilter, string>(ThePirateBayFilter.SeedersAsc, "Seeders Asc."),
                new KeyValuePair<ThePirateBayFilter, string>(ThePirateBayFilter.UploadedDesc, "Uploaded Desc."),
                new KeyValuePair<ThePirateBayFilter, string>(ThePirateBayFilter.UploadedAsc, "Uploaded Asc."),
                new KeyValuePair<ThePirateBayFilter, string>(ThePirateBayFilter.SizeDesc, "Size Desc."),
                new KeyValuePair<ThePirateBayFilter, string>(ThePirateBayFilter.SizeAsc, "Size Asc."),
                new KeyValuePair<ThePirateBayFilter, string>(ThePirateBayFilter.LeechersAsc, "Leechers Asc."),
                new KeyValuePair<ThePirateBayFilter, string>(ThePirateBayFilter.LeecherssDesc, "Leechers Desc."),
            };
        }
    }
}
