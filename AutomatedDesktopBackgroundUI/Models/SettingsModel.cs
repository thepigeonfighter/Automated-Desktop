using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundUI.Models
{
    public class SettingsModel
    {
        public TimeSpan CollectionRefreshTime { get; set; }
        public TimeSpan BackgroundRefreshTime { get; set; }
        public bool EnableContextMenuButton { get; set; }
        public bool ShowSettingsWindowOnLoad { get; set; }
        public bool ShowWarningOnWindowClose { get; set; }
    }
}
