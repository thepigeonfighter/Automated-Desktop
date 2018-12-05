using AutomatedDesktopBackgroundLibrary;
using AutomatedDesktopBackgroundUI.Config;
using AutomatedDesktopBackgroundUI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using AutomatedDesktopBackgroundUI.Properties;
using AutomatedDesktopBackgroundUI.Utility;
using AutomatedDesktopBackgroundLibrary.Utility;

namespace AutomatedDesktopBackgroundUI.SessionData
{
    public class DataAccess : IDataAccess, IFileListener, INotifyPropertyChanged
    {
        private IDataKeeper _dataKeeper;
        //Dependencies
        private InterestBuilder interestBuilder = new InterestBuilder();
        private IRefreshCycleServices cycleServices = new RefreshCycleServices();
        private IShellService shellService;

        private IFileCollection _fileCollection;
        private IAPIManager _apiManager;

        public event PropertyChangedEventHandler PropertyChanged;



        public DataAccess(IDataKeeper dataKeeper, IAPIManager apiManager, IShellExtension shellExtension)
        {
            _dataKeeper = dataKeeper;
            _apiManager = apiManager;
            InitializeDataAccessLayer();
            GlobalConfig.EventSystem.OnRefreshStatusChange += RefreshStatusChange;
            //TODO remove this into factory
            shellService = new ShellService(shellExtension);
            shellService.RestartRequest = OnRestartRequested;
        }

        private void OnRestartRequested()
        {
            Application.Current.Shutdown();

            shellService.ElevateApplication();
            
        }

        private void RefreshStatusChange(object sender, EventArgs e)
        {
            OnPropertyChanged(PropertyNames.RefreshState);
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void InitializeDataAccessLayer()
        {
            _fileCollection = _dataKeeper.GetFreshFileSnapShot();
            _dataKeeper.RegisterFileListener(this);
            GlobalConfig.EventSystem.DownloadedImageEvent += EventSystem_DownloadedImageEvent;
            GlobalConfig.EventSystem.DownloadCompleteEvent += EventSystem_DownloadCompleteEvent;
        }

        private void EventSystem_DownloadCompleteEvent(object sender, bool e)
        {
            OnPropertyChanged(PropertyNames.CollectionDownloaded);
        }

        private void EventSystem_DownloadedImageEvent(object sender, string e)
        {
            OnPropertyChanged(PropertyNames.ImageDownloaded);
        }



        public async Task<InterestInfoModel> AddInterest(InterestInfoModel item)
        {
            InterestModel interest = await interestBuilder.Build(item.Name, _fileCollection, _apiManager);
            InterestInfoModel model = new InterestInfoModel()
            {
                Name = interest.Name,
                Results = interest.TotalImages,

            };
            _dataKeeper.AddInterest(interest);
            return model;

        }

        public List<InterestInfoModel> GetAllInterests()
        {
            List<InterestModel> interests = _fileCollection.AllInterests;
            List<InterestInfoModel> interestInfo = new List<InterestInfoModel>();
            //TODO consider updating interest info to reflect this instead of using code to figure it out. 
            foreach (var i in interests)
            {
                int downloadedCount = 0;
                int hasSeenCount = 0;
                int likedCount = 0;
                int hatedCount = 0;
                foreach (var img in _fileCollection.AllImages.Where(x => x.InterestId == i.Id).ToList())
                {
                    if (img.IsDownloaded)
                    {
                        downloadedCount++;
                    }
                    if (img.HasBeenSeen)
                    {
                        hasSeenCount++;
                    }
                    if (img.IsFavorite)
                    {
                        likedCount++;
                    }
                    if (img.IsFavorite)
                    {
                        hatedCount++;
                    }

                }
                int percentageViewed = 0;
                if (hasSeenCount > 0)
                {
                    percentageViewed = i.TotalImages / hasSeenCount;
                }
                InterestInfoModel model = new InterestInfoModel()
                {
                    CurrentlyDownloaded = downloadedCount,
                    HatedImages = hatedCount,
                    LikedImages = likedCount,
                    Name = i.Name,
                    PercentageViewed = percentageViewed,
                    Results = i.TotalImages,
                    ImagesViewed = hasSeenCount
                    
                };
                interestInfo.Add(model);

            }
            return interestInfo;


        }

        public RefreshStateModel GetCurrentRefreshState()
        {
            RefreshStateModel refreshStateModel = new RefreshStateModel()
            {
                IsBackgroundRefreshing = GlobalConfig.BackgroundRefreshing,
                IsCollectionRefreshing = GlobalConfig.CollectionsRefreshing
            };
            return refreshStateModel;

        }

        public CurrentImageModel GetCurrentWallpaper()
        {
            ImageModel image = _dataKeeper.GetFileSnapShot().CurrentWallpaper;
            if (image == null)
            {
                return new CurrentImageModel()
                {
                    Name = "Unknown"
                };
            }
            CurrentImageModel currentImage = new CurrentImageModel()
            {
                IsHated = image.IsHated,
                IsLiked = image.IsFavorite,
                Name = image.Name,              
                LocalUrl = image.LocalUrl

            };
            return currentImage;

        }

        public void OnFileUpdate()
        {
            _fileCollection = _dataKeeper.GetFileSnapShot();
            OnPropertyChanged(PropertyNames.FileUpdate);
        }

        public void RemoveInterest(InterestInfoModel item)
        {
            InterestModel interestToRemove = _fileCollection.AllInterests.FirstOrDefault(x => x.Name == item.Name);
            if (interestToRemove != null)
            {
                _dataKeeper.DeleteInterest(interestToRemove);
            }

        }

        public void SetCurrentRefreshState()
        {
            throw new NotImplementedException();
        }
        public void SkipCurrentImage()
        {
            List<ImageModel> images = _dataKeeper.GetFileSnapShot().AllImages;
            if (images.Count > 0)
            {
                BackGroundPicker backGroundPicker = new BackGroundPicker(_dataKeeper);
                Task.Run(()=> backGroundPicker.PickRandomBackground(true)).ConfigureAwait(false);

            }
            else
            {
                MessageBox.Show("There are no images currently downloaded");
            }
        }

        public void LikeCurrentImage()
        {
            ImageModel currentImage = _dataKeeper.GetFileSnapShot().CurrentWallpaper;
            currentImage.IsFavorite = true;
            _dataKeeper.AddImage(currentImage);
        }
        public void HateCurrentImage()
        {

            ImageModel currentImage = _dataKeeper.GetFileSnapShot().CurrentWallpaper;
            currentImage.IsHated = true;
            _dataKeeper.DeleteImage(currentImage, true);
            SkipCurrentImage();
        }

        public void DownloadCollection(InterestInfoModel item)
        {
            _apiManager.GetImagesBySearch(item.Name, true);
        }

        public void SetRefreshState(EventContainer eventContainer)
        {
            switch (eventContainer.Command)
            {
                case CommandNames.StartBackgroundRefreshing:
                    cycleServices.StartBackgroundJob((RefreshStateModel)eventContainer.Data);
                    SkipCurrentImage();
                    break;
                case CommandNames.StopBackgroundRefreshing:
                    cycleServices.StopBackgroundJob((RefreshStateModel)eventContainer.Data);
                    break;
                case CommandNames.StartCollectionRefreshing:
                    cycleServices.StartCollectionJob((RefreshStateModel)eventContainer.Data);
                    break;
                case CommandNames.StopCollectionRefreshing:
                    cycleServices.StopCollectionJob((RefreshStateModel)eventContainer.Data);
                    break;
                case CommandNames.UpdateBackgroundCycle:
                    Task.Run(() => cycleServices.UpateBackgroundJobTimeAsync((RefreshStateModel)eventContainer.Data)).ConfigureAwait(false);
                    break;
                case CommandNames.UpdateCollectionCycle:
                    Task.Run(() => cycleServices.UpdateCollectionJobTimeAsync((RefreshStateModel)eventContainer.Data)).ConfigureAwait(false);
                    break;
                default:
                    break;
            }
        }
        public SettingsModel GetCurrentSettings()
        {
            Settings.Default.Upgrade();
            bool contextMenuEnabled = shellService.IsContextMenuEnable();
            Settings.Default.ContextMenu = contextMenuEnabled;
            Settings.Default.Save();
            SettingsModel model = new SettingsModel()
            {
                ShowSettingsWindowOnLoad = Settings.Default.ShowSettingsWindow,
                CollectionRefreshTime = Settings.Default.CollectionCycle,
                BackgroundRefreshTime = Settings.Default.BGCycle,
                EnableContextMenuButton = Settings.Default.ContextMenu,
                ShowWarningOnWindowClose = Settings.Default.ShowWarning

            };
            return model;
        }

        public void UpdateSettings(SettingsModel settings)
        {
            Settings.Default.CollectionCycle = settings.CollectionRefreshTime;
            Settings.Default.BGCycle = settings.BackgroundRefreshTime;
            Settings.Default.ContextMenu = settings.EnableContextMenuButton;
            Settings.Default.ShowSettingsWindow = settings.ShowSettingsWindowOnLoad;
            Settings.Default.ShowWarning = settings.ShowWarningOnWindowClose;
            Settings.Default.Save();
            OnPropertyChanged(PropertyNames.CurrentSettings);

        }
        public void UpdateContextMenu(EventContainer eventContainer)
        {
            switch (eventContainer.Command)
            {
                case CommandNames.AddContextMenuShortcut:
                    shellService.CreateShortCut();
                    break;
                case CommandNames.RemoveContextMenuShortcut:
                    shellService.RemoveShortCut();
                    break;
                default:
                    break;
            }

        }

        public void ResetApplication()
        {
            _dataKeeper.ResetApplication();
        }
    }
}
