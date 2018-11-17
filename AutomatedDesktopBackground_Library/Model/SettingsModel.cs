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
        public TimeSpan BackgroundRefreshSetting { get; set; } = new TimeSpan(0, 5, 0);
        public TimeSpan CollectionRefreshSetting { get; set; } = new TimeSpan(24, 0, 0);
        public bool ShowWarning { get; set; } = true;
        public bool StartWithSettingsWindowOpen { get; set; } = false;

        public SettingsModel LoadSettings()
        {
            if (File.Exists(InternalFileDirectorySystem.SettingsFile))
            {
                string json = File.ReadAllText(InternalFileDirectorySystem.SettingsFile);
                SettingsModel settings = JsonConvert.DeserializeObject<SettingsModel>(json);
                return settings;
            }
            else
            {
                return new SettingsModel();
            }
        }
        public void SaveSettings(SettingsModel settings)
        {
            string json = JsonConvert.SerializeObject(settings);
            File.WriteAllText(InternalFileDirectorySystem.SettingsFile,json);
        }

    }
}
