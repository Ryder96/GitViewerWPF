using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using T3UploaderWPF.Network;
using T3UploaderWPF.UI.Data;

namespace T3UploaderWPF.Settings
{
    public class SettingsManager
    {
        // Singleton
        private static readonly Lazy<SettingsManager> _instance = new((() => new SettingsManager()));
        private readonly string _settingsFilePath = "settings.json";
        public Settings? Settings { get; private set; }
        public static SettingsManager Instance { get { return _instance.Value; } }
        private SettingsManager() { }

        public ErrorHandler ImportSettings()
        {
            var file = ReadFile();
            if (!file.Success || Settings == null)
                return file;

            if (Settings.Git == null)
            {
                return new ErrorHandler
                {
                    Success = false,
                    ErrorMessage = "Git information is null"
                };
            }
            return GitManager.Instance.Initialize(Settings.Git);
        }

        private ErrorHandler ReadFile()
        {

            if (!File.Exists(_settingsFilePath))
            {
                Settings = null;
                return new ErrorHandler
                {
                    Success = false,
                    ErrorMessage = "Settings file does not exist create it!"
                };
            }

            var json = File.ReadAllText(_settingsFilePath);


            Settings = JsonConvert.DeserializeObject<Settings>(json);
            if (Settings == null)
            {
                return new ErrorHandler
                {
                    Success = false,
                    ErrorMessage = "Failed to deserialize settings"
                };
            }

            return new ErrorHandler { Success = true };
        }

        public ErrorHandler Update(bool init)
        {
            var file = ReadFile();
            if (!file.Success || Settings == null)
                return file;

            Settings.Initialized = init;

            try
            {
                File.WriteAllText(_settingsFilePath, JsonConvert.SerializeObject(Settings, Formatting.Indented));
                return new ErrorHandler { Success = true };
            }
            catch (Exception ex)
            {
                return new ErrorHandler
                {
                    Success = false,
                    ErrorMessage = "Erron on updating settings",
                    Exception = ex,
                    ExceptionMessage = ex.Message
                };
            }
        }

    }
}
