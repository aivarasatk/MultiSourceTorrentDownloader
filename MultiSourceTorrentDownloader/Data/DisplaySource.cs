using MultiSourceTorrentDownloader.Enums;
using System.Reactive.Subjects;

namespace MultiSourceTorrentDownloader.Data
{
    public class DisplaySource
    {
        private bool _selected;
        public bool Selected 
        {
            get => _selected;
            set
            {
                _selected = value;
                _selectedSubject.OnNext(value);
            }
        }

        private ISubject<bool> _selectedSubject = new Subject<bool>();

        public ISubject<bool> SelectedObservable
        {
            get => _selectedSubject;
            set
            {
                if (value != _selectedSubject)
                    _selectedSubject = value;
            }
        }

        public string SourceName { get; set; }
        public TorrentSource Source { get; set; }
    }
}
