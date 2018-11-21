using AutomatedDesktopBackgroundLibrary;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace AutomatedDesktopBackgroundUI.ViewModels 
{
    public class ShellViewModel:Conductor<Screen>, IHandle<string>
    {
        private static IEventAggregator _eventAggregator;
        private IWindowManager _windowManager;
        public ShellViewModel(IWindowManager windowManager)
        {
            _windowManager = windowManager;
            _eventAggregator = new EventAggregator();
            _eventAggregator.Subscribe(this);
            UpdateConnectionStatus();
            LoadMain();
        }

        public void Handle(string message)
        {
            switch(message)
            {
                case "AcceptSettings":
                LoadMain();
                    break;
                case "CheckConnection":
                    UpdateConnectionStatus();
                    break;
                default:
                    break;
            }

        }

        public void LoadSettings()
        {
            ActivateItem(new SettingsViewModel(_eventAggregator));
        }
        public void LoadMain()
        {
            ActivateItem(new MainViewModel());
        }
        private bool _isConnected;
        public void UpdateConnectionStatus()
        {
            IsConnnected = GlobalConfig.IsConnected();
        }
        public bool IsConnnected
        {
            get {
                
                return _isConnected; }
            set
            {
                if (value != _isConnected)
                {
                    _isConnected = value;
                    SetConnectionContent();
                }
            
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
            if(IsConnnected)
            {
                ConnectionStatus = "Connected";                     //Green -ish
                ConnectionColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("#a6d785"));
            }
            //TODO HOOK THIS UP
            else
            {
                ConnectionStatus = "Offline";                       //Red -ish
                ConnectionColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("#b36b6b"));
            }
        }


    }
}
