using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AutomatedDesktopBackgroundLibrary
{
    public static class DataKeeper
    {
        private static readonly IDataStorage _database = new DataStorageBuilder().Build(Database.JsonFile);

        #region Image Functions

        public static ImageModel AddImage(ImageModel entry)
        {
            _database.ImageFileProcessor.CreateEntry(entry);
            return entry;
        }

        public static void DeleteImageAndImageInfoEntry(ImageModel entry)
        {
            _database.ImageFileProcessor.DeleteEntry(entry);
            DeleteImage(entry, true);
        }

        #endregion Image Functions

        #region Interest Model Functions

        public static void AddInterest(InterestModel interest)
        {
            _database.InterestFileProcessor.CreateEntry(interest);
        }

        public static void DeleteInterest(InterestModel interest)
        {
            _database.InterestFileProcessor.DeleteEntry(interest);
            _database.ImageFileProcessor.RemoveAllImagesByInterest(interest);
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
                //signals that a file was found in the collection that has no records may need to invent a file cleaner
            }
        }
        #endregion
    }
}