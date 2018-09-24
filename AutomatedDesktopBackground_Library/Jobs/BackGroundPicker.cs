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

                    Random r = new Random();

                    ImageModel randomImage = GetImage();
                    DataKeeper.UpdateWallpaper(randomImage);
                    WallpaperSetter.Set(randomImage.LocalUrl, WallpaperSetter.Style.Stretched);
                    
            }

        }
        private ImageModel GetImage()
        {
            IFileCollection fileCollection = DataKeeper.GetFileSnapShot();
            ImageModel randomImage = new ImageModel();
            if (fileCollection.AllImages.Count > 1)
            {
                IFilteredFileResult results = fileCollection.GetAllDownloadedImages();
                List<ImageModel> downloadedImages = results.GetResults().ConvertAll(x => (ImageModel)x);
                ImageModel currentWallper = fileCollection.CurrentWallpaper;
                //Gets the instance not the copy
                if (currentWallper != null)
                {
                    ImageModel imageThatIsWallpaper = downloadedImages.FirstOrDefault(x => x.LocalUrl == currentWallper.LocalUrl);
                    //To ensure we aren't setting the wallpaper to ths same file
                    downloadedImages.Remove(imageThatIsWallpaper);
                }

                Random r = new Random();
               randomImage = downloadedImages[r.Next(downloadedImages.Count)];
                return randomImage;
            }
            else
            {
                throw  new Exception("No Images Downloaded");
            }
            
        }
        
    }
}
