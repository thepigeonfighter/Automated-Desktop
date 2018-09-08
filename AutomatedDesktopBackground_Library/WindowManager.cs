using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AutomatedDesktopBackgroundLibrary
{
    public static class WindowManager
    {
        static Window rootWindow;
        public static void RegisterWindow(Window window)
        {
            rootWindow = window;
        }
        public static void CloseRootWindow()
        {
            rootWindow.Close();
        }
    }
}
