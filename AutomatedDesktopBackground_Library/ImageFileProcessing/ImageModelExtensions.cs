using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AutomatedDesktopBackgroundLibrary.ImageFileProcessing
{
    public static class ImageModelExtensions
    {
        public static List<ImageModel> GetAllDownloadedImages(this List<ImageModel> entries)
        {
            return entries.Where(x => x.IsDownloaded).ToList();
        }

        public static ImageModel GetWallpaper(this List<ImageModel> entries)
        {
            return entries.FirstOrDefault(x => x.IsWallpaper);
        }

        public static List<ImageModel> AllImagesByInterest(this List<ImageModel> entries, InterestModel interest)
        {
            List<ImageModel> output = new List<ImageModel>();
            try
            {
                output = entries.Where(x => x.InterestId == interest.Id).ToList();
            }
            catch
            {
                Debug.WriteLine("There were no images associated with that interest");
            }
            return output;
        }
    }
}