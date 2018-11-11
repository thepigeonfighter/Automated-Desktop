using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundLibrary.Utility
{
    
    public class CustomMessageBox
    {
        public static void Show(string _message)
        {
            GlobalConfig.EventSystem.InvokeShowCustomMessageBoxEvent(_message);
        }

    }
}
