﻿using AutomatedDesktopBackgroundLibrary;
using AutomatedDesktopBackgroundUI.Config;
using AutomatedDesktopBackgroundUI.Models;
using AutomatedDesktopBackgroundUI.SessionData;
using AutomatedDesktopBackgroundUI.Utility;
using Caliburn.Micro;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Media;

namespace AutomatedDesktopBackgroundUI.ViewModels
{
    public class ShellViewModel : Conductor<Screen>.Collection.OneActive, IHandle<EventContainer>
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
            _sessionContext.ForceSettingsUpdate();
            StartUp();
        }

        private void StartUp()
        {
            if(_sessionContext.CurrentSettings.ShowSettingsWindowOnLoad)
            {
                LoadSettings();
                SettingsModel tempSettings = _sessionContext.CurrentSettings;
                tempSettings.ShowSettingsWindowOnLoad = false;
                _eventAggregator.PublishOnUIThread(new EventContainer() { Command = CommandNames.SettingsChanged, Data = tempSettings });
            }
            else if(!_sessionContext.CurrentSettings.ShowSettingsWindowOnLoad)
            {
                LoadMain();
            }
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
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);          
            VersionNumber =  $"v.{ fileVersionInfo.ProductVersion}";
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
        private string _versionNumber;

        public string VersionNumber
        {
            get { return _versionNumber; }
            set
            { _versionNumber = value;
                NotifyOfPropertyChange(() => VersionNumber);
            }
        }


        private void SetConnectionContent()
        {
            if (IsConnnected)
            {
                ConnectionStatus = "Connected";                     //Green -ish
                ConnectionColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("#6e9e6a"));
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
                case CommandNames.QuitApplication:
                    TryClose();
                    break;
                default:
                    break;
            }
        }
        public void CloseWindow()
        {
            if (_sessionContext.CurrentSettings.ShowWarningOnWindowClose)
            {
                string message = "This does not close program, It only hides the window. To close program go to settings, then click quit application";
                MessageBox.Show(message, "Hide Window", MessageBoxButton.OK);
            }
        }
        public void HateImage()
        {
            MessageBox.Show("hated ");
        }
    }
}
