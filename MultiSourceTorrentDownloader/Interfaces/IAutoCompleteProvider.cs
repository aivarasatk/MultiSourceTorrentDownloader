using AutoCompleteTextBox.Editors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiSourceTorrentDownloader.Interfaces
{
    public interface IAutoCompleteProvider : ISuggestionProvider
    {
        IEnumerable<string> Values { get; set; }
    }
}
