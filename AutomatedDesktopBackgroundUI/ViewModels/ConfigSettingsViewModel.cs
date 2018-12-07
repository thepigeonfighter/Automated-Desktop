using AutomatedDesktopBackgroundUI.Models;
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
    public class ConfigSettingsViewModel:Screen ,ISettingScreen
    {
        
        public bool IsContextMenuEnabled
        {
            get { return _isContextMenuEnabled; }
            set
            {
                _isContextMenuEnabled = value;
                NotifyOfPropertyChange(() => IsContextMenuEnabled);
                UpdateValue();
            }
        }
        

        public bool ShowWarningOnExit
        {
            get { return _showWarningOnExit; }
            set
            {
                _showWarningOnExit = value;
                NotifyOfPropertyChange(() => ShowWarningOnExit);
            }
        }

        //SO hacky makes the update value method not called when the page first loads
        private int _counter = 0;

        private bool _showWarningOnExit;
        private bool _isContextMenuEnabled;
        private SettingsModel _currentSettings;
        private IEventAggregator _eventAggregator;

        public ConfigSettingsViewModel(SettingsModel currentSettings, IEventAggregator eventAggregator)
        {
            _currentSettings = currentSettings;
            IsContextMenuEnabled = _currentSettings.EnableContextMenuButton;
            ShowWarningOnExit = _currentSettings.ShowWarningOnWindowClose;
            _eventAggregator = eventAggregator;
        }
        public SettingsModel GetSettings()
        {
            SettingsModel settings = new SettingsModel()
            {
                EnableContextMenuButton = IsContextMenuEnabled,
                ShowWarningOnWindowClose = ShowWarningOnExit

            };
            return settings;
        }
        private void UpdateValue()
        {
            _counter++;
            if (_counter > 1)
            {
                if(IsContextMenuEnabled)
                {
                    _eventAggregator.PublishOnUIThread(new EventContainer() { Command = Config.CommandNames.AddContextMenuShortcut });
                }
                else
                {
                    _eventAggregator.PublishOnUIThread(new EventContainer() { Command = Config.CommandNames.RemoveContextMenuShortcut });
                }
            }
        }
    }
}
