using AutomatedDesktopBackgroundLibrary;
using AutomatedDesktopBackgroundUI.Config;
using AutomatedDesktopBackgroundUI.SessionData;
using AutomatedDesktopBackgroundUI.Utility;
using Caliburn.Micro;
using System;
using System.Windows.Media;

namespace AutomatedDesktopBackgroundUI.ViewModels
{
    public class ShellViewModel : Conductor<Screen>, IHandle<EventContainer>
    {

        private SimpleContainer _simpleContainer;
        private IEventAggregator _eventAggregator;
        private ISessionContext _sessionContext;
        private IDataAccess _dataAccess;
       
        public ShellViewModel(SimpleContainer simpleContainer)
        {
            _simpleContainer = simpleContainer;
            _eventAggregator = (IEventAggregator) simpleContainer.GetInstance(typeof(IEventAggregator), null);
            _sessionContext = (ISessionContext)simpleContainer.GetInstance(typeof(ISessionContext), null);
            _dataAccess = (IDataAccess)simpleContainer.GetInstance(typeof(IDataAccess), null);
            _eventAggregator.Subscribe(this);
            UpdateConnectionStatus();
            LoadMain();
        }
        private void BuildUpContainer()
        {

        }

        private void SkipWallpaper()
        {
            _dataAccess.SkipCurrentImage();
        }

        public void LoadSettings()
        {
            ActivateItem(new SettingsViewModel(_sessionContext,_eventAggregator));
        }
        public void LoadMain()
        {
            ActivateItem(new MainViewModel(_sessionContext, _eventAggregator));
        }
        private bool _isConnected;
        public void UpdateConnectionStatus()
        {
            IsConnnected = GlobalConfig.IsConnected();
        }
        public bool IsConnnected
        {
            get
            {
                return _isConnected;
            }
            set
            {
                _isConnected = value;
                SetConnectionContent();

            }
        }
        private string _connectionStatus;

        public string ConnectionStatus
        {
            get { return _connectionStatus; }
            set
            {
                _connectionStatus = value;
                NotifyOfPropertyChange(() => ConnectionStatus);
            }
        }

        private SolidColorBrush _brush;

        public SolidColorBrush ConnectionColor
        {
            get { return _brush; }
            set
            {
                _brush = value;
                NotifyOfPropertyChange(() => ConnectionColor);
            }
        }

        private void SetConnectionContent()
        {
            if (IsConnnected)
            {
                ConnectionStatus = "Connected";                     //Green -ish
                ConnectionColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("#a6d785"));
            }
            else
            {
                ConnectionStatus = "Offline";                       //Red -ish
                ConnectionColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("#b36b6b"));
            }
        }

        public void Handle(EventContainer eventContainer)
        {
            switch (eventContainer.Command)
            {
                case CommandNames.CheckConnection:
                    UpdateConnectionStatus();
                    break;
                case CommandNames.AcceptSettings:
                    LoadMain();
                    break;
                case CommandNames.RevertSettings:
                    LoadMain();
                    break;
                default:
                    break;
            }
        }
    }
}
