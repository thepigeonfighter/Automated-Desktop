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
    public class CurrentImageViewModel:Screen
    {
        private ISessionContext _sessionContext;
        private CurrentImageModel _currentImage;
        private IEventAggregator _eventAggregator;

        public CurrentImageViewModel(ISessionContext sessionContext, IEventAggregator eventAggregator)
        {
            _sessionContext = sessionContext;
            _eventAggregator = eventAggregator;
            CurrentImage = _sessionContext.CurrentWallpaper;
            _sessionContext.PropertyChanged += SessionContextPropertyChanged;
            this.Deactivated += CurrentImageViewModel_Deactivated;
        }

        private void CurrentImageViewModel_Deactivated(object sender, DeactivationEventArgs e)
        {
            _sessionContext.PropertyChanged -= SessionContextPropertyChanged;
        }

        private void SessionContextPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == PropertyNames.CurrentWallpaper || e.PropertyName == PropertyNames.FileUpdate)
            {
                CurrentImage = _sessionContext.CurrentWallpaper;
                EnableHateButton = CanHateImage();
                EnableLikeButton = CanLikeImage();
            }
           
        }

        public CurrentImageModel CurrentImage
        {
            get { return _currentImage; }
            set
            {
                _currentImage = value;
                NotifyOfPropertyChange(() => CurrentImage);
                NotifyOfPropertyChange(() => Title);
            }
        }
        public string Title { get
            {
                return $"Current Image is: {_currentImage.Name}";
            } private set
            {
               
            }
        }
        public void SkipWallpaper()
        {
            _eventAggregator.PublishOnUIThread(CommandNames.SkipWallpaper);
            
        }
        public bool CanHateImage()
        {
            if (_currentImage == null)
            {
                return false;
            }
            if (_currentImage.IsHated)
            {
                return false;
            }
            if (_currentImage.Name == "Unknown")
            {
                return false;
            }
            return true;
        }
        public void HateImage()
        {
            _eventAggregator.PublishOnUIThread(CommandNames.HateImage);
        }
        public void LikeImage()
        {
            _eventAggregator.PublishOnUIThread(CommandNames.LikeImage);
        }
        private bool _enableLikeButton;

        public bool EnableLikeButton
        {
            get
            {
                return _enableLikeButton;
            }
            set {
                _enableLikeButton = value;
                NotifyOfPropertyChange(() => EnableLikeButton);
            }
        }
        private bool _enableHateButton;

        public bool EnableHateButton
        {
            get { return _enableHateButton; }
            set
            {
                _enableHateButton = value;
                NotifyOfPropertyChange(() => EnableHateButton);
            }
        }

        private bool CanLikeImage()
        {
            if(_currentImage == null)
            {
                return false;
            }
            if(_currentImage.IsLiked)
            {
                return false;
            }
            if(_currentImage.Name == "Unknown")
            {
                return false;
            }
            return true;
        }
    }
}
