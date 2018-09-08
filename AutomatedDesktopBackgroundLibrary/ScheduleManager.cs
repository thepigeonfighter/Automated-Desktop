using System;
using System.Collections.Generic;
using System.Text;

namespace AutomatedDesktopBackgroundLibrary
{
    public class ScheduleManager
    {
        public string ImageFilePath { get; set; }
        public DateTime RefreshFrequency { get; set; }
        public string CurrentImage { get; set; }
        public int CurrentImageId { get; set; }
    }
}
