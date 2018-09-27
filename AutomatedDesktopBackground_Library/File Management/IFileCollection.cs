using System.Collections.Generic;

namespace AutomatedDesktopBackgroundLibrary
{
    public interface IFileCollection
    {
        List<ImageModel> AllImages { get; set; }
        List<InterestModel> AllInterests { get; set; }
        ImageModel CurrentWallpaper { get; set; }
    }
}