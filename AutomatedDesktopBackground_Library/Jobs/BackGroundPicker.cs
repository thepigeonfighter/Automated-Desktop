using System;
using System.Collections.Generic;
using System.IO;

namespace AutomatedDesktopBackgroundLibrary
{
    /// <summary>
    /// Picks a random image from the entire library of downloaded images
    /// </summary>
    public class BackGroundPicker
    {
        
        public void PickRandomBackground(bool inAppRequest)
        {
            if (inAppRequest)
            {
                if (!GlobalConfig.InCollectionRefresh)
                {
                    string imageUrl = GetImageFileDir();
                    DataKeeper.UpdateWallpaper(imageUrl);
                    WallpaperSetter.Set(imageUrl, WallpaperSetter.Style.Fit);
                }
            }
            else
            {
                string imageUrl = GetImageFileDir();
                DataKeeper.UpdateWallpaper(imageUrl);
                WallpaperSetter.Set(imageUrl, WallpaperSetter.Style.Fit);
                string[] lines = new string[]{$"{imageUrl}","This is a temp file", "that alerts a file watcher " , "To update the application data"};
                File.WriteAllLines(InternalFileDirectorySystem.WallpaperCacheFile, lines);
            }
        }

        private string GetImageFileDir()
        {
            string[] directories = Directory.GetDirectories(InternalFileDirectorySystem.ImagesFolder);
            List<string> imageFilePaths = new List<string>();
            foreach (string dir in directories)
            {
                string[] fileDir = Directory.GetFiles(dir, "*.jpeg");
                imageFilePaths.AddRange(fileDir);
            }
            ImageModel currentWallpaper = DataKeeper.GetFileSnapShot().CurrentWallpaper;
            if (currentWallpaper != null)
            {
                string currentWallpaperDir = imageFilePaths.Find(x => x.Equals(currentWallpaper.LocalUrl));
                if (!String.IsNullOrWhiteSpace(currentWallpaperDir))
                {
                    imageFilePaths.Remove(currentWallpaperDir);
                }
            }
            Random r = new Random();
            string imageUrl = imageFilePaths[r.Next(0, imageFilePaths.Count)];
            return imageUrl;
        }
    }
}