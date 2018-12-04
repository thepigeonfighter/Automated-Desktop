using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AutomatedDesktopBackgroundLibrary
{
    public class DataKeeper : IDataKeeper
    {
        private IDataStorage _dataStorageProcessor ;
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public DataKeeper(IDataStorage database)
        {
            _dataStorageProcessor = database;
        }

        #region Image Functions

        public ImageModel AddImage(ImageModel entry)
        {
            try
            {
                _dataStorageProcessor.ImageFileProcessor.CreateEntry(entry);
                log.Debug($"Have successfully added the image {entry.Name}");
            }
            catch(Exception ex)
            {
                log.Error($"Have failed to add the image {entry.Name}");
                log.Info(ex.InnerException.Message);
            }
            return entry;
        }

        public void DeleteImageAndImageInfoEntry(ImageModel entry)
        {
            try
            {
                _dataStorageProcessor.ImageFileProcessor.DeleteEntry(entry);
                DeleteImage(entry, true);
                log.Debug($"Deleted the image {entry.Name} ");
            }
            catch (Exception ex)
            {
                log.Error($"Have failed to delete the image {entry.Name}");
                log.Info(ex.InnerException.Message);
            }
        }

        #endregion Image Functions

        #region Interest Model Functions

        public void AddInterest(InterestModel interest)
        {
            try
            {
                _dataStorageProcessor.InterestFileProcessor.CreateEntry(interest);
                log.Debug($"Have added the interest {interest.Name}");
            }
            catch (Exception ex)
            {
                log.Error($"Have failed to add the interest{interest.Name}");
                log.Info(ex.InnerException.Message);
            }
        }

        public void DeleteInterest(InterestModel interest)
        {
            try
            {
                _dataStorageProcessor.InterestFileProcessor.DeleteEntry(interest);
                _dataStorageProcessor.ImageFileProcessor.RemoveAllImagesByInterest(interest, GetFileSnapShot());
                log.Debug($"Have deleted the interest{interest.Name}");
            }
            catch (Exception ex)
            {
                log.Error($"Have failed to delete the interest{interest.Name}");
                log.Info(ex.InnerException.Message);
            }
        }

        #endregion Interest Model Functions

        #region File Functions

        public IFileCollection GetFileSnapShot()
        {
            return _dataStorageProcessor.FileCollection;
        }
        public IFileCollection GetFreshFileSnapShot()
        {
            _dataStorageProcessor.UpdateAllLists();
            return _dataStorageProcessor.FileCollection;
        }

        public void DeleteImage(ImageModel image, bool KeepRecord)
        {
            if (KeepRecord)
            {
                image.IsDownloaded = false;
                AddImage(image);
            }
            _dataStorageProcessor.Database.DeleteFile(image.LocalUrl);
        }

        public void DeleteGroupOfFiles(List<ImageModel> images)
        {
            _dataStorageProcessor.Database.DeleteImages(images);
        }

        public void RegisterFileListener(IFileListener fileListener)
        {
            _dataStorageProcessor.RegisterFileListeners(fileListener);
        }

        public void ResetApplication()
        {
            _dataStorageProcessor.ResetApplication();
        }

        public void UpdateAllLists()
        {
            _dataStorageProcessor.UpdateAllLists();
        }

        #endregion File Functions

        #region Wallpaper Functions
        public void UpdateWallpaper(string url)
        {
            IFileCollection fileCollection = GetFileSnapShot();
            List<ImageModel> images = fileCollection.AllImages;
            ImageModel image = images.FirstOrDefault(x => x.LocalUrl == url);
            if (image != null)
            {
                _dataStorageProcessor.ImageFileProcessor.UpdateWallPaper(image, fileCollection.CurrentWallpaper);
            }
            else
            {
                Debug.WriteLine("Wallpaper file was not found in our records");
            }
        }
        #endregion
    }
}