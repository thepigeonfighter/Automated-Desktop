using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomatedDesktopBackgroundLibrary.StringExtensions;
namespace AutomatedDesktopBackgroundLibrary
{
    public class ImageModelBuilder
    {
        public ImageModel Build(string imageUrl, string localUrl, int interestId)
        {
            ImageModel image = new ImageModel(); 
            image.Name = imageUrl.GetImageFileName();
            image.Url = imageUrl;
            image.LocalUrl = $@"{localUrl}\{image.Name}";
            image.InterestId = interestId;
            return image;
        }
    }
}
