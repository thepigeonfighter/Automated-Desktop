using AutomatedDesktopBackgroundLibrary.Scheduler;
using AutomatedDesktopBackgroundLibrary.StringExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace AutomatedDesktopBackgroundLibrary
{
    public class MainViewController : IFileListener
    {
        public event EventHandler<string> OnPageStateChange;

        public PageRefreshState refreshState;
        private IFileCollection _fileCollection = new FileCollection();
        private readonly InterestBuilder interestBuilder = new InterestBuilder();
        private readonly APIManager manager = new APIManager();
        public BindingList<InterestModel> interests;// = new BindingList<InterestModel>();
        private bool IsDownloading = false;

        public MainViewController()
        {
            GlobalConfig.EventSystem.DownloadCompleteEvent += EventSystem_DownloadCompleteEvent;
            GlobalConfig.EventSystem.ApplicationResetEvent += EventSystem_ApplicationResetEvent;
            DataKeeper.RegisterFileListener(this);
            DataKeeper.UpdateAllLists();

            interests = new BindingList<InterestModel>(DataKeeper.GetFileSnapShot().AllInterests)
            {
                RaiseListChangedEvents = true
            };
        }

        /// <summary>
        /// Sets the state of the page to give a blueprint for what buttons should be enabled
        /// </summary>
        /// <param name="command"></param>
        public void SetPageState(ButtonCommands command)
        {
            switch (command)
            {
                case ButtonCommands.StartCollections:
                    if (refreshState == PageRefreshState.None)
                    {
                        refreshState = PageRefreshState.ColOnly;
                    }
                    else
                    {
                        refreshState = PageRefreshState.BGAndCol;
                    }
                    break;

                case ButtonCommands.StartBackground:
                    if (refreshState == PageRefreshState.None)
                    {
                        refreshState = PageRefreshState.BGOnly;
                    }
                    else
                    {
                        refreshState = PageRefreshState.BGAndCol;
                    }
                    break;

                case ButtonCommands.StopCollections:
                    if (refreshState == PageRefreshState.ColOnly)
                    {
                        refreshState = PageRefreshState.None;
                    }
                    else
                    {
                        refreshState = PageRefreshState.BGOnly;
                    }
                    break;

                case ButtonCommands.StopBackground:
                    if (refreshState == PageRefreshState.BGOnly)
                    {
                        refreshState = PageRefreshState.None;
                    }
                    else
                    {
                        refreshState = PageRefreshState.ColOnly;
                    }
                    break;

                case ButtonCommands.SetToStartState:
                    refreshState = PageRefreshState.None;
                    break;
            }

            OnPageStateChange?.Invoke(this, "Page is changing");
        }

        private void EventSystem_ApplicationResetEvent(object sender, string e)
        {
            Task.Run(() => GlobalConfig.JobManager.StopSchedulerAsync());
            interests.Clear();
        }

        public ImageModel GetCurrentWallPaper()
        {
            ImageModel fakeImage = new ImageModel() { Name = "Unknown" };
            ImageModel currentWallpaper = _fileCollection.CurrentWallpaper;
            return currentWallpaper ?? fakeImage;
        }

        private void EventSystem_DownloadCompleteEvent(object sender, bool e)
        {
            IsDownloading = false;
        }

        public async Task AddInterest(string interest)
        {
            InterestModel newInterest = await interestBuilder.Build(interest, _fileCollection, manager).ConfigureAwait(true);
            DataKeeper.AddInterest(newInterest);
            interests = new BindingList<InterestModel>(DataKeeper.GetFileSnapShot().AllInterests);
        }

        public void RemoveInterest(string interest)
        {
            InterestModel interestToDelete = interest.GetInterestByName();
            DataKeeper.DeleteInterest(interestToDelete);
            InterestModel instance = interests.FirstOrDefault(x => x.Name == interest);
            if (instance != null)
            {
                interests.Remove(instance);
            }
        }

        public void DownloadNewCollection(string query)
        {
            if (!IsDownloading)
            {
                GlobalConfig.EventSystem.InvokeStartedDownloadingEvent();
                Task.Run(() => manager.GetImagesBySearch(query, true));
                IsDownloading = true;
            }
        }

        public bool AreAnyImagesDownloaded()
        {
            OnFileUpdate();
            List<ImageModel> images = _fileCollection.AllImages;
            return images.Count > 0;
        }

        public void CloseWindow()
        {
            WindowManager.CloseRootWindow();
        }
        public bool ShouldDisplayWarning()
        {
            return ScheduleManager.GetWarningFlagStatus();
        }
        #region Background and Collection Controls

        public async Task StartCollectionRefresh()
        {
            if (refreshState == PageRefreshState.None || refreshState == PageRefreshState.BGOnly)
            {
                await Task.Run(() => GlobalConfig.JobManager.StartCollectionUpdatingAsync()).ConfigureAwait(false);
                GlobalConfig.CollectionsRefreshing = true;
            }
        }

        public async Task StartBackGroundRefresh(bool forceStart = false)
        {
            if (!forceStart)
            {
                if (refreshState == PageRefreshState.None || refreshState == PageRefreshState.ColOnly)
                {
                    await Task.Run(() => GlobalConfig.JobManager.StartBackgroundUpdatingAsync()).ConfigureAwait(false);
                    BackGroundPicker bg = new BackGroundPicker();
                    bg.PickRandomBackground(true);
                }
            }
            else
            {
                BackGroundPicker bg = new BackGroundPicker();
                bg.PickRandomBackground(true);
                await Task.Delay(300).ConfigureAwait(false);
                await Task.Run(() => GlobalConfig.JobManager.StartBackgroundUpdatingAsync()).ConfigureAwait(false);
            }
            GlobalConfig.BackgroundRefreshing = true;
        }

        public async Task StopBackGroundRefresh()
        {
            if (refreshState != PageRefreshState.None || refreshState != PageRefreshState.ColOnly)
            {
                await Task.Run(() => GlobalConfig.JobManager.StopBackgroundUpdatingAsync()).ConfigureAwait(false);
            }
            GlobalConfig.BackgroundRefreshing = false;
        }

        public async Task StopCollectionChange()
        {
            if (refreshState != PageRefreshState.None || refreshState != PageRefreshState.BGOnly)
            {
                await Task.Run(() => GlobalConfig.JobManager.StopCollectionUpdatingAsync()).ConfigureAwait(false);
            }
            GlobalConfig.BackgroundRefreshing = false;
        }

        #endregion Background and Collection Controls

        public void SetImageAsFavorite()
        {
            ImageModel currentImage = _fileCollection.CurrentWallpaper;
            currentImage.IsFavorite = true;
            DataKeeper.AddImage(currentImage);
        }

        public void SetImageAsHated()
        {
            ImageModel currentImage = _fileCollection.CurrentWallpaper;
            currentImage.IsHated = true;
            currentImage.IsFavorite = false;
            currentImage.IsDownloaded = false;
            DataKeeper.AddImage(currentImage);
            DataKeeper.DeleteImage(currentImage, true);

            BackGroundPicker bg = new BackGroundPicker();
            bg.PickRandomBackground(true);
        }

        public bool IsFavorited()
        {
            if (_fileCollection.CurrentWallpaper != null)
            {
                return _fileCollection.CurrentWallpaper.IsFavorite;
            }
            else
            {
                return false;
            }
        }

        public bool InterestExists(string interest)
        {
            return interest.GetInterestByName() != null;
        }

        public int GetInterestTotalImages(string interestName)
        {
            InterestModel interest = _fileCollection.AllInterests.FirstOrDefault(x => x.Name == interestName);
            int totalImages = 0;
            if (interest != null)
            {
                totalImages = interest.TotalImages;
            }
            return totalImages;
        }

        public void OnFileUpdate()
        {
            _fileCollection = DataKeeper.GetFileSnapShot();
        }
        public bool ShowSettingsWindow()
        {
            try
            {
                SettingsModel settings = new SettingsModel();
                settings = settings.LoadSettings();
                return settings.StartWithSettingsWindowOpen;
            }
            catch
            {

                return false;
            }
        }
    }
}