﻿using MultiSourceTorrentDownloader.Common;
using MultiSourceTorrentDownloader.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiSourceTorrentDownloader.Models
{
    public partial class TorrentInfoDialogModel
    {
        public bool IsLoading
        {
            get => _isLoading;
            set => this.MutateVerbose(ref _isLoading, value, RaisePropertyChanged());
        }
        public string Title
        {
            get => _title;
            set => this.MutateVerbose(ref _title, value, RaisePropertyChanged());
        }

        public DateTime Date
        {
            get => _date;
            set => this.MutateVerbose(ref _date, value, RaisePropertyChanged());
        }

        public SizeEntity Size
        {
            get => _size;
            set => this.MutateVerbose(ref _size, value, RaisePropertyChanged());
        }

        public string Uploader
        {
            get => _uploader;
            set => this.MutateVerbose(ref _uploader, value, RaisePropertyChanged());
        }

        public int Seeders
        {
            get => _seeders;
            set => this.MutateVerbose(ref _seeders, value, RaisePropertyChanged());
        }

        public int Leechers
        {
            get => _leechers;
            set => this.MutateVerbose(ref _leechers, value, RaisePropertyChanged());
        }

        public string TorrentMagnet
        {
            get => _torrentMagnet;
            set => this.MutateVerbose(ref _torrentMagnet, value, RaisePropertyChanged());
        }

        public string TorrentLink
        {
            get => _torrentLink;
            set => this.MutateVerbose(ref _torrentLink, value, RaisePropertyChanged());
        }
        public string Description
        {
            get => _description;
            set => this.MutateVerbose(ref _description, value, RaisePropertyChanged());
        }

        public bool MagnetDownloaded
        {
            get => _magnetDownloaded;
            set => this.MutateVerbose(ref _magnetDownloaded, value, RaisePropertyChanged());
        }
    }
}
