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
        public MainViewModel()
        {
            //TODO build a view factory to abstract this 
            CurrentImageViewModel = new CurrentImageViewModel();
            RefreshStateViewModel = new RefreshStateViewModel();
            Items.Add(CurrentImageViewModel);
            Items.Add(RefreshStateViewModel);
        }
    }
}
