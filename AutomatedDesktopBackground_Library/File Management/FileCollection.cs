using System.Collections.Generic;

namespace AutomatedDesktopBackgroundLibrary
{
    public class FileCollection : IFileCollection
    {
        public List<ImageModel> AllImages { get; set; }
        public List<InterestModel> AllInterests { get; set; }
        public ImageModel CurrentWallpaper { get; set; }
    }
}