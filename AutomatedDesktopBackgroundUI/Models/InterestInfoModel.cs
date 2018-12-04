using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundUI.Models
{
    public class InterestInfoModel
    {
        public string Name { get; set; }
        public int Results { get; set; }
        public int LikedImages { get; set; }
        public int HatedImages { get; set; }
        public int CurrentlyDownloaded { get; set; }
        public int PercentageViewed { get; set; }
        public int ImagesViewed { get; set; }
    }
}
