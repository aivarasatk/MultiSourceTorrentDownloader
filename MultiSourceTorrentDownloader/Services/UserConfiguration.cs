using MultiSourceTorrentDownloader.Data;
using MultiSourceTorrentDownloader.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiSourceTorrentDownloader.Services
{
    public class UserConfiguration : IUserConfiguration
    {
        private Settings _settingsSingleton;

        private readonly string _windowSettingsPath;
        private readonly string _searchSettingsPath;

        public UserConfiguration()
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create);
            var currAppDir = Path.Combine(localAppData, System.Diagnostics.Process.GetCurrentProcess().ProcessName);
            Directory.CreateDirectory(currAppDir);

            _windowSettingsPath = Path.Combine(currAppDir, "windowSettings.json");
            if (!File.Exists(_windowSettingsPath))
            {
                using var windowsFile = File.Create(_windowSettingsPath);
            }

            _searchSettingsPath = Path.Combine(currAppDir, "searchSettings.json");
            if (!File.Exists(_searchSettingsPath))
            {
                using var searchFile = File.Create(_searchSettingsPath);
            }
        }

        public Settings GetConfiguration()
        {
            if (_settingsSingleton != null)
                return _settingsSingleton;

            var fileData = File.ReadAllText(_windowSettingsPath);
            var windowSettings = JsonConvert.DeserializeObject<Window>(fileData);

            fileData = File.ReadAllText(_searchSettingsPath);
            var searchSettings = JsonConvert.DeserializeObject<Search>(fileData);

            _settingsSingleton = new Settings
            {
                Window = windowSettings,
                Search = searchSettings
            };

            return _settingsSingleton;

        }

        public void SaveSearchSettings(Search search)
        {
            var res = JsonConvert.SerializeObject(search, Formatting.Indented);
            File.WriteAllText(_searchSettingsPath, res);
        }

        public void SaveWindowSettings(Window window)
        {
            var res = JsonConvert.SerializeObject(window, Formatting.Indented);
            File.WriteAllText(_windowSettingsPath, res);
        }
    }
}
