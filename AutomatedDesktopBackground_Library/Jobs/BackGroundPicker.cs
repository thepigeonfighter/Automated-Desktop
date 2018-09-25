using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.ComponentModel;
using System.IO;
using System.Windows;

namespace AutomatedDesktopBackgroundLibrary
{
    /// <summary>
    /// Picks a random image from the entire library of downloaded images
    /// </summary>
    public class BackGroundPicker
    {
       public void PickRandomBackground()
        {
            if (!GlobalConfig.InCollectionRefresh)
            {


               string imageUrl = GetImageFileDir();
                DataKeeper.UpdateWallpaper(imageUrl);    
               WallpaperSetter.Set(imageUrl, WallpaperSetter.Style.Stretched);
                    
            }

        }
        private string GetImageFileDir()
        {
            string[] directories = Directory.GetDirectories(StringExtensions.StringExtensions.GetApplicationDirectory());
            List<string> imageFilePaths = new List<string>();
            foreach(string dir in directories)
            {
                string[] fileDir = Directory.GetFiles(dir, "*.jpeg");
                imageFilePaths.AddRange(fileDir);
            }
            Random r = new Random();
            string imageUrl = imageFilePaths[r.Next(0, imageFilePaths.Count)];
            return imageUrl;

        }
        
    }
}
