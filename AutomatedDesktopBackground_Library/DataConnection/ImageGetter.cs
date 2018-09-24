using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Text;
using System.Drawing;
using System.Windows;
using System.Drawing.Imaging;
using System.Linq;
using System.IO;
using AutomatedDesktopBackgroundLibrary.DataConnection;
using AutomatedDesktopBackgroundLibrary.StringExtensions;
namespace AutomatedDesktopBackgroundLibrary
{
    /// <summary>
    /// This class needs to get an image from online and then download it to a directory 
    /// </summary>
    public class ImageGetter:ImageGetterBase
    {

        private ImageModelBuilder imageBuilder = new ImageModelBuilder();
        /// <summary>
        /// Retrieves an image from the internet, and creates a new image entry in the database
        /// </summary>
        /// <param name="imageUrl">
        /// the download path that will be used to retrieve the image
        /// </param>
        /// 
        /// <param name="folderName">
        /// the name of the folder that will store the image</param>
        /// <param name="userRequested">
        /// represents whether the user has specifically requested this download or if it is a background operation
        /// </param>
        public void  GetImage(string imageUrl , string folderName, bool userRequested)
        {
            totalDownloadsRequested = ExpectedDownloadAmount;
            string downloadPath = folderName.CreateDirectory().FullName;
            InterestModel interest = folderName.GetInterestByName();
            ImageModel imageToDownload = imageBuilder.Build(imageUrl, downloadPath, interest.Id); 
            DownloadFile(imageToDownload);
            IsUserRequested = userRequested;
        }
        /// <summary>
        /// Retrieves an image from the internet, using an image entry that was previously downloaded
        /// </summary>
        /// <param name="imageUrl">
        /// the download path that will be used to retrieve the image
        /// </param>
        /// 
        /// <param name="folderName">
        /// the name of the folder that will store the image</param>
        /// <param name="userRequested">
        /// represents whether the user has specifically requested this download or if it is a background operation
        /// </param>
        public void GetImageLocal(string imageUrl, string downloadPath, bool userRequested)
        {
            totalDownloadsRequested = ExpectedDownloadAmount;
            string dirPath = Path.GetDirectoryName(downloadPath);
            Directory.CreateDirectory(dirPath);
            DownloadFileLocal(imageUrl, downloadPath);
            IsUserRequested = userRequested;
            isLocalGet = true;
        }

        /// <summary>
        /// Get the filename from a web url : 
        /// 
        /// www.google.com/image.png -> returns : image.png
        /// 
        /// </summary>
        /// <param name="hreflink"></param>
        /// <returns></returns>

        private void DownloadFileLocal(string imageUrl, string downloadPath)
        {
            if (!IsHated(imageUrl))
            {
                using (WebClient wc = new WebClient())
                {

                    wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                    wc.DownloadFileCompleted += wc_DownloadFileCompleted;
                    wc.DownloadFileAsync(new Uri(imageUrl), downloadPath);

                }
            }

        }
        /// <summary>
        /// Download a file asynchronously in the desktop path, show the download progress and save it with the original filename.
        /// </summary>
        private void DownloadFile(ImageModel image)
        {
            if (!IsHated(image.Url))
            {
                using (WebClient wc = new WebClient())
                {

                    wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                    wc.DownloadFileCompleted += wc_DownloadFileCompleted;
                    wc.DownloadFileAsync(new Uri(image.Url), image.LocalUrl);
                    images.Add(image);

                }
            }
                
            
        }
        private bool IsHated(string Url)
        {
            List<ImageModel> hatedImages = DataKeeper.GetFileSnapShot().HatedImages;
            if(hatedImages.Count == 0)
            {
                return false;
            }
            else
            {
                ImageModel hatedImage = hatedImages.FirstOrDefault(x => x.Url == Url);
                if(hatedImage != null)
                {
                    ExpectedDownloadAmount--;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///  Show the progress of the download in a progressbar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (IsUserRequested)
            {
                GlobalConfig.EventSystem.InvokePercentChangeEvent(e.ProgressPercentage);
            }
        }

        private void wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            // progressBar1.Value = 0;

            if (e.Cancelled)
            {

                HandleDownloadCancel();
            }

            if (e.Error != null) // We have an error! Retry a few times, then abort.
            {

                HandleError();
            }
            ExpectedDownloadAmount--;
            if (ExpectedDownloadAmount == 0)
            {

                HandleDownloadComplete();
                SubmitChanges();
            }
            else
            {
                HandleImageDownload();
            };


        }
        private void SubmitChanges()
        {
            if (images.Count > 0)
            {

                GiveEachImageAnId();
                if(errorIndex.Count > 0)
                {
                    RemoveCorruptedImages();
                    images = images.Where(x => x.IsDownloaded == true).ToList();
                }
                if (images.Count > 0)
                {

                    UpdateImageFile();
                    
                }
                DisplayDownloadCompletionMessage();
            }
        }


    }

    }
