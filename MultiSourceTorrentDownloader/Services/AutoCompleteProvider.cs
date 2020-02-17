using AutoCompleteTextBox.Editors;
using MultiSourceTorrentDownloader.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiSourceTorrentDownloader.Services
{
    public class AutoCompleteProvider : IAutoCompleteProvider
    {
        public IEnumerable<string> Values { get; set; }

        public IEnumerable GetSuggestions(string filter)
        {
            return string.IsNullOrWhiteSpace(filter) 
                ? null 
                : Values?.Where(val => val.StartsWith(filter, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
