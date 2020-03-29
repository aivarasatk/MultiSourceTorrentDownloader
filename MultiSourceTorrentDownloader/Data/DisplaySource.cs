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
            SourceStates = new ObservableCollection<SourceState>();
        }

        private bool _selected;
        private bool _isLoadingSourceStates;

        public bool IsLoadingSourceStates        
        {
            get => _isLoadingSourceStates;
            set => this.MutateVerbose(ref _isLoadingSourceStates, value, RaisePropertyChanged());
        }

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

        public ObservableCollection<SourceState> SourceStates { get; set; }
    }
}
