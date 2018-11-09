﻿using AutomatedDesktopBackgroundLibrary;
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
            WindowManager.RegisterWindow(this);
            Window window = new MainWindow();
            GlobalConfig.EventSystem.UpdateBackgroundEvent += BackgroundUpdating;
            log.Debug(" registered/ creating main window");
            log.Debug(" perfoming startup update check");
            Task.Run(()=>CheckForUpdates());
            Hide();
            log.Debug("hiding root window/showing main window");
            window.Show();
            StartWatchingForWallpaperChanges();
           
        }

        private void BackgroundUpdating(object sender, string e)
        {
            Task.Run(() => CheckForUpdates());
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
                    
                    ReleaseEntry release = await manager.Result.UpdateApp();
                    System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                    FileVersionInfo info = FileVersionInfo.GetVersionInfo(assembly.Location);
                    if(info.FileVersion.ToSemanticVersion()< release.Version)
                    {
                        UpdateManager.RestartApp();
                    }
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
