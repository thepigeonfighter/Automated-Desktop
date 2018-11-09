using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AutomatedDesktopBackgroundLibrary;
using Squirrel;
namespace ChangeBackgroundOnce
{
    class Program
    {
        static void Main(string[] args)
        {
            
            List<ImageModel> images = DataKeeper.GetFreshFileSnapShot().AllImages;
            if (images.Count > 0)
            {
                BackGroundPicker backGroundPicker = new BackGroundPicker();
                backGroundPicker.PickRandomBackground(false);
                
            }
            CheckForUpdates().Wait();
        }
        private static async Task CheckForUpdates()
        {
            string path = @"https://github.com/thepigeonfighter/ChangeBackgroundOnce";
            try
            {
                using (var manager = UpdateManager.GitHubUpdateManager(path))
                {
                    
                    await manager.Result.UpdateApp();
                    manager.Result.RemoveShortcutForThisExe();
                    manager.Result.Dispose();
                    
                }
            }
            catch (Exception e)
            {
                string text = $"Failed to update change background once. This is why {e.InnerException.Message} ";
                string filepath = InternalFileDirectorySystem.ApplicationDirectory + @"/Logs/ChangeOnceUpdateError.txt";
                File.WriteAllText(filepath, text);
            }
        }
    }
}
