using MultiSourceTorrentDownloader.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace MultiSourceTorrentDownloader.Models
{
    public class ModelBase : INotifyPropertyChanged, IDataErrorInfo
    {
        private string _statusBarMessage;
        private MessageType _messageType;

        public string StatusBarMessage
        {
            get => _statusBarMessage;
            set => this.MutateVerbose(ref _statusBarMessage, value, RaisePropertyChanged());
        }

        public MessageType MessageType
        {
            get => _messageType;
            set
            {
                this.MutateVerbose(ref _messageType, value, RaisePropertyChanged());
                _messageTypeSubject.OnNext(value);
            }
        }

        private ISubject<MessageType> _messageTypeSubject = new Subject<MessageType>();

        public ISubject<MessageType> StatusBarObservable
        {
            get => _messageTypeSubject;
            set
            {
                if (value != _messageTypeSubject)
                    _messageTypeSubject = value;
            }
        }

        public string this[string columnName] => throw new NotImplementedException();

        public string Error => throw new NotImplementedException();

        public event PropertyChangedEventHandler PropertyChanged;
        protected Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }
    }
}
