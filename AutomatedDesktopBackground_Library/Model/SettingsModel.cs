using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AutomatedDesktopBackgroundLibrary
{
    public class SettingsModel
    {
        public TimeSpan BackgroundRefreshSetting { get; set; }
        public TimeSpan CollectionRefreshSetting { get; set; }
        public SettingsModel()
        {
            if (!File.Exists(InternalFileDirectorySystem.SettingsFile))
            {
                File.Create(InternalFileDirectorySystem.SettingsFile);
                SaveSettings(GetDefaultSettings());
            }
        }
        public SettingsModel LoadSettings()
        {
            string json = File.ReadAllText(InternalFileDirectorySystem.SettingsFile);
            SettingsModel settings = JsonConvert.DeserializeObject<SettingsModel>(json);
            return settings;
        }
        public void SaveSettings(SettingsModel settings)
        {
            string json = JsonConvert.SerializeObject(settings);
            File.WriteAllText(InternalFileDirectorySystem.SettingsFile,json);
        }
        public SettingsModel GetDefaultSettings()
        {
            SettingsModel defaultSettings = new SettingsModel()
            {
                BackgroundRefreshSetting = new TimeSpan(0, 5, 0),
                CollectionRefreshSetting = new TimeSpan(24,0,0)
            };
            return defaultSettings;

        }
    }
}
