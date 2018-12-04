using AutomatedDesktopBackgroundUI.SessionData;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundUI.ViewModels
{
    public class MainViewModel:Conductor<Screen>.Collection.AllActive
    {
        public Screen CurrentImageViewModel { get; private set; }

        public Screen RefreshStateViewModel { get; private set; }

        public Screen InterestListViewModel { get; private set; }

        public Screen InterestEntryViewModel { get; private set; }
        

        public MainViewModel(ISessionContext sessionContext, IEventAggregator eventAggregator)
        {
            //TODO build a view factory to abstract this 
            CurrentImageViewModel = new CurrentImageViewModel(sessionContext,eventAggregator);
            RefreshStateViewModel = new RefreshStateViewModel(sessionContext,eventAggregator);
            InterestListViewModel = new InterestListViewModel(sessionContext,eventAggregator);
            InterestEntryViewModel = new InterestEntryViewModel(sessionContext);
            Items.Add(CurrentImageViewModel);
            Items.Add(RefreshStateViewModel);
            Items.Add(InterestListViewModel);
            Items.Add(InterestEntryViewModel);
        }
    }
}
