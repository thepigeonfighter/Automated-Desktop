using AutomatedDesktopBackgroundUI.Config;
using AutomatedDesktopBackgroundUI.Models;
using AutomatedDesktopBackgroundUI.Utility;
using Caliburn.Micro;
using System.Windows;

namespace AutomatedDesktopBackgroundUI.SessionData
{
    public class CommandControl : IHandle<EventContainer>
    {
        private IEventAggregator _eventAggregator;
        private IDataAccess _dataAccess;
        private ISessionContext _sessionContext;
        public CommandControl(IEventAggregator eventAggregator, IDataAccess dataAccess, ISessionContext sessionContext)
        {
            _eventAggregator = eventAggregator;
            _dataAccess = dataAccess;
            _eventAggregator.Subscribe(this);
            _sessionContext = sessionContext;
        }

        public void Handle(EventContainer eventContainer)
        {
            switch (eventContainer.Command)
            {
                case CommandNames.CheckConnection:
                    break;
                case CommandNames.LikeImage:
                    _dataAccess.LikeCurrentImage();
                    break;
                case CommandNames.HateImage:
                    _dataAccess.HateCurrentImage();
                    break;
                case CommandNames.AddInterest:

                    break;
                case CommandNames.RemoveInterest:
                    _dataAccess.RemoveInterest(_sessionContext.SelectedInterest);
                    break;
                case CommandNames.AcceptSettings:
                    break;
                case CommandNames.RevertSettings:
                    break;
                case CommandNames.DownloadCollection:
                    _dataAccess.DownloadCollection(_sessionContext.SelectedInterest);
                    _sessionContext.IsDownloading = true;
                    break;
                case CommandNames.SkipWallpaper:
                    _dataAccess.SkipCurrentImage();
                    break;
                case CommandNames.StartBackgroundRefreshing:
                    _dataAccess.StartBackgroundRefresh(_sessionContext.CurrentRefreshState);
                    break;
                case CommandNames.StopBackgroundRefreshing:
                    _dataAccess.StopBackgroundRefresh(_sessionContext.CurrentRefreshState);
                    break;
                case CommandNames.StartCollectionRefreshing:
                    _dataAccess.StartCollectionRefresh(_sessionContext.CurrentRefreshState);
                    break;
                case CommandNames.StopCollectionRefreshing:
                    _dataAccess.StopCollectionRefresh(_sessionContext.CurrentRefreshState);
                    break;
                case CommandNames.SettingsChanged:
                    _dataAccess.UpdateSettings((SettingsModel)eventContainer.Data);
                    break;
                default:
                    break;
            }
        }
    }
}
