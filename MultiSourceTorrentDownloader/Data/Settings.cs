using MultiSourceTorrentDownloader.Enums;
using System.Collections.Generic;

namespace MultiSourceTorrentDownloader.Data
{
    public class Settings
    {
        public Window Window { get; set; }
        public Search Search { get; set; }
        public AutoComplete AutoComplete { get; set; }
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
        public IDictionary<TorrentSource, string> SelectedSources { get; set; }
        public bool SaveSearchOrder { get; set; }
        public Sorting SearchSortOrder { get; set; }
    }
    
    public class AutoComplete
    {
        public IEnumerable<string> Values { get; set; }
    }
}
