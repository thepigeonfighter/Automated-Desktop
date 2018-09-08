using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutomatedDesktopBackgroundLibrary.Scheduler;
namespace AutomatedDesktopBackgroundLibrary
{
    public class SettingsViewController
    {
        private static readonly Regex _regex = new Regex("[^0-9]+"); //regex that matches disallowed text
       public bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }
        public void ChangeDesktopBackground()
        {
            BackGroundPicker backGroundPicker = new BackGroundPicker();
            backGroundPicker.PickRandomBackground();
        }
        public GlobalConfig.TimeSettings ConvertStringToTime(string time)
        {
            switch (time)
            {
                case "Days":
                    return GlobalConfig.TimeSettings.Days;
                case "Hours":
                    return GlobalConfig.TimeSettings.Hours;
                case "Minutes":
                    return GlobalConfig.TimeSettings.Minutes;
                default:
                    throw new Exception("Invalid Time input");

            }
        }
        public void ChangeBackgroundRefreshRate(string amount, GlobalConfig.TimeSettings timeSettings)
        {
            TimeSpan ts = ConvertToTimeSpan(int.Parse(amount), timeSettings);
            ScheduleManager.ChangeBackgroundRefreshSettings(ts);
        }
        public void ChangeCollectionRefreshRate(string amount, GlobalConfig.TimeSettings timeSettings)
        {
            TimeSpan ts = ConvertToTimeSpan(int.Parse(amount), timeSettings);
            ScheduleManager.ChangeCollectionRefreshSettings(ts);
        }
        private TimeSpan ConvertToTimeSpan(int num, GlobalConfig.TimeSettings ts)
        {
            switch(ts)
            {
                case GlobalConfig.TimeSettings.Days:
                    return new TimeSpan(num*24, 0, 0);
                case GlobalConfig.TimeSettings.Hours:
                    return new TimeSpan(num, 0, 0);
                case GlobalConfig.TimeSettings.Minutes:
                    return new TimeSpan(0, num, 0);
                default:
                    throw new Exception("Invalide time setting");

            }
        }
        public TimeModel CurrentCollectionRefreshRate()
        {
           TimeSpan ts =ScheduleManager.CollectionRefreshSetting();
            return ScheduleManager.GetReadableForm(ts);
        }
        public TimeModel CurrentBackgroundRefreshRate()
        {
            TimeSpan ts =  ScheduleManager.BackgroundRefreshSetting();
            return ScheduleManager.GetReadableForm(ts);
        }
       
    }
}
