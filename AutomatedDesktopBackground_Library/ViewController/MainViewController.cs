using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using AutomatedDesktopBackgroundLibrary.StringExtensions;

namespace AutomatedDesktopBackgroundLibrary
{
    public class MainViewController : IFileListener
    {
        public event EventHandler<string> OnPageStateChange;
        public PageRefreshState refreshState;
        private IFileCollection _fileCollection = new FileCollection();
        APIManager manager = new APIManager();
        public BindingList<InterestModel> interests;// = new BindingList<InterestModel>();
        private bool IsDownloading = false;
        public MainViewController()
        {

            GlobalConfig.EventSystem.DownloadCompleteEvent += EventSystem_DownloadCompleteEvent;
            GlobalConfig.EventSystem.ApplicationResetEvent += EventSystem_ApplicationResetEvent;
            GlobalConfig.EventSystem.ImageHatingHasCompletedEvent += EventSystem_ImageHatingHasCompletedEvent;
            DataKeeper.RegisterFileListener(this);
            DataKeeper.UpdateAllLists();

            interests = new BindingList<InterestModel>(DataKeeper.GetFileSnapShot().AllInterests);
            interests.RaiseListChangedEvents = true;

        }

        private async void EventSystem_ImageHatingHasCompletedEvent(object sender, string e)
        {
            bool isRefreshing = await GlobalConfig.JobManager.JobRunning(JobType.BackgroundRefresh);
            if (!isRefreshing)
            {
                await StartBackGroundRefresh(true);
            }

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
            if (currentWallpaper != null)
            {
                return currentWallpaper;
            }
            return fakeImage;
        }
        private void EventSystem_DownloadCompleteEvent(object sender, bool e)
        {
            IsDownloading = false;
        }
        //TODO build a interest builder maybe
        public async Task AddInterest(string interest)
        {
            List<InterestModel> existinginterests = _fileCollection.AllInterests;

            int newId = 1;
            if (existinginterests.Count > 0)
            {
                newId = existinginterests.Max(x => x.Id) + 1;
            }
            InterestModel newInterest = new InterestModel();
            IRootObject response = await manager.GetResults(interest);
            newInterest.Name = interest;
            newInterest.TotalImages = response.total;
            newInterest.TotalPages = response.total_pages;
            newInterest.Id = newId;
            existinginterests.Add(newInterest);
            DataKeeper.AddInterest(newInterest);
            interests = new BindingList<InterestModel>(existinginterests);

        }
        public void RemoveInterest(string interest)
        {
            InterestModel interestToDelete = interest.GetInterestByName();
            DataKeeper.DeleteInterest(interestToDelete);
            interests.Remove(interests.First(x => x.Name == interest));
        }

        private void RefreshInterestList()
        {
            var tempList = _fileCollection.AllInterests;
            interests = new BindingList<InterestModel>(tempList);
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
            List<ImageModel> images = _fileCollection.AllImages;
            images.AddRange(_fileCollection.FavoriteImages);
            if (images.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void CloseProgram()
        {
            WindowManager.CloseRootWindow();
        }
        #region Background and Collection Controls
        public async Task StartCollectionRefresh()
        {
            if (refreshState == PageRefreshState.None || refreshState == PageRefreshState.BGOnly)
            {
                await Task.Run(() => GlobalConfig.JobManager.StartCollectionUpdatingAsync());
            }
        }
        public async Task StartBackGroundRefresh(bool forceStart = false)
        {
            if (!forceStart)
            {
                if (refreshState == PageRefreshState.None || refreshState == PageRefreshState.ColOnly)
                {
                    await Task.Run(() => GlobalConfig.JobManager.StartBackgroundUpdatingAsync());
                    BackGroundPicker bg = new BackGroundPicker();
                    bg.PickRandomBackground();
                }
            }
            else
            {
                BackGroundPicker bg = new BackGroundPicker();
                bg.PickRandomBackground();
                await Task.Delay(300);
                await Task.Run(() => GlobalConfig.JobManager.StartBackgroundUpdatingAsync());

            }

        }
        public async Task StopBackGroundRefresh()
        {
            if (refreshState != PageRefreshState.None || refreshState != PageRefreshState.ColOnly)
            {
                await Task.Run(() => GlobalConfig.JobManager.StopBackgroundUpdatingAsync());
            }
        }
        public async Task StopCollectionChange()
        {
            if (refreshState != PageRefreshState.None || refreshState != PageRefreshState.BGOnly)
            {
                await Task.Run(() => GlobalConfig.JobManager.StopCollectionUpdatingAsync());
            }
        }
        #endregion
        public void SetImageAsFavorite()
        {
            ImageModel currentImage = _fileCollection.CurrentWallpaper;
            DataKeeper.AddFavoriteImage(currentImage);

        }
        public async Task SetImageAsHated()
        {
            ImageModel currentImage = _fileCollection.CurrentWallpaper;
            DataKeeper.AddHatedImage(currentImage);
            await StopBackGroundRefresh();



        }
        public bool IsFavorited()
        {

            List<ImageModel> favImages = _fileCollection.FavoriteImages;
            ImageModel currentImage = _fileCollection.CurrentWallpaper;
            foreach (ImageModel i in favImages)
            {
                if (i.Name == currentImage.Name)
                {
                    return true;
                }
            }
            return false;
        }
        public bool InterestExists(string interest)
        {
            if (interest.GetInterestByName() != null)
            {
                return true;
            }
            return false;
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
            RefreshInterestList();
        }
    }
}
