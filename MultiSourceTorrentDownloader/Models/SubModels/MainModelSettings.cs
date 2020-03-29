using MultiSourceTorrentDownloader.Data;
using MultiSourceTorrentDownloader.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiSourceTorrentDownloader.Models.SubModels
{
    public partial class MainModelSettings : ModelBase
    {
        public MainModelSettings()
        {
            AvailableSources = new ObservableCollection<DisplaySource>();
            SelectablePages = new ObservableCollection<int>();
        }

        public ObservableCollection<DisplaySource> AvailableSources { get; set; }
        public ObservableCollection<int> SelectablePages { get; set; }

        private int _pagesToLoadBySearch;
        private bool _saveSearchSortOrder;

    }
}
