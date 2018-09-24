using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundLibrary
{
    public class FileCollection : IFileCollection
    {
        public List<ImageModel> AllImages { get; set; } = new List<ImageModel>();
        public List<ImageModel> FavoriteImages { get; set;} = new List<ImageModel>();
        public List<ImageModel> HatedImages { get; set; } = new List<ImageModel>();
        public ImageModel CurrentWallpaper { get; set; } = new ImageModel();
        public List<InterestModel> AllInterests { get; set; } = new List<InterestModel>();
    }
}
