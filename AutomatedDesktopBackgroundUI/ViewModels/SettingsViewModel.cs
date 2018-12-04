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
    public class SettingsViewModel:Conductor<Screen>.Collection.AllActive , IHandle<EventContainer>
    {
        public SetRefreshCycleViewModel SetRefreshCycleViewModel { get; private set; }
        private readonly IEventAggregator _eventAggregator;
        private ISessionContext _sessionContext;
        private SettingsModel _currentSettings;

        public SettingsViewModel(ISessionContext sessionContext, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _sessionContext = sessionContext;
            _currentSettings = _sessionContext.CurrentSettings;
            _eventAggregator.Subscribe(this);
            SetRefreshCycleViewModel = new SetRefreshCycleViewModel( _eventAggregator , _sessionContext, _currentSettings);

            Items.Add(SetRefreshCycleViewModel);
            
        }

        public void AcceptSettings()
        {
            //So slopy
            SettingsModel settings = SetRefreshCycleViewModel.GetSettings();
            _eventAggregator.PublishOnUIThread(new EventContainer() { Command = CommandNames.SettingsChanged, Data = settings });
            _eventAggregator.PublishOnUIThread(new EventContainer() { Command = CommandNames.AcceptSettings});
        }
        public void RevertSettings()
        {
            _eventAggregator.PublishOnUIThread(new EventContainer() { Command = CommandNames.RevertSettings });
        }

        public void Handle(EventContainer message)
        {
            if(message.Command == CommandNames.SettingsChanged)
            {
                SettingsModel settings =(SettingsModel) message.Data;
            }
        }
    }
}
