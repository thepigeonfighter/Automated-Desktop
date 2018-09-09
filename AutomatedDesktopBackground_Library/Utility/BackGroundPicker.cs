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
            List<ImageModel> images = TextConnectorProcessor.LoadFromTextFile<ImageModel>(GlobalConfig.ImageFile);
            List<ImageModel> favImages = TextConnectorProcessor.LoadFromTextFile<ImageModel>(GlobalConfig.FavoritesFile);
            favImages?.ForEach(x => images.Add(x));
            if (images.Count > 0)
            {
                Random r = new Random();

                ImageModel randomImage = images[r.Next(images.Count)];
                images.Clear();
                images.Add(randomImage);
                TextConnectorProcessor.SaveToTextFile(images, GlobalConfig.CurrentWallpaperFile);
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
