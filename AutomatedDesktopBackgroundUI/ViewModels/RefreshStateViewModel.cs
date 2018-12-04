using AutomatedDesktopBackgroundUI.Config;
using AutomatedDesktopBackgroundUI.Models;
using AutomatedDesktopBackgroundUI.SessionData;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AutomatedDesktopBackgroundUI.ViewModels
{
    public class RefreshStateViewModel:Screen
    {
        private ISessionContext _sessionContext;
        private RefreshStateModel _currentRefreshState;
        private IEventAggregator _eventAggregator;

        public RefreshStateModel CurrentRefreshState
        {
            get { return _currentRefreshState; }
            set
            {
                _currentRefreshState = value;
                NotifyOfPropertyChange(() => CurrentRefreshState);
            }
        }

        public RefreshStateViewModel(ISessionContext sessionContext, IEventAggregator eventAggregator)
        {
            _sessionContext = sessionContext;
            _sessionContext.PropertyChanged += SessionContextPropertyChanged;
            _eventAggregator = eventAggregator;
            UpdateView();

        }

        private void SessionContextPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == PropertyNames.RefreshState)
            {
                UpdateView();
            }

        }
        private void UpdateView()
        {
            CurrentRefreshState = _sessionContext.CurrentRefreshState;
            SetBackgroundMessage();
            SetCollectionMessage();
            EnableBackgroundRefresh = !CurrentRefreshState.IsBackgroundRefreshing;
            EnableCollectionRefresh = !CurrentRefreshState.IsCollectionRefreshing;
        }

        private void SetBackgroundMessage()
        {
            if(CurrentRefreshState.IsBackgroundRefreshing)
            {
                BackgroundMessage = "Backgrounds are refreshing";
            }
            else
            {
                BackgroundMessage = "Backgrounds are not refreshing";
            }
        }
        private void SetCollectionMessage()
        {
            if (CurrentRefreshState.IsCollectionRefreshing)
            {
                CollectionMessage = "Collections are refreshing";
            }
            else
            {
                CollectionMessage = "Collections are not refreshing";
            }
        }

        private string _backgroundMessage;

        public string BackgroundMessage
        {
            get { return _backgroundMessage; }
            set {
                _backgroundMessage = value;
                NotifyOfPropertyChange(() => BackgroundMessage);
            }
        }
        private string _collectionMessage;

        public string CollectionMessage
        {
            get { return _collectionMessage; }
            set {
                _collectionMessage = value;
                NotifyOfPropertyChange(() => CollectionMessage);
            }
        }
        private bool _enableBackgroundRefresh;

        public bool EnableBackgroundRefresh
        {
            get { return _enableBackgroundRefresh; }
            set
            {
                _enableBackgroundRefresh = value;
                NotifyOfPropertyChange(() => EnableBackgroundRefresh);
            }
        }
        private bool _enableCollectionRefresh;

        public bool EnableCollectionRefresh
        {
            get { return _enableCollectionRefresh; }
            set
            {
                _enableCollectionRefresh = value;
                NotifyOfPropertyChange(() => EnableCollectionRefresh);
            }
        }
        public void StopBackgroundRefresh()
        {
            _eventAggregator.PublishOnUIThread(CommandNames.StopBackgroundRefreshing);
        }
        public void StartBackgroundRefresh()
        {
            _eventAggregator.PublishOnUIThread(CommandNames.StartBackgroundRefreshing);
        }
        public void StopCollectionRefresh()
        {
            _eventAggregator.PublishOnUIThread(CommandNames.StopCollectionRefreshing);
        }
        public void StartCollectionRefresh()
        {
            _eventAggregator.PublishOnUIThread(CommandNames.StartCollectionRefreshing);
        }

    }
}
