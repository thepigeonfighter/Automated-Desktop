using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AutomatedDesktopBackgroundUI.ViewModels
{
    public class SettingsViewModel:Screen , IHandle
    {
        private readonly IEventAggregator _eventAggregator;
        public SettingsViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }
        public void AcceptSettings()
        {
            //TODO replace this with some meaningful type
            _eventAggregator.BeginPublishOnUIThread("AcceptSettings");
        }
        public void RevertSettings()
        {
            _eventAggregator.BeginPublishOnUIThread("AcceptSettings");
        }

    }
}
