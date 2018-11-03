﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;

namespace AutomatedDesktopBackgroundLibrary.File_Management
{
    public class WallpaperChangeWatcher
    {
        private FileSystemWatcher _watcher = new FileSystemWatcher();
        private int writeCounter = 0;
        public void StartWatchingWallpaperFile()
        {
            string opener = "---------------This is the wallpaper watcher file--------------------------";
            File.WriteAllText(InternalFileDirectorySystem.WallpaperCacheFile, opener);
            _watcher.Path = Path.GetDirectoryName(InternalFileDirectorySystem.WallpaperCacheFile);
            _watcher.Filter = "*.temp";
            _watcher.Changed += OnWallpaperUpdated;
            _watcher.EnableRaisingEvents = true;
        }

        private void OnWallpaperUpdated(object sender, FileSystemEventArgs e)
        {
            writeCounter++;
            if (writeCounter == 2)
            {
                try
                {
                    string[] lines = File.ReadAllLines(InternalFileDirectorySystem.WallpaperCacheFile);
                    DataKeeper.UpdateWallpaper(lines[0]);
                }
                catch
                {
                    MessageBox.Show("Failed to update wallpaper cache file");
                }
                finally
                {
                    writeCounter = 0;
                }
            }
        }
    }
}
