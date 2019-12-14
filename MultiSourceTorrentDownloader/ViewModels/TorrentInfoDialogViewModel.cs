using MultiSourceTorrentDownloader.Common;
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
    public class TorrentInfoDialogViewModel
    {
        private readonly ILogService _logger;
        public TorrentInfoDialogModel Model { get; set; }

        public TorrentInfoDialogViewModel(ILogService logger)
        {
            Model = new TorrentInfoDialogModel();

            _logger = logger;

            InitializeModel();
        }

        private void InitializeModel()
        {
            Model.DownloadTorrentCommand = new Command(OnDownloadTorrentCommand);
        }

        private void OnDownloadTorrentCommand(object obj)
        {

            try
            {
                Model.IsLoading = true;
                Process.Start(Model.TorrentMagnet);
            }
            catch (Exception ex)
            {
                _logger.Warning($"Failed to open magnet '{Model.TorrentMagnet}'", ex);
                MessageBox.Show($"Failed to open magnet link: {ex.Message}", "Torrent info", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Model.IsLoading = false;
            }
        }
    }
}
