using AutomatedDesktopBackgroundLibrary;
using AutomatedDesktopBackgroundLibrary.File_Management;
using log4net;
using Squirrel;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace AutomatedDesktopBackgroundUI
{
    /// <summary>
    /// Interaction logic for RootWindow.xaml
    /// </summary>
    public partial class RootWindow : Window
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly WallpaperChangeWatcher watcher = new WallpaperChangeWatcher();
        public RootWindow()
        {
                        
            InitializeComponent();
            Window mainWindow = new MainWindow();
            GlobalConfig.EventSystem.ShowCustomMessageBoxEvent += EventSystem_ShowCustomMessageBoxEvent;
            log.Debug(" registered/ creating main window");
            log.Debug(" perfoming startup update check");
            Task.Run(()=>CheckForUpdates()).ConfigureAwait(false);
            Hide();
            log.Debug("hiding root window/showing main window");
            mainWindow.Show();
            StartWatchingForWallpaperChanges();
           
        }

        private void EventSystem_ShowCustomMessageBoxEvent(object sender, string e)
        {
            Window warningWindow =this.Dispatcher.Invoke(()=>new WarningWindow(e));
            this.Dispatcher.Invoke(()=>warningWindow.Show());
        }

        private void StartWatchingForWallpaperChanges()
        {
            try
            {
                watcher.StartWatchingWallpaperFile();
                log.Debug("Starting watching wallpaper file");
            }
            catch(Exception e)
            {
                log.Error("Failed to start watching wallpaper file");
                log.Info(e.InnerException.Message);
            }
        }


        private async Task CheckForUpdates()
        {
            string path = @"https://github.com/thepigeonfighter/Automated-Desktop";
            try
            {
                using (var manager = UpdateManager.GitHubUpdateManager(path))
                {
                    log.Debug("succesfully created update client, now awaiting result");
                    await manager.Result.UpdateApp();
                    manager.Result.Dispose();
                    log.Info("sucessfully handed off the update results and have disposed the update manager");
                }
            }
            catch(Exception e)
            {
                log.Error("The update manager was not created sucessfully");
                log.Info($"{e.InnerException.Message.ToString()}");
            }

        }


    }
}
