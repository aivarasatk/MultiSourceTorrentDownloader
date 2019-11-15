using System;
using System.Collections.Generic;
using System.Text;

namespace MultiSourceTorrentDownloader.Interfaces
{
    public interface ILogService
    {
        void Information(string message);
        void Information(string message, Exception ex);

        void Error(string message);
        void Error(string message, Exception ex);
    }
}
