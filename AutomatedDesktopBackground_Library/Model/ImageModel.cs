using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundLibrary
{
    public class ImageModel:IData
    {
        public int Id { get; set; }
        public string FileDir { get; set; }
        public string Name { get; set; }
        public int InterestId { get; set; }
        public bool IsDownloaded { get; set; }
    }
}
