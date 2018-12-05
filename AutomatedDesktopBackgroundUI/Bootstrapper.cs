using AutomatedDesktopBackgroundLibrary;
using AutomatedDesktopBackgroundLibrary.Utility;
using AutomatedDesktopBackgroundUI.SessionData;
using AutomatedDesktopBackgroundUI.ViewModels;
using AutomatedDesktopBackgroundUI.Views;
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
        public Bootstrapper()
        {
            Initialize();
        }

        protected override void Configure()
        {
            container = new SimpleContainer();
             IEventAggregator _eventAggregator = new EventAggregator();
             IDataStorageBuilder _dataStorageBuilder = new DataStorageBuilder();
             IDataStorage _dataStorage = _dataStorageBuilder.Build(Database.JsonFile);
             IDataKeeper _dataKeeper = new DataKeeper(_dataStorage);
             ImageModelBuilder _imageBuilder = new ImageModelBuilder();
             ImageGetter _imageGetter = new ImageGetter(_dataKeeper, _imageBuilder);
             IAPIManager _apiManager = new APIManager(_imageGetter, _dataKeeper);
            //TODO build a data access builder
            IShellExtension _shellExtension = new WindowsShellExtension();
             IDataAccess _dataAccess = new DataAccess(_dataKeeper, _apiManager, _shellExtension);
             ISessionContext _sessionContext = new SessionContext(_dataAccess);
             CommandControl _commandControl = new CommandControl(_eventAggregator, _dataAccess, _sessionContext);
            
            container.Instance(_dataStorageBuilder);
            container.Instance(_eventAggregator);
            container.Instance(_dataStorage);
            container.Instance(_dataKeeper);
            container.Instance(_imageBuilder);
            container.Instance(_imageGetter);
            container.Instance(_apiManager);
            container.Instance(_shellExtension);
            container.Instance(_dataAccess);
            container.Instance(_sessionContext);
            container.Instance(_commandControl);
           // IShellViewModel _shellViewModel = new ShellViewModel(container);
          //  container.Singleton<ShellViewModel>();
            
        }
        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            //ShellViewModel shellViewModel =(ShellViewModel)container.GetInstance(typeof(ShellViewModel), null);
            //  DisplayRootViewFor(typeof(ShellViewModel));
            ShellViewModel shell =  new ShellViewModel(container);
            IEventAggregator _eventAggregator = (IEventAggregator)container.GetInstance(typeof(IEventAggregator), null);
            var shellView = new ShellView(_eventAggregator);
            ViewModelBinder.Bind(shell, shellView,this);
            shellView.Show();
            
         
            
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
