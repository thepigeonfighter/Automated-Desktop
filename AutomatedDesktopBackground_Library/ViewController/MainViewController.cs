using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AutomatedDesktopBackgroundLibrary
{
    public class MainViewController 
    {
        APIManager manager = new APIManager();
        ImageFileManager fileManager = new ImageFileManager();
        public BindingList<InterestModel> interests = new BindingList<InterestModel>();
        private bool IsDownloading = false;
        public MainViewController() {

            GlobalConfig.EventSystem.DownloadCompleteEvent += EventSystem_DownloadCompleteEvent;
            if(GetCurrentWallPaperFromFile().Id !=-1 && GlobalConfig.CurrentWallpaper == null)
            {
                GlobalConfig.CurrentWallpaper = GetCurrentWallPaperFromFile();
            }
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

        public void AddInterest(string interest)
        {
            RefreshInterestList();
            int id = 1;
            if (interests.Count > 0)
            {
                id = interests.Max(x => x.Id) + 1;
            }
            InterestModel newInterest = new InterestModel() { Name = interest, Id = id };
            interests.Add(newInterest);

            TextConnectorProcessor.SaveToTextFile(interests.ToList(), GlobalConfig.InterestFile);
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

        public void CloseProgram()
        {
            WindowManager.CloseRootWindow();
        }
        public async Task  StartBackGroundRefresh()
        {      
          await Task.Run(()=> GlobalConfig.JobManager.StartBackgroundUpdatingAsync());
        }
        public async Task StopBackGroundRefresh()
        {
            
            await Task.Run(()=> GlobalConfig.JobManager.StopBackgroundUpdatingAsync());

        }
        public async Task StopCollectionChange()
        {
            if (GlobalConfig.CollectionUpdating)
            {
                 await Task.Run(()=>GlobalConfig.JobManager.StopCollectionUpdatingAsync());
            }
        }
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
        public async Task StartCollectionRefresh()
        {
            await Task.Run(() => GlobalConfig.JobManager.StartCollectionUpdatingAsync());
        }

    }
}
