using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundLibrary
{
    public static class DataKeeper
    {
        private static IDataStorage _database = new DataStorageBuilder().Build(Database.Textfile);
        public static void UpdateAllLists()
        {
            _database.UpdateAllLists();
        }
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
        public static void AddHatedImage(ImageModel entry)
        {
            _database.HatedImageFileProcessor.CreateEntry(entry);
        }
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
        public static IFileCollection GetFileSnapShot()
        {
            return _database.FileCollection;
        }
        public static void DeleteDownloadedImageFile(ImageModel image)
        {
            _database.Database.DeleteFile(image.LocalUrl);
        }
        public static void UpdateWallpaper(ImageModel image)
        {
            _database.WallPaperFileProcessor.Update(image);
        }
        public static void RegisterFileListener(IFileListener fileListener)
        {
            _database.RegisterFileListeners(fileListener);
        }
        public static void ResetApplication()
        {
            _database.ResetApplication();
        }
    }
}
