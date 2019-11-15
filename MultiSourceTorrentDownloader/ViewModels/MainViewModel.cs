using MultiSourceTorrentDownloader.Interfaces;
using MultiSourceTorrentDownloader.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MultiSourceTorrentDownloader.ViewModels
{
    public class MainViewModel
    {
        public MainModel Model { get; private set; }

        public MainViewModel(ITorrentSource source)
        {
            Model = new MainModel();
        }
    }
}
