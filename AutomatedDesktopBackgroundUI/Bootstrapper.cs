using AutomatedDesktopBackgroundUI.ViewModels;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AutomatedDesktopBackgroundUI
{
    public class Bootstrapper:BootstrapperBase
    {
        private SimpleContainer container;
        private IWindowManager windowManager = new WindowManager();
        public Bootstrapper()
        {
            
            Initialize();

        }

        protected override void Configure()
        {
            container = new SimpleContainer();

            container.Singleton<IWindowManager, WindowManager>();
            windowManager =(IWindowManager) container.GetInstance(typeof(IWindowManager),"");
            container.PerRequest<ShellViewModel>();
        }
        protected override void OnStartup(object sender, StartupEventArgs e)
        {

            //windowManager.ShowWindow( new ShellViewModel());
            DisplayRootViewFor<ShellViewModel>();
        }

        protected override object GetInstance(Type service, string key)
        {
            return container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            container.BuildUp(instance);
        }
        

    }
}
