using MultiSourceTorrentDownloader.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace MultiSourceTorrentDownloader.Models.SubModels
{
    public partial class MainModelSettings
    {
        public bool IsLoading
        {
            get => _isLoading;
            set => this.MutateVerbose(ref _isLoading, value, RaisePropertyChanged());
        }

        public bool SaveSearchSortOrder
        {
            get => _saveSearchSortOrder;
            set => this.MutateVerbose(ref _saveSearchSortOrder, value, RaisePropertyChanged());
        }

        public int PagesToLoadBySearch
        {
            get => _pagesToLoadBySearch;
            set => this.MutateVerbose(ref _pagesToLoadBySearch, value, RaisePropertyChanged());
        }
    }
}
