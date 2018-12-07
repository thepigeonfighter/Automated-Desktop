using AutomatedDesktopBackgroundUI.Config;
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
    public class SettingsViewModel:Conductor<object>.Collection.AllActive
    {
        public ISettingScreen SetRefreshCycleViewModel { get; private set; }
        public ISettingScreen ConfigSettingsViewModel { get; private set; }
        private readonly IEventAggregator _eventAggregator;
        private ISessionContext _sessionContext;
        private SettingsModel _currentSettings;

        public SettingsViewModel(ISessionContext sessionContext, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _sessionContext = sessionContext;
            _currentSettings = _sessionContext.CurrentSettings;
            SetRefreshCycleViewModel = new SetRefreshCycleViewModel( _currentSettings);
            ConfigSettingsViewModel = new ConfigSettingsViewModel(_currentSettings, _eventAggregator);

            Items.Add(SetRefreshCycleViewModel);
            Items.Add(ConfigSettingsViewModel);
        }

        public void AcceptSettings()
        {
            //So slopy
            SettingsModel settings = SetRefreshCycleViewModel.GetSettings();
            SettingsModel configSettings = ConfigSettingsViewModel.GetSettings();
            settings.EnableContextMenuButton = configSettings.EnableContextMenuButton;
            settings.ShowWarningOnWindowClose = configSettings.ShowWarningOnWindowClose;

            _eventAggregator.PublishOnUIThread(new EventContainer() { Command = CommandNames.SettingsChanged, Data = settings });
            _eventAggregator.PublishOnUIThread(new EventContainer() { Command = CommandNames.AcceptSettings});
        }
        public void RevertSettings()
        {
            _eventAggregator.PublishOnUIThread(new EventContainer() { Command = CommandNames.RevertSettings });
        }
        public void ResetApplication()
        {
            //TODO implement custom messagebox
            if (MessageBox.Show("Are you sure you want to reset the application?", "Reset Application",
                MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
            {
                _eventAggregator.PublishOnUIThread(new EventContainer() { Command = CommandNames.ResetApplication });
                _eventAggregator.PublishOnUIThread(new EventContainer() { Command = CommandNames.RevertSettings });
            }
        }
        public void QuitApplication()
        {
            _eventAggregator.PublishOnUIThread(new EventContainer() { Command = CommandNames.QuitApplication });
           
        }

    }
}
