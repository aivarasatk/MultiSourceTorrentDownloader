using MultiSourceTorrentDownloader.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace MultiSourceTorrentDownloader.Services
{
    public class LogService : ILogService
    {
        private ILogger _logger;

        public LogService()
        {
            _logger = new LoggerConfiguration()
                .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();
        }

        public void Information(string message) => _logger.Information(message);

        public void Information(string message, Exception ex) => _logger.Information(ex, message);

        public void Error(string message) => _logger.Error(message);

        public void Error(string message, Exception ex) => _logger.Error(ex, message);

    }
}
