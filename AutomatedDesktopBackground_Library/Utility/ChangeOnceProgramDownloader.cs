using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundLibrary.Utility
{
    public class ChangeOnceProgramDownloader
    {
        public EventHandler<string> DownloadProgressReport;
        public EventHandler<string> InstallationComplete;
        private static string tempRootDir = InternalFileDirectorySystem.ApplicationDirectory + @"\TempAssets";
        private static string zipDir = tempRootDir + @"\Assets.zip";
        private static string extractDir = tempRootDir + @"\extract";
        private static string setupExe = extractDir + @"\ChangeBackgroundOnce-1.2\Setup.exe";

        public void  DownloadApplication()
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var webClient = new WebClient();
                var assetURL = "https://github.com/thepigeonfighter/ChangeBackgroundOnce/archive/v1.2.zip";
                Directory.CreateDirectory(tempRootDir);
                using (webClient)
                {
                   
                    webClient.DownloadFileAsync(new Uri(assetURL), zipDir);
                    webClient.DownloadFileCompleted += DownloadComplete;
                    webClient.DownloadProgressChanged += DownloadProgress;
                   
                }
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.InnerException.Message);
            }

        }

        private void DownloadProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            string progress = $"Progress => {e.ProgressPercentage.ToString()}/100";
            DownloadProgressReport?.Invoke(this,progress);
        }

        private void DownloadComplete(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                UnZipResults();
            }
            else
            {
                System.Windows.MessageBox.Show("Your error is here");
              //  CustomMessageBox.Show("Something went wrong when downloading Change Background Once EXE");
            }
        }
        private void UnZipResults()
        {
            if (Directory.Exists(extractDir)) { Directory.Delete(extractDir, true); }
            ZipFile.ExtractToDirectory(zipDir, extractDir);
            InstallFile();
        }
        private void InstallFile()
        {
            try
            {
                Process.Start(setupExe);
                Task.Delay(1000).ContinueWith((x) => InstallationComplete?.Invoke(this, "Success"));
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.InnerException.Message);
            }
        }

    }
}
