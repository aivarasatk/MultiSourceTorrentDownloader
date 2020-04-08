using MultiSourceTorrentDownloader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace MultiSourceTorrentDownloader.Data
{
    public class SourceStateUI : DataModelBase
    {
        public SourceStateUI(string source, bool isAlive, bool selected)
        {
            SourceName = source;
            _isAlive = isAlive;
            _selected = selected;
        }

        public string SourceName { get; set; }
        public bool _isAlive;

        public bool IsAlive
        {
            get => _isAlive;
            set => this.MutateVerbose(ref _isAlive, value, RaisePropertyChanged());
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

    }
}
