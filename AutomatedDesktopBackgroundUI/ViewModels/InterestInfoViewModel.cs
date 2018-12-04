using System;
using AutomatedDesktopBackgroundUI.Config;
using AutomatedDesktopBackgroundUI.Models;
using AutomatedDesktopBackgroundUI.SessionData;
using AutomatedDesktopBackgroundUI.Utility;
using Caliburn.Micro;
namespace AutomatedDesktopBackgroundUI.ViewModels
{
    public class InterestInfoViewModel:Screen
    {
        private readonly InterestInfoModel _model;
        private string _interestInfo;
        private IEventAggregator _eventAggregator;
        private ISessionContext _sessionContext;
        private int _downloadCount;
        private bool _visibility;

        public bool IsVisible
        {
            get { return _visibility; }
            set {
                if (value != _visibility)
                {
                    _visibility = value;
                    NotifyOfPropertyChange(() => IsVisible);
                    DisableButton();
                }
            }
        }


        public string InterestInfo
        {
            get { return _interestInfo; }
            set
            {
                    _interestInfo = value;
                    NotifyOfPropertyChange(() => InterestInfo);
            }
        }

        private void DisableButton()
        {
            RemoveInterestButton = IsVisible;
        }

        public InterestInfoViewModel(InterestInfoModel model ,IEventAggregator eventAggregator, ISessionContext sessionContext)
        {
            _model = model;
            InterestInfo = this.ToString();
            _eventAggregator = eventAggregator;
            _sessionContext = sessionContext;
            _sessionContext.PropertyChanged += SessionContextPropertyChanged;
            IsVisible =! _sessionContext.IsDownloading;
        }

        private void SessionContextPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == PropertyNames.IsDownloading)
            {
                IsVisible = !_sessionContext.IsDownloading;
               
            }
            else if(e.PropertyName == PropertyNames.ImageDownloaded)
            {
                HandleImageDownloadedEvent();
            }
            else if(e.PropertyName == PropertyNames.CollectionDownloaded)
            {
                IsVisible = _sessionContext.IsDownloading;
                HandleDownloadCompleteEvent();

            }
        }

        public override string ToString()
        {
            string output =
                $" Results : {_model.Results} " +
                $"\n Images Downloaded: {_model.CurrentlyDownloaded}" +
                $"\n Liked Images: {_model.LikedImages} " +
                $"\n Hated Images: {_model.HatedImages}\n Images Viewed:{_model.ImagesViewed}";
            return output; 
        }
        private bool _removeInterestButton;

        public bool RemoveInterestButton
        {
            get { return _removeInterestButton; }
            set {
                _removeInterestButton = value;
                NotifyOfPropertyChange(() => RemoveInterestButton);
            }
        }

        public void RemoveInterest()
        {
            _eventAggregator.PublishOnUIThread(new EventContainer() { Command= CommandNames.RemoveInterest });
        }
        public void DownloadImages()
        {
            _eventAggregator.PublishOnUIThread(new EventContainer() { Command = CommandNames.DownloadCollection });
        }
        private string _downloadInfo ="";

        public string DownloadInfo
        {
            get { return _downloadInfo; }
            set {
                _downloadInfo = value;
                NotifyOfPropertyChange(() => DownloadInfo);
            }
        }
        private void HandleDownloadCompleteEvent()
        {
            _downloadCount = 0;
            DownloadInfo = "";
        }

        private void HandleImageDownloadedEvent()
        {
            _downloadCount++;
            if (_downloadCount > 1)
            {
                DownloadInfo = $"{_downloadCount} images downloaded.";
            }
            else
            {
                DownloadInfo = $"{_downloadCount} image downloaded.";
            }
        }
       
    }
}
