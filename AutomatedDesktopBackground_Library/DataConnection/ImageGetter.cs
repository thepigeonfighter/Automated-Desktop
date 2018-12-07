using AutomatedDesktopBackgroundLibrary.DataConnection;
using AutomatedDesktopBackgroundLibrary.StringExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using log4net;


namespace AutomatedDesktopBackgroundLibrary
{
    /// <summary>
    /// This class needs to get an image from online and then download it to a directory
    /// </summary>
    public class ImageGetter : ImageGetterBase
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ImageGetter(IDataKeeper dataKeeper, ImageModelBuilder imageBuilder) : base(dataKeeper, imageBuilder)
        {
        }

        public void GetImage(string imageUrl, string folderName,string description, bool userRequested)
        {
            totalDownloadsRequested = ExpectedDownloadAmount;
            InterestModel interest = _dataKeeper.GetFileSnapShot().AllInterests.First(x=>x.Name == folderName);
            Directory.CreateDirectory($@"{InternalFileDirectorySystem.ImagesFolder}\{interest.Name}");
            ImageModel imageToDownload = _imageBuilder.Build(imageUrl,description, interest);
            log.Debug($"Requesting to download an image named {imageToDownload.Name} from {imageUrl}");
            DownloadFile(imageToDownload);
            _IsUserRequested = userRequested;
        }

        public void GetImageLocal(ImageModel image, bool userRequested)
        {
            totalDownloadsRequested = ExpectedDownloadAmount;
            string dirPath = Path.GetDirectoryName(image.LocalUrl);
            Directory.CreateDirectory(dirPath);
            DownloadFileLocal(image);
            _IsUserRequested = userRequested;
            isLocalGet = true;
        }
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
            List<ImageModel> hatedImages = _dataKeeper.GetFileSnapShot().AllImages.Where(x => x.IsHated).ToList();
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
                log.Warn($"This image has failed to download because {e.Error.InnerException.Message}");
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
                    string error = errorIndex.Count > 1 ? "errors" : "error";
                    GlobalConfig.EventSystem.
                        InvokeErrorsEncounteredEvent($"Encountered {errorIndex.Count} {error} in download process, " +
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