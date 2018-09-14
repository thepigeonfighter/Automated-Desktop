using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AutomatedDesktopBackgroundLibrary
{
    public enum PageRefreshState { BGOnly, ColOnly, BGAndCol, None}
    public enum ButtonCommands { StartCollections, StartBackground, StopCollections,StopBackground, SetToStartState}
    public class MainViewController 
    {
        public event EventHandler<string> OnPageStateChange;
        public PageRefreshState refreshState;
        APIManager manager = new APIManager();
        ImageFileManager fileManager = new ImageFileManager();
        public BindingList<InterestModel> interests = new BindingList<InterestModel>();
        private bool IsDownloading = false;
        public MainViewController() {

            GlobalConfig.EventSystem.DownloadCompleteEvent += EventSystem_DownloadCompleteEvent;
            GlobalConfig.EventSystem.ApplicationResetEvent += EventSystem_ApplicationResetEvent;
            if(GetCurrentWallPaperFromFile().Id !=-1 && GlobalConfig.CurrentWallpaper == null)
            {
                GlobalConfig.CurrentWallpaper = GetCurrentWallPaperFromFile();
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
                    if(refreshState == PageRefreshState.ColOnly)
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
            RefreshInterestList();
        }

        public ImageModel GetCurrentWallPaperFromFile()
        {
            if (File.Exists(GlobalConfig.CurrentWallpaperFile))
            {
                return TextConnectorProcessor.LoadFromTextFile<ImageModel>(GlobalConfig.CurrentWallpaperFile).First();
            }
            ImageModel noImage = new ImageModel() { Id = -1 };
            return noImage;
        }
        private void EventSystem_DownloadCompleteEvent(object sender, bool e)
        {
            IsDownloading = false;
        }

        public async Task AddInterest(string interest)
        {       
                List<InterestModel> results = await Task.Run(()=>InterestHelper.CreateInterest(interest));
                interests = new BindingList<InterestModel>(results);
               
        }
        public async Task RemoveInterest(string interest)
        {
            //TODO make sure this checks that the current image being displayed is trying to be deleted
            await Task.Run(()=>fileManager.RemoveImagesByInterestAsync(interest));
            RefreshInterestList();
        }

        public void RefreshInterestList()
        {
           var tempList = TextConnectorProcessor.LoadFromTextFile <InterestModel>( GlobalConfig.InterestFile);
            interests = new BindingList<InterestModel>(tempList);
        }
        public void DownloadNewCollection(string query)
        {
            
            if (!IsDownloading)
            {
                GlobalConfig.EventSystem.InvokeStartedDownloadingEvent();
                manager.GetImagesBySearch(query, true);
                IsDownloading = true;
                
            }
        }
        public bool AreAnyImagesDownloaded()
        {
            List<ImageModel> images = TextConnectorProcessor.LoadFromTextFile<ImageModel>(GlobalConfig.ImageFile);
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
        public async Task StartBackGroundRefresh( )
        {
            if (refreshState == PageRefreshState.None || refreshState == PageRefreshState.ColOnly)
            {
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
        public bool SetImageAsFavorite()
        {
            if (GlobalConfig.CurrentWallpaper != null)
            {
                fileManager.LikeImage(GlobalConfig.CurrentWallpaper);
                return true;
            }
            else
            {
                ImageModel currentWallpaper = GetCurrentWallPaperFromFile();
                if(currentWallpaper.Id != -1)
                {
                    fileManager.LikeImage(currentWallpaper);
                    return true;
                }
            }
            return false;
        }
        public async Task SetImageAsHated()
        {
            if (GlobalConfig.CurrentWallpaper != null)
            {
              await Task.Run(()=>  fileManager.HateImage(GlobalConfig.CurrentWallpaper));
            }
            else
            {
                ImageModel currentWallpaper = GetCurrentWallPaperFromFile();
                if (currentWallpaper.Id != -1)
                {
                    await Task.Run(()=>fileManager.HateImage(currentWallpaper));
                }
            }
        }
        public bool IsFavorited()
        {
            List<ImageModel> favImages = fileManager.GetAllFavoritedImages();
            foreach(ImageModel i in favImages)
            {
                if(i.Name == GlobalConfig.CurrentWallpaper.Name)
                {
                    return true;
                }
            }
            return false;
        }
        public bool InterestExists(string interest)
        {
            
            return fileManager.InterestExists(interest);
        }

        public int GetTotalImagesByInterestName(string interestName)
        {
            InterestModel interest = interests.FirstOrDefault(x => x.Name == interestName);
            return interest.TotalImages;
        }

    }
}
