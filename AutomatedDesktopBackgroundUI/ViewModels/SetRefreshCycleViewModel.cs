using AutomatedDesktopBackgroundUI.Models;
using AutomatedDesktopBackgroundUI.SessionData;
using AutomatedDesktopBackgroundUI.Utility;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AutomatedDesktopBackgroundUI.ViewModels
{
    public class SetRefreshCycleViewModel:Screen, ISettingScreen
    {
        #region Properties
        public BindableCollection<TimeModel> BackgroundTime { get; set; }
        public BindableCollection<TimeModel> CollectionTime { get; set; }

        public TimeModel SelectedCollectionTime
        {
            get { return _selectedCollectionTime; }
            set
            {
                if (value != SelectedBackgroundTime)
                {
                    TimeModel selectedTime = CollectionTime.FirstOrDefault(x => x.Time == value.Time);
                    if (selectedTime != null)
                    {
                        SelectedCollectionTimeIndex = CollectionTime.IndexOf(selectedTime);
                    }
                    _selectedCollectionTime = value;
                    NotifyOfPropertyChange(() => SelectedCollectionTime);
                }
            }
        }
        public int SelectedCollectionTimeIndex
        {
            get { return _selectedCollectionTimeIndex; }
            set
            {
                _selectedCollectionTimeIndex = value;
                NotifyOfPropertyChange(() => SelectedCollectionTimeIndex);
            }
        }
        

        public TimeModel SelectedBackgroundTime
        {
            get { return _selectedBackgroundTime; }
            set
            {
                if (value != SelectedBackgroundTime)
                {
                    TimeModel selectedTime = BackgroundTime.FirstOrDefault(x => x.Time == value.Time);
                    if (selectedTime != null)
                    {
                        SelectedBackgroundTimeIndex = BackgroundTime.IndexOf(selectedTime);
                    }
                    _selectedBackgroundTime = value;
                    NotifyOfPropertyChange(() => SelectedBackgroundTime);
                }
            }
        }


        public int SelectedBackgroundTimeIndex
        {
            get { return _selectedBackgroundTimeIndex; }
            set
            {
                _selectedBackgroundTimeIndex = value;
                NotifyOfPropertyChange(() => SelectedBackgroundTimeIndex);
            }
        }
        #endregion

        #region Backing Vars
        private TimeModel _selectedBackgroundTime = new TimeModel();

        private int _selectedCollectionTimeIndex;

        private int _selectedBackgroundTimeIndex;

        private SettingsModel _currentSettings;

        private TimeModel _selectedCollectionTime = new TimeModel();
        #endregion

        #region CTOR

        public SetRefreshCycleViewModel( SettingsModel currentSettings)
        {
            _currentSettings = currentSettings;
            SelectedBackgroundTime.Time = currentSettings.BackgroundRefreshTime;
            SelectedCollectionTime.Time = currentSettings.CollectionRefreshTime;
            Initialize();
        }
        #endregion
        private void Initialize()
        {

            BackgroundTime = GetSampleBackgroundTimes();
            CollectionTime = GetSampleCollectionTimes();
            TimeModel currentBackgroundRefresh = new TimeModel()
            {
                Time = _currentSettings.BackgroundRefreshTime
            };
            TimeModel currentCollectionRefresh = new TimeModel()
            {
                Time = _currentSettings.CollectionRefreshTime
            };
            SelectedBackgroundTime = currentBackgroundRefresh;
            SelectedCollectionTime = currentCollectionRefresh;


        }
        private BindableCollection<TimeModel> GetSampleCollectionTimes()
        {
            BindableCollection<TimeModel> times = new BindableCollection<TimeModel>();
            times.Add(new TimeModel() { Time = new TimeSpan(6, 0, 0) });
            times.Add(new TimeModel() { Time = new TimeSpan(12, 0, 0) });
            times.Add(new TimeModel() { Time = new TimeSpan(24, 0, 0) });
            times.Add(new TimeModel() { Time = new TimeSpan(48, 0, 0) });
            times.Add(new TimeModel() { Time = new TimeSpan(96, 0, 0) });
            times.Add(new TimeModel() { Time = new TimeSpan(192, 0, 0) });
            times.Add(new TimeModel() { Time = new TimeSpan(384, 0, 0) });
            return times;
        }
        private BindableCollection<TimeModel> GetSampleBackgroundTimes()
        {
            BindableCollection<TimeModel> times = new BindableCollection<TimeModel>();
            times.Add(new TimeModel() { Time = new TimeSpan(0, 0, 30) });
            times.Add(new TimeModel() { Time = new TimeSpan(0, 1, 0) });
            times.Add(new TimeModel() { Time = new TimeSpan(0, 5, 0) });
            times.Add(new TimeModel() { Time = new TimeSpan(0, 10, 0) });
            times.Add(new TimeModel() { Time = new TimeSpan(0, 15, 0) });
            times.Add(new TimeModel() { Time = new TimeSpan(0, 30, 0) });
            times.Add(new TimeModel() { Time = new TimeSpan(1, 0, 0) });
            return times;
        }
        
        public SettingsModel GetSettings()
        {
            return new SettingsModel()
            {
                CollectionRefreshTime = SelectedCollectionTime.Time,
                BackgroundRefreshTime = SelectedBackgroundTime.Time
            };

        }
    }
}
