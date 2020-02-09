using MultiSourceTorrentDownloader.Common;
using MultiSourceTorrentDownloader.Enums;
using MultiSourceTorrentDownloader.Interfaces;
using MultiSourceTorrentDownloader.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MultiSourceTorrentDownloader.ViewModels
{
    public class TorrentInfoDialogViewModel : ViewModelBase<TorrentInfoDialogModel>
    {
        private readonly ILogService _logger;

        public TorrentInfoDialogViewModel(ILogService logger) : base(new TorrentInfoDialogModel())
        {
            _logger = logger;

            InitializeModel();
        }

        private void InitializeModel()
        {
            Model.DownloadTorrentCommand = new Command(OnDownloadTorrentCommand);
            Model.CopyTorrentLinkCommand = new Command(OnCopyTorrentLinkCommand);
        }

        private void OnCopyTorrentLinkCommand(object obj)
        {
            Clipboard.SetText(Model.TorrentLink);
            ShowStatusBarMessage(MessageType.Information, "Copied torrent link to clipboard");
        }

        private void OnDownloadTorrentCommand(object obj)
        {
            try
            {
                Model.IsLoading = true;
                Process.Start(Model.TorrentMagnet);
                Model.MagnetDownloaded = true;
                ShowStatusBarMessage(MessageType.Information, "Opening on local torrent downloading app");
            }
            catch (Exception ex)
            {
                _logger.Warning($"Failed to open magnet '{Model.TorrentMagnet}'", ex);
                ShowStatusBarMessage(MessageType.Error, $"Failed to open magnet link: {ex.Message}");
            }
            finally
            {
                Model.IsLoading = false;
            }
        }
    }
}
