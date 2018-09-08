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
        public void RemoveInterest(string interest)
        {
            //TODO make sure this checks that the current image being displayed is trying to be deleted
            fileManager.RemoveImagesByInterest(interest);
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
                manager.GetImagesBySearch(query, 1);
                IsDownloading = true;
                
            }
        }

        public void CloseProgram()
        {
            WindowManager.CloseRootWindow();
        }
        public async Task  StartBackGroundRefresh()
        {      
          await Task.Run(()=> GlobalConfig.JobManager.UpdateBackGroundAsync());
        }
        public async Task StopBackGroundRefresh()
        {
            
            await Task.Run(()=> GlobalConfig.JobManager.StopBackgroundChange());

        }
        public async Task StopCollectionChange()
        {
            if (GlobalConfig.CollectionUpdating)
            {
                 await Task.Run(()=>GlobalConfig.JobManager.StopCollectionChange());
            }
        }

    }
}
