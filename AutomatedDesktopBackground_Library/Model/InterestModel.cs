using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundLibrary
{
    public class InterestModel:IData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TotalImages { get; set; }
        public int TotalPages { get; set; }
        public bool EntireCollectionDownloaded { get; set; }
    }
}
