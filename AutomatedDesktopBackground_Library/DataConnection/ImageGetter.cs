using AutomatedDesktopBackgroundLibrary.DataConnection;
using AutomatedDesktopBackgroundLibrary.StringExtensions;
using AutomatedDesktopBackgroundLibrary.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;

namespace AutomatedDesktopBackgroundLibrary
{
    /// <summary>
    /// This class needs to get an image from online and then download it to a directory
    /// </summary>
    public class ImageGetter : ImageGetterBase
    {
        private readonly ImageModelBuilder _imageBuilder = new ImageModelBuilder();

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
        public void GetImage(string imageUrl, string folderName, bool userRequested)
        {
            totalDownloadsRequested = ExpectedDownloadAmount;
            // string downloadPath = folderName.CreateDirectory().FullName;

            InterestModel interest = folderName.GetInterestByName();
            Directory.CreateDirectory($@"{InternalFileDirectorySystem.ImagesFolder}\{interest.Name}");
            ImageModel imageToDownload = _imageBuilder.Build(imageUrl, interest);
            DownloadFile(imageToDownload);
            _IsUserRequested = userRequested;
        }

        /// <summary>
        /// Retrieves an image from the internet, using an image entry that was previously downloaded
        /// </summary>
        /// <param name="imageUrl">
        /// the download path that will be used to retrieve the image
        /// </param>
        /// <param name="downloadPath">
        /// the path to which the image will be saved
        /// </param>
        ///
        /// <param name="folderName">
        /// the name of the folder that will store the image</param>
        /// <param name="image">
        /// </param>
        /// <param name="userRequested">
        /// represents whether the user has specifically requested this download or if it is a background operation
        /// </param>
        public void GetImageLocal(ImageModel image, bool userRequested)
        {
            totalDownloadsRequested = ExpectedDownloadAmount;
            string dirPath = Path.GetDirectoryName(image.LocalUrl);
            Directory.CreateDirectory(dirPath);
            DownloadFileLocal(image);
            _IsUserRequested = userRequested;
            isLocalGet = true;
        }

        /// <summary>
        /// <para>Get the filename from a web url :</para>
        /// <para>www.google.com/image.png -> returns : image.png</para>
        ///
        /// </summary>
        /// <param name="hreflink">
        /// </param>
        /// <param name="imageUrl">
        /// </param>
        /// <param name="downloadPath"></param>
        /// <param name="image">
        /// </param>
        /// <returns></returns>

        private void DownloadFileLocal(ImageModel image)
        {
            if (!IsHated(image.Url))
            {
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadProgressChanged += Wc_DownloadProgressChanged;
                    wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
                    wc.DownloadFileAsync(new Uri(image.Url), image.LocalUrl);
                    images.Add(image);
                }
            }
        }

        /// <summary>
        /// Download a file asynchronously in the desktop path, show the download progress and save it with the original filename.
        /// </summary>
        /// <param name="image">
        /// </param>
        private void DownloadFile(ImageModel image)
        {
            if (!IsHated(image.Url))
            {
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadProgressChanged += Wc_DownloadProgressChanged;
                    wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
                    wc.DownloadFileAsync(new Uri(image.Url), image.LocalUrl);
                    images.Add(image);
                }
            }
        }

        private bool IsHated(string Url)
        {
            List<ImageModel> hatedImages = DataKeeper.GetFileSnapShot().AllImages.Where(x => x.IsHated).ToList();
            if (hatedImages.Count == 0)
            {
                return false;
            }
            else
            {
                ImageModel hatedImage = hatedImages.FirstOrDefault(x => x.Url == Url);
                if (hatedImage != null)
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
        private void Wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (_IsUserRequested)
            {
                GlobalConfig.EventSystem.InvokePercentChangeEvent(e.ProgressPercentage);
            }
        }

        private void Wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {

            if (e.Cancelled)
            {
                HandleDownloadCancel();
            }

            if (e.Error != null) // We have an error! Retry a few times, then abort.
            {
                CustomMessageBox.Show(e.Error.InnerException.ToString());
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
            }
        }

        private void SubmitChanges()
        {
            if (images.Count > 0)
            {
                GiveEachImageAnId();
                if (errorIndex.Count > 0)
                {
                    GlobalConfig.EventSystem.
                        InvokeErrorsEncounteredEvent($"Encountered {errorIndex.Count} errors in download process, " +
                        $"corrupted files have been deleted and download is complete");
                    RemoveCorruptedImages();
                    images = images.Where(x => x.IsDownloaded).ToList();
                    errorIndex.Clear();
                }
                if (images.Count > 0)
                {
                    UpdateImageFile();
                }
            }
        }
    }
}