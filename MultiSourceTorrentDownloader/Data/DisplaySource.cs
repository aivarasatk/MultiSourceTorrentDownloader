using MultiSourceTorrentDownloader.Enums;
using MultiSourceTorrentDownloader.Models;
using System.Collections.ObjectModel;
using System.Reactive.Subjects;

namespace MultiSourceTorrentDownloader.Data
{
    public class DisplaySource : ModelBase
    {
        public DisplaySource()
        {
            SourceStates = new ObservableCollection<SourceStateUI>();
        }

        private bool _selected;

        public bool Selected 
        {
            get => _selected;
            set
            {
                this.MutateVerbose(ref _selected, value, RaisePropertyChanged());
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

        public ObservableCollection<SourceStateUI> SourceStates { get; set; }
    }
}
