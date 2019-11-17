using MultiSourceTorrentDownloader.Enums;
using MultiSourceTorrentDownloader.Interfaces;
using MultiSourceTorrentDownloader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiSourceTorrentDownloader.ViewModels
{
    public class MainViewModel
    {
        private readonly IThePirateBaySource _thePirateBaySource;
        public MainModel Model { get; private set; }

        public MainViewModel(IThePirateBaySource thePirateBaySource)
        {
            Model = new MainModel();

            _thePirateBaySource = thePirateBaySource;

            InitializeViewModel();
        }

        private void InitializeViewModel()
        {
            Model.Filters = ThePirateBayFilters();
            Model.SelectedFilter = Model.Filters.First();
            //var torrents = await _thePirateBaySource.GetTorrents("the deuce");

        }

        private IEnumerable<KeyValuePair<ThePirateBayFilter, string>> ThePirateBayFilters()
        {
            return new List<KeyValuePair<ThePirateBayFilter, string>>
            {
                new KeyValuePair<ThePirateBayFilter, string>(ThePirateBayFilter.SeedersDesc, "Seeders Desc."),
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
