using AutomatedDesktopBackgroundUI.Models;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundUI.ViewModels
{
    public class ConfigSettingsViewModel:Screen ,ISettingScreen
    {
        private bool _isContextMenuEnabled;
        //TODO implement this!!!
        public bool IsContextMenuEnabled
        {
            get { return _isContextMenuEnabled; }
            set
            {
                _isContextMenuEnabled = value;
                NotifyOfPropertyChange(() => IsContextMenuEnabled);
            }
        }
        private bool _showWarningOnExit;

        public bool ShowWarningOnExit
        {
            get { return _showWarningOnExit; }
            set
            {
                _showWarningOnExit = value;
                NotifyOfPropertyChange(() => ShowWarningOnExit);
            }
        }
        private SettingsModel _currentSettings;

        public ConfigSettingsViewModel(SettingsModel currentSettings)
        {
            _currentSettings = currentSettings;
            IsContextMenuEnabled = _currentSettings.EnableContextMenuButton;
            ShowWarningOnExit = _currentSettings.ShowWarningOnWindowClose;
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
    }
}
