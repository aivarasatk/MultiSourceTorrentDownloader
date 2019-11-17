using MultiSourceTorrentDownloader.Interfaces;
using MultiSourceTorrentDownloader.Models;
using System;
using System.Collections.Generic;
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

        private async void InitializeViewModel()//CHNAGE TO VOID
        {
            //Model.IsLoading = true;// REMOVE
            //var torrents = await _thePirateBaySource.GetTorrents("the deuce");

        }
    }
}
