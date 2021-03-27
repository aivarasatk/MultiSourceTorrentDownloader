using MultiSourceTorrentDownloader.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiSourceTorrentDownloader.Services
{
    public class AutoCompleteService : IAutoCompleteService
    {
        private const int MaxEntries = 50;

        private List<string> _autoCompletes = new List<string>();

        public IReadOnlyCollection<string> AutoCompletes { get => _autoCompletes; }

        public void Init(IEnumerable<string> autoCompletes)
        {
            _autoCompletes = new List<string>(autoCompletes);
        }

        /// <summary>
        /// Adds an entry to the auto complete list if one does not already exist
        /// </summary>
        public void TryAddAutoCompleteEntry(string entry)
        {
            if (string.IsNullOrWhiteSpace(entry)
                || _autoCompletes.Any(a => a.Equals(entry, StringComparison.InvariantCultureIgnoreCase)))
                return;

            if (_autoCompletes.Count is MaxEntries)
                _autoCompletes.RemoveAt(0);

            _autoCompletes.Add(entry);
        }
    }
}
