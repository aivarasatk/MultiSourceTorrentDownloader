using MultiSourceTorrentDownloader.Enums;
using MultiSourceTorrentDownloader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiSourceTorrentDownloader.ViewModels
{
    public class ViewModelBase<T> where T : ModelBase, new()
    {
        public T Model { get; private set; }

        private TimeSpan _statusBarFadeTime;
        private IDisposable _statusBarSubscription;

        public ViewModelBase()
        {
            Model = new T();
            Initialize();
        }

        private void Initialize()
        {
            _statusBarFadeTime = TimeSpan.FromSeconds(10);
            Model.MessageType = MessageType.Empty;
            Model.StatusBarObservable
                 .Where(x => x != MessageType.Empty)
                 .Subscribe(OnStatusBarMessageChanged);
        }

        private void OnStatusBarMessageChanged(MessageType obj)
        {
            _statusBarSubscription?.Dispose();
            _statusBarSubscription = Observable
                .Timer(_statusBarFadeTime)
                .Subscribe(x =>
                {
                    Model.StatusBarMessage = string.Empty;
                    Model.MessageType = MessageType.Empty;
                });

        }

        public void ShowStatusBarMessage(MessageType messageType, string message)
        {
            Model.MessageType = messageType;
            Model.StatusBarMessage = message;
        }
    }
}
