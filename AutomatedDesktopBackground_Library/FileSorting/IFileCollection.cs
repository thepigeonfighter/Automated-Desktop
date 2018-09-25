using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundLibrary
{
    public interface IFileCollection
    {
        List<ImageModel> AllImages { get; set; }
        List<ImageModel> FavoriteImages { get; set; }
        List<ImageModel> HatedImages { get; set; }
        ImageModel CurrentWallpaper { get; set; }
        List<InterestModel> AllInterests { get; set; }
    }
}
