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

        private readonly ILogService _logService;

        private readonly string _windowSettingsPath;
        private readonly string _searchSettingsPath;
        private readonly string _autoCompleteSettingsPath;

        public UserConfiguration(ILogService logService)
        {
            _logService = logService ?? throw new ArgumentNullException(nameof(LogService));

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

            _autoCompleteSettingsPath = Path.Combine(currAppDir, "autoCompleteSettings.json");
            if (!File.Exists(_autoCompleteSettingsPath))
            {
                using var autoCompleteFile = File.Create(_autoCompleteSettingsPath);
            }
        }

        public Settings GetConfiguration()
        {
            if (_settingsSingleton != null)
                return _settingsSingleton;
                        
            var windowSettings = ReadSettings<Window>(_windowSettingsPath);
            var searchSettings = ReadSettings<Search>(_searchSettingsPath);

            var autoCompleteSettings = ReadSettings<AutoComplete>(_autoCompleteSettingsPath);
            autoCompleteSettings.Values ??= Enumerable.Empty<string>();


            _settingsSingleton = new Settings
            {
                Window = windowSettings,
                Search = searchSettings,
                AutoComplete = autoCompleteSettings
            };

            return _settingsSingleton;

        }

        private T ReadSettings<T>(string filePath) where T: class
        {
            try
            {
                var fileData = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<T>(fileData);
            }
            catch (Exception ex)
            {
                _logService.Information($"{typeof(T)} settings parse exception", ex);
                return null;
            }
        }

        public void SaveSettings<T>(T settings) where T: class
        {
            try
            {
                var res = JsonConvert.SerializeObject(settings, Formatting.Indented);

                var path = settings switch
                {
                    Search => _searchSettingsPath,
                    Window => _windowSettingsPath,
                    AutoComplete => _autoCompleteSettingsPath,
                    _ => throw new NotImplementedException("Settings type not found"),
                };

                File.WriteAllText(path, res);
            }
            catch (Exception ex)
            {
                _logService.Information($"{typeof(T)} settings save exception", ex);
            }
        }
    }
}
