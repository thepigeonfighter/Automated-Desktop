using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AutomatedDesktopBackgroundLibrary
{
    public static class DataKeeper
    {
        private static readonly IDataStorage _database = new DataStorageBuilder().Build(Database.JsonFile);
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Image Functions

        public static ImageModel AddImage(ImageModel entry)
        {
            try
            {
                _database.ImageFileProcessor.CreateEntry(entry);
                log.Debug($"Have successfully added the image {entry.Name}");
            }
            catch(Exception ex)
            {
                log.Error($"Have failed to add the image {entry.Name}");
                log.Info(ex.InnerException.Message);
            }
            return entry;
        }

        public static void DeleteImageAndImageInfoEntry(ImageModel entry)
        {
            try
            {
                _database.ImageFileProcessor.DeleteEntry(entry);
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

        public static void AddInterest(InterestModel interest)
        {
            try
            {
                _database.InterestFileProcessor.CreateEntry(interest);
                log.Debug($"Have added the interest {interest.Name}");
            }
            catch (Exception ex)
            {
                log.Error($"Have failed to add the interest{interest.Name}");
                log.Info(ex.InnerException.Message);
            }
        }

        public static void DeleteInterest(InterestModel interest)
        {
            try
            {
                _database.InterestFileProcessor.DeleteEntry(interest);
                _database.ImageFileProcessor.RemoveAllImagesByInterest(interest);
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

        public static IFileCollection GetFileSnapShot()
        {
            return _database.FileCollection;
        }
        public static IFileCollection GetFreshFileSnapShot()
        {
            _database.UpdateAllLists();
            return _database.FileCollection;
        }

        public static void DeleteImage(ImageModel image, bool KeepRecord)
        {
            if (KeepRecord)
            {
                image.IsDownloaded = false;
                AddImage(image);
            }
            _database.Database.DeleteFile(image.LocalUrl);
        }

        public static void DeleteGroupOfFiles(List<ImageModel> images)
        {
            _database.Database.DeleteImages(images);
        }

        public static void RegisterFileListener(IFileListener fileListener)
        {
            _database.RegisterFileListeners(fileListener);
        }

        public static void ResetApplication()
        {
            _database.ResetApplication();
        }

        public static void UpdateAllLists()
        {
            _database.UpdateAllLists();
        }

        #endregion File Functions

        #region Wallpaper Functions
        public static void UpdateWallpaper(string url)
        {
            IFileCollection fileCollection = GetFileSnapShot();
            List<ImageModel> images = fileCollection.AllImages;
            ImageModel image = images.FirstOrDefault(x => x.LocalUrl == url);
            if (image != null)
            {
                _database.ImageFileProcessor.UpdateWallPaper(image, fileCollection.CurrentWallpaper);
            }
            else
            {
                Debug.WriteLine("Wallpaper file was not found in our records");
            }
        }
        #endregion
    }
}