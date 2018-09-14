using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.ComponentModel;

namespace AutomatedDesktopBackgroundLibrary
{
    /// <summary>
    /// Picks a random image from the entire library of downloaded images
    /// </summary>
    public class BackGroundPicker
    {
       public void PickRandomBackground()
        {
            List<ImageModel> downloadedImages = TextConnectorProcessor.LoadFromTextFile<ImageModel>(GlobalConfig.ImageFile).Where(x=> x.IsDownloaded == true).ToList();
            List<ImageModel> favImages = TextConnectorProcessor.LoadFromTextFile<ImageModel>(GlobalConfig.FavoritesFile);
            favImages?.ForEach(x => downloadedImages.Add(x));
            if (downloadedImages.Count > 0)
            {
                Random r = new Random();

                ImageModel randomImage = downloadedImages[r.Next(downloadedImages.Count)];
                downloadedImages.Clear();
                downloadedImages.Add(randomImage);
                TextConnectorProcessor.SaveToTextFile(downloadedImages, GlobalConfig.CurrentWallpaperFile);
                GlobalConfig.CurrentWallpaper = randomImage;
                WallpaperSetter.Set(randomImage.FileDir, WallpaperSetter.Style.Stretched);
            }
            else
            {
                throw new Exception("There are no images downloaded, Please Download some images and try again");
            }

        }
        
    }
}
