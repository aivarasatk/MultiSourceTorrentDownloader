using System.Collections.Generic;

namespace MultiSourceTorrentDownloader.Interfaces
{
    public interface IAutoCompleteService
    {
        IReadOnlyCollection<string> AutoCompletes { get; }

        void Init(IEnumerable<string> autoCompletes);
        void TryAddAutoCompleteEntry(string entry);
    }
}