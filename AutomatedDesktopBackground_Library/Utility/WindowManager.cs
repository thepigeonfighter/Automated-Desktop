using System.Windows;
namespace AutomatedDesktopBackgroundLibrary
{
    /// <summary>
    /// Keeps track of the invisible root window that way the application knows to close only when the root window has been closed
    /// </summary>
    public static class WindowManager
    {
        private static Window rootWindow;

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