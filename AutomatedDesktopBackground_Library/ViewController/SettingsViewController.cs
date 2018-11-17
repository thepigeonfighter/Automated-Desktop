using AutomatedDesktopBackgroundLibrary.Scheduler;
using AutomatedDesktopBackgroundLibrary.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace AutomatedDesktopBackgroundLibrary
{
    public class SettingsViewController
    {
        private static readonly Regex _regex = new Regex("[^0-9]+"); //regex that matches disallowed text
        public bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        public bool ChangeDesktopBackground()
        {
            List<ImageModel> images = DataKeeper.GetFileSnapShot().AllImages;
            if (images.Count > 0)
            {
                BackGroundPicker backGroundPicker = new BackGroundPicker();
                backGroundPicker.PickRandomBackground(true);
                return true;
            }
            else
            {
                return false;
            }
        }

        public TimeSettings ConvertStringToTime(string time)
        {
            switch (time)
            {
                case "Days":
                    return TimeSettings.Days;

                case "Hours":
                    return TimeSettings.Hours;

                case "Minutes":
                    return TimeSettings.Minutes;

                default:
                    throw new Exception("Invalid Time input");
            }
        }

        public void ChangeBackgroundRefreshRate(string amount, TimeSettings timeSettings)
        {
            TimeSpan ts = ConvertToTimeSpan(int.Parse(amount), timeSettings);
            ScheduleManager.ChangeBackgroundRefreshSettings(ts);
        }

        public void ChangeCollectionRefreshRate(string amount, TimeSettings timeSettings)
        {
            TimeSpan ts = ConvertToTimeSpan(int.Parse(amount), timeSettings);
            ScheduleManager.ChangeCollectionRefreshSettings(ts);
        }

        private TimeSpan ConvertToTimeSpan(int num, TimeSettings ts)
        {
            switch (ts)
            {
                case TimeSettings.Days:
                    return new TimeSpan(num * 24, 0, 0);

                case TimeSettings.Hours:
                    return new TimeSpan(num, 0, 0);

                case TimeSettings.Minutes:
                    return new TimeSpan(0, num, 0);

                default:
                    throw new Exception("Invalide time setting");
            }
        }

        public void ResetApplication()
        {
            DataKeeper.ResetApplication();
        }

        public TimeModel CurrentCollectionRefreshRate()
        {
            TimeSpan ts = ScheduleManager.CollectionRefreshSetting();
            return ScheduleManager.GetReadableForm(ts);
        }

        public TimeModel CurrentBackgroundRefreshRate()
        {
            TimeSpan ts = ScheduleManager.BackgroundRefreshSetting();
            return ScheduleManager.GetReadableForm(ts);
        }

        public void ClearSettings()
        {
            if(File.Exists(InternalFileDirectorySystem.SettingsFile))
            {
                File.Delete(InternalFileDirectorySystem.SettingsFile);
            }
        }
    }
}