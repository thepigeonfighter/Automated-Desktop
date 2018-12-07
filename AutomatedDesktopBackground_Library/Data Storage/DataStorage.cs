using AutomatedDesktopBackgroundLibrary.ImageFileProcessing;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundLibrary
{
    public class DataStorage : IDataStorage
    {
        public IImageFileProcessor ImageFileProcessor { get; set; }
        public IInterestFileProcessor InterestFileProcessor { get; set; }
        public IDatabaseConnector Database { get; set; }
        public IFileCollection FileCollection { get; set; }

        private readonly List<IFileListener> fileListeners = new List<IFileListener>();

        #region Update FileCollection Events

        public void WireUpEvents()
        {
            ImageFileProcessor.OnFileAltered += UpdateImageFileEvent;
            InterestFileProcessor.OnFileUpdate += UpdateInterestFileEvent;
            ImageFileProcessor.OnWallPaperUpdate += UpdateWallpaperEvent;
        }

        private void UpdateWallpaperEvent(object sender, ImageModel e)
        {
            FileCollection.CurrentWallpaper = e;
            Task.Run(() => UpdateFileListeners());
            GlobalConfig.EventSystem.InvokeUpdateBackroundEvent();
        }

        private void UpdateInterestFileEvent(object sender, List<InterestModel> e)
        {
            FileCollection.AllInterests = InterestFileProcessor.LoadAllEntries();
            Task.Run(() => UpdateFileListeners());
        }

        private void UpdateImageFileEvent(object sender, List<ImageModel> e)
        {
            FileCollection.AllImages = e;
            Task.Run(() => UpdateFileListeners());
        }

        public void RegisterFileListeners(IFileListener fileListener)
        {
            fileListeners.Add(fileListener);
        }

        private async Task UpdateFileListeners()
        {
            await Task.Delay(200).ConfigureAwait(false);
            fileListeners.ForEach(i => i.OnFileUpdate());
        }

        public void UpdateAllLists()
        {
            FileCollection.AllImages = ImageFileProcessor.LoadAllEntries();
            FileCollection.AllInterests = InterestFileProcessor.LoadAllEntries();
            FileCollection.CurrentWallpaper = FileCollection.AllImages.GetWallpaper();
            Task.Run(() => UpdateFileListeners());
        }

        public void ResetApplication()
        {
            Database.DeleteAllFiles();
            UpdateAllLists();
        }

        #endregion Update FileCollection Events
    }
}