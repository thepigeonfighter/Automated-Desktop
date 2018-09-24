using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundLibrary.Utility
{
    public class ImageFileUpdater
    {
        protected static void SetIsDownloadedToFalse(List<ImageModel> images)
        {
            List<ImageModel> updatedImages = new List<ImageModel>();
            foreach (ImageModel i in images)
            {
               
                i.IsDownloaded = false;
            }
        }

    }
}
