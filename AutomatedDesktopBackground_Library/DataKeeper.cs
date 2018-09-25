using System;
using System.Collections.Generic;
using System.Linq;

namespace AutomatedDesktopBackgroundLibrary
{
    public static class DataKeeper
    {
        private static readonly IDataStorage _database = new DataStorageBuilder().Build(Database.Textfile);
        private static readonly FileCleaner _fileCleaner = new FileCleaner();
        private  static bool FileCleanInProgress = false;
        #region Image Functions

        public static ImageModel AddImage(ImageModel entry)
        {
            _database.ImageFileProcessor.CreateEntry(entry);
            return entry;
        }

        public static void OverwriteImageFile(List<ImageModel> images)
        {
            _database.ImageFileProcessor.OverwriteEntries(images);
        }

        public static void DeleteImage(ImageModel entry)
        {
            _database.ImageFileProcessor.DeleteEntry(entry);
        }

        public static void UpdateImage(List<ImageModel> entries)
        {
            _database.ImageFileProcessor.UpdateEntries(entries);
        }

        public static void UpdateImage(ImageModel entry)
        {
            _database.ImageFileProcessor.UpdateEntries(entry);
        }

        #endregion Image Functions

        #region Favorite Image Functions

        public static void AddFavoriteImage(ImageModel entry)
        {
            _database.FavoritedImageFileProcessor.CreateEntry(entry);
        }

        public static void DeleteFavoriteImage(ImageModel entry)
        {
            _database.FavoritedImageFileProcessor.DeleteEntry(entry);
        }

        public static void UpdateFavoriteImage(List<ImageModel> entries)
        {
            _database.FavoritedImageFileProcessor.UpdateEntries(entries);
        }

        public static void UpdateFavoriteImage(ImageModel entry)
        {
            _database.FavoritedImageFileProcessor.UpdateEntries(entry);
        }

        public static void OverwriteFavoriteImages(List<ImageModel> entries)
        {
            _database.FavoritedImageFileProcessor.OverwriteEntries(entries);
        }

        #endregion Favorite Image Functions

        #region Hated Image Functions

        public static void AddHatedImage(ImageModel entry)
        {
            ImageModel favoriteImage = GetFileSnapShot()
                .FavoriteImages
                .FirstOrDefault(x => x.Name == entry.Name);
            if (favoriteImage == null)
            {
                DeleteImage(entry);
            }
            else
            {
                DeleteFavoriteImage(entry);
            }
            DeleteDownloadedImageFile(entry);
            _database.HatedImageFileProcessor.CreateEntry(entry);
        }

        public static void UpdateHatedImage(ImageModel entry)
        {
            _database.HatedImageFileProcessor.UpdateEntries(entry);
        }

        #endregion Hated Image Functions

        #region Interest Model Functions

        public static void AddInterest(InterestModel interest)
        {
            _database.InterestFileProcessor.CreateEntry(interest);
        }

        public static void DeleteInterest(InterestModel interest)
        {
            _database.InterestFileProcessor.DeleteEntry(interest);
        }

        public static void UpdateInterest(List<InterestModel> interests)
        {
            _database.InterestFileProcessor.UpdateEntries(interests);
        }

        public static void UpdateInterest(InterestModel interest)
        {
            _database.InterestFileProcessor.UpdateInterest(interest);
        }

        #endregion Interest Model Functions

        #region File Functions

        public static IFileCollection GetFileSnapShot()
        {
            return _database.FileCollection;
        }

        public static void DeleteDownloadedImageFile(ImageModel image)
        {
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

        public static void UpdateWallpaper(string url)
        {
            IFileCollection fileCollection = GetFileSnapShot();
            List<ISaveable> saveables = fileCollection.GetAllImageEntries().GetResults();
            List<ImageModel> images = saveables.ConvertAll(x => (ImageModel)x);
            ImageModel image = images.FirstOrDefault(x => x.LocalUrl == url);
            if (image != null)
            {
                _database.WallPaperFileProcessor.Update(image);
            }
            else
            {

                    _fileCleaner.Run(GetFileSnapShot());
                

            }
        }

    }
}