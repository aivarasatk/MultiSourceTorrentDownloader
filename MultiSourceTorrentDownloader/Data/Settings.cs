using MultiSourceTorrentDownloader.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MultiSourceTorrentDownloader.Data
{
    public class Settings
    {
        public Window Window { get; set; }
        public Search Search { get; set; }
    }

    public class Window
    {
        public double Width { get; set; }
        public double Height {get;set;}
        public double PositionLeft { get; set; }
        public double PositionTop { get; set; }
    }

    public class Search
    {
        public int PagesToLoadOnSeach { get; set; }
        public IEnumerable<TorrentSource> SelectedSources { get; set; }
        public bool SaveSearchOrder { get; set; }
        public Sorting SearchSortOrder { get; set; }
    }
}
