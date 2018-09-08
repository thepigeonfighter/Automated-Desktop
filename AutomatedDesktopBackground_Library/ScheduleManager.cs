using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundLibrary.Scheduler
{
    public static class ScheduleManager
    {
        private static DateTime BackgroundSetAt = new DateTime(2008,1,1);
        private static DateTime CollectionChangedAt = new DateTime(2008, 1, 1);
        private static TimeSpan BackgroundRefreshSettings = new TimeSpan(0, 0, 30);
        private static TimeSpan CollectionRefreshSettings = new TimeSpan(2,0,0);
        private static DateTime NextBackgroundChangeAt = new DateTime(2008, 1, 1);
        private static DateTime NextCollectionChangeAt = new DateTime(2008, 1, 1);

        public static TimeSpan CollectionRefreshSetting() { return CollectionRefreshSettings; }
        public static TimeSpan BackgroundRefreshSetting() { return BackgroundRefreshSettings; }
        public static void ChangeBackgroundRefreshSettings(TimeSpan time)
        {
            BackgroundRefreshSettings = time;
            ResetBackgroundSettings();
        }
        public static void ChangeCollectionRefreshSettings(TimeSpan time)
        {
            CollectionRefreshSettings = time;
        }
        public static void OnBackgroundChange(DateTime time)
        {
            BackgroundSetAt = time;
            NextBackgroundChangeAt = BackgroundSetAt.Add(BackgroundRefreshSettings);
        }
        public static void OnCollectionChange(DateTime time)
        {
            CollectionChangedAt = time;
            NextCollectionChangeAt = CollectionChangedAt.Add(CollectionRefreshSettings);
        }
        private static  async void ResetBackgroundSettings()
        {
            await Task.Run(()=> GlobalConfig.JobManager.StopScheduler());
            await Task.Run(()=>GlobalConfig.JobManager.UpdateBackGroundAsync());
        }

        public static TimeModel  GetReadableForm(TimeSpan time)
        {
            if(time.Days>0)
            {
                return new TimeModel() { Amount = time.Days, TimeSetting = GlobalConfig.TimeSettings.Days };
                //return days
            } else if(time.Hours>0)
            {
                return new TimeModel() { Amount = time.Hours, TimeSetting = GlobalConfig.TimeSettings.Hours };
                // return hour format
            }
            else
            {
                return new TimeModel() { Amount = time.Minutes, TimeSetting = GlobalConfig.TimeSettings.Minutes };
                //return minute format
            }
            
              
            
        }

    }
}
