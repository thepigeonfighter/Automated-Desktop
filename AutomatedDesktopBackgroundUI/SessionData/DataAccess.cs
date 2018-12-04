using AutomatedDesktopBackgroundLibrary;
using AutomatedDesktopBackgroundLibrary.Scheduler;
using AutomatedDesktopBackgroundUI.Config;
using AutomatedDesktopBackgroundUI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace AutomatedDesktopBackgroundUI.SessionData
{
    public class DataAccess : IDataAccess, IFileListener, INotifyPropertyChanged
    {
        private IDataKeeper _dataKeeper;
        private InterestBuilder interestBuilder = new InterestBuilder();
        private IFileCollection _fileCollection;
        private IAPIManager _apiManager;

        public event PropertyChangedEventHandler PropertyChanged;



        public DataAccess(IDataKeeper dataKeeper, IAPIManager apiManager)
        {
            _dataKeeper = dataKeeper;
            _apiManager = apiManager;
            InitializeDataAccessLayer();
            GlobalConfig.EventSystem.OnRefreshStatusChange += RefreshStatusChange;
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
        public void StartBackgroundRefresh(RefreshStateModel model)
        {
            if(!model.IsBackgroundRefreshing)
            {
                Task.Run(()=>GlobalConfig.JobManager.StartBackgroundUpdatingAsync()).ConfigureAwait(false);
                GlobalConfig.BackgroundRefreshing = true;
                SkipCurrentImage();
            }

        }
        public void StopBackgroundRefresh(RefreshStateModel model)
        {
            if (model.IsBackgroundRefreshing)
            {
                Task.Run(() => GlobalConfig.JobManager.StopBackgroundUpdatingAsync()).ConfigureAwait(false);
                GlobalConfig.BackgroundRefreshing = false;
            }
        }
        public void StartCollectionRefresh(RefreshStateModel model)
        {
            if (!model.IsCollectionRefreshing)
            {
                Task.Run(() => GlobalConfig.JobManager.StartCollectionUpdatingAsync()).ConfigureAwait(false);
                GlobalConfig.CollectionsRefreshing = true;
            }
        }
        public void StopCollectionRefresh(RefreshStateModel model)
        {
            if (model.IsCollectionRefreshing)
            {
                Task.Run(() => GlobalConfig.JobManager.StopCollectionUpdatingAsync()).ConfigureAwait(false);
                GlobalConfig.CollectionsRefreshing = false;
            }
        }

        public Models.SettingsModel GetCurrentSettings()
        {
            Models.SettingsModel settingsModel = new Models.SettingsModel()
            {
                BackgroundRefreshTime = ScheduleManager.BackgroundRefreshSetting(),
                CollectionRefreshTime = ScheduleManager.CollectionRefreshSetting(),
                //TODO hook this up
                EnableContextMenuButton = false
            };
            return settingsModel;
        }

        public void UpdateSettings(Models.SettingsModel settings)
        {
            ScheduleManager.ChangeBackgroundRefreshSettings(settings.BackgroundRefreshTime);
            ScheduleManager.ChangeCollectionRefreshSettings(settings.CollectionRefreshTime);
        }
    }
}
