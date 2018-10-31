using System;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundLibrary.Scheduler
{
    public static class ScheduleManager
    {
        //TODO set to decent times
        private static DateTime BackgroundSetAt = new DateTime(2008, 1, 1);

        private static SettingsModel _settingsModel;
        private static DateTime _collectionChangedAt = new DateTime(2008, 1, 1);
        private static TimeSpan _backgroundRefreshSettings = new TimeSpan(0, 1, 0);
        private static TimeSpan _collectionRefreshSettings = new TimeSpan(1, 0, 0);
        private static DateTime NextBackgroundChangeAt = new DateTime(2008, 1, 1);
        private static DateTime NextCollectionChangeAt = new DateTime(2008, 1, 1);
        private static bool _warnUserOnExit = true;

        static ScheduleManager()
        {
            _settingsModel = new SettingsModel();
            SettingsModel tempSettings = _settingsModel.LoadSettings();
            if(tempSettings != null) { _settingsModel = tempSettings; }
             _backgroundRefreshSettings = _settingsModel.BackgroundRefreshSetting;
            _collectionRefreshSettings = _settingsModel.CollectionRefreshSetting;
            _warnUserOnExit = _settingsModel.ShowWarning;

        }
        public static void UpdateWarningFlag(bool value)
        {
            _warnUserOnExit = value;
            _settingsModel.SaveSettings(GetCurrentSettings());
        }
        public static bool GetWarningFlagStatus()
        {
            return _warnUserOnExit;
        }
        public static TimeSpan CollectionRefreshSetting()
        {
            return _collectionRefreshSettings;
        }

        public static TimeSpan BackgroundRefreshSetting()
        {
            return _backgroundRefreshSettings;
        }

        public static void ChangeBackgroundRefreshSettings(TimeSpan time)
        {
            _backgroundRefreshSettings = time;
            _settingsModel.SaveSettings(GetCurrentSettings());
            ResetBackgroundSettings();
        }

        public static void ChangeCollectionRefreshSettings(TimeSpan time)
        {
            _collectionRefreshSettings = time;
            _settingsModel.SaveSettings(GetCurrentSettings());
            ResetCollectionSettings();
        }

        public static void OnBackgroundChange(DateTime time)
        {
            BackgroundSetAt = time;
            NextBackgroundChangeAt = BackgroundSetAt.Add(_backgroundRefreshSettings);
        }

        public static void OnCollectionChange(DateTime time)
        {
            _collectionChangedAt = time;
            NextCollectionChangeAt = _collectionChangedAt.Add(_collectionRefreshSettings);
        }

        private static async void ResetBackgroundSettings()
        {
            if (GlobalConfig.BackgroundRefreshing)
            {
                await Task.Run(() => GlobalConfig.JobManager.StopBackgroundUpdatingAsync()).ConfigureAwait(false);
                await Task.Run(() => GlobalConfig.JobManager.StartBackgroundUpdatingAsync()).ConfigureAwait(false);
            }
        }

        private static async void ResetCollectionSettings()
        {
            if (GlobalConfig.CollectionsRefreshing)
            {
                await Task.Run(() => GlobalConfig.JobManager.StopCollectionUpdatingAsync()).ConfigureAwait(false);
                await Task.Run(() => GlobalConfig.JobManager.StartCollectionUpdatingAsync()).ConfigureAwait(false);
            }
        }

        public static TimeModel GetReadableForm(TimeSpan time)
        {
            if (time.Days > 0)
            {
                return new TimeModel() { Amount = time.Days, TimeSetting = TimeSettings.Days };
                //return days
            }
            else if (time.Hours > 0)
            {
                return new TimeModel() { Amount = time.Hours, TimeSetting = TimeSettings.Hours };
                // return hour format
            }
            else
            {
                return new TimeModel() { Amount = time.Minutes, TimeSetting = TimeSettings.Minutes };
                //return minute format
            }
        }

        private static SettingsModel GetCurrentSettings()
        {
            SettingsModel currentSettings = new SettingsModel()
            {
                BackgroundRefreshSetting = _backgroundRefreshSettings,
                CollectionRefreshSetting = _collectionRefreshSettings,
                ShowWarning = _warnUserOnExit

            };
            return currentSettings;
        }
    }
}