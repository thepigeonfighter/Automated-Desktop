using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace AutomatedDesktopBackgroundLibrary.DataConnection
{
    public class ImageGetterBase : IFileListener
    {
        protected bool _IsUserRequested;
        protected IFileCollection _fileCollection;

        /// <summary>
        /// A list of any requests that resulted in an error.
        /// </summary>
        protected List<int> errorIndex = new List<int>();

        /// <summary>
        /// The amount of downloads that the class expects to process
        /// </summary>
        public int ExpectedDownloadAmount { get; set; }

        /// <summary>
        /// The amount of downloads that were originally requested
        /// </summary>
        protected int totalDownloadsRequested;

        protected List<ImageModel> images = new List<ImageModel>();
        protected bool isLocalGet = false;

        public ImageGetterBase()
        {
            _fileCollection = DataKeeper.GetFileSnapShot();
            DataKeeper.RegisterFileListener(this);
        }

        protected void HandleError()
        {
            errorIndex.Add(totalDownloadsRequested - ExpectedDownloadAmount);
            if (_IsUserRequested)
            {
                //MessageBox.Show(e.Error.InnerException.ToString());
                GlobalConfig.EventSystem.InvokeDownloadImageEvent("!");
            }
        }

        protected void HandleImageDownload()
        {
            if (_IsUserRequested)
            {
                int downloadsComplete = -(ExpectedDownloadAmount - totalDownloadsRequested);
                string message = $"{downloadsComplete}/{totalDownloadsRequested}";
                GlobalConfig.EventSystem.InvokeDownloadImageEvent(message);
            }
        }

        protected void HandleDownloadCancel()
        {
            if (_IsUserRequested)
            {
                GlobalConfig.EventSystem.InvokeDownloadImageEvent("Download Cancelled");
                MessageBox.Show("The download has been cancelled");
            }
        }

        protected void HandleDownloadComplete()
        {
            if (_IsUserRequested)
            {
                string message = $"{totalDownloadsRequested}/{totalDownloadsRequested}";
                GlobalConfig.EventSystem.InvokeDownloadImageEvent(message);
                GlobalConfig.EventSystem.InvokeDownloadCompleteEvent(true);
            }
            GlobalConfig.InCollectionRefresh = false;
        }

        protected void DisplayDownloadCompletionMessage()
        {
            if (_IsUserRequested)
            {
                if (errorIndex.Count == 0)
                {
                    MessageBox.Show("Download Complete!");
                }
                else
                {
                    MessageBox.Show($"Encountered {errorIndex.Count} errors in download process, corrupted files have been deleted and download is complete");
                    errorIndex.Clear();
                }
            }
        }

        protected void RemoveCorruptedImages()
        {
            //This loops through any errors if we encountered any errors it deletes the file and
            //adjust the save list to not show any images that failed to download

            for (int i = 0; i < errorIndex.Count; i++)
            {
                int index = errorIndex[i];
                ImageModel corruptedImage = images[index];
                corruptedImage.IsDownloaded = false;
            }

            foreach (ImageModel i in images)
            {
                if (!i.IsDownloaded)
                {
                    File.Delete(i.LocalUrl);
                }
            }
        }

        protected void UpdateImageFile()
        {
            if (!isLocalGet)
            {
                RemoveOldPhotos();
                images.ForEach(x => DataKeeper.AddImage(x));
            }
            else
            {
                images.ForEach(x => DataKeeper.AddImage(x));
                RemovePhotosWhereIsDownloadedIsFalse();
            }
            images.Clear();
        }

        private void RemovePhotosWhereIsDownloadedIsFalse()
        {
            List<ImageModel> photosToRemove = _fileCollection.AllImages.Where(x => !x.IsDownloaded).ToList();
            photosToRemove.ForEach(x => DataKeeper.DeleteImage(x, true));
        }

        private void RemoveOldPhotos()
        {
            int interestId = images[0].InterestId;
            List<ImageModel> oldPhotos = _fileCollection.AllImages.Where(x => x.InterestId == interestId).ToList();
            List<ImageModel> unFavoritePhotos = oldPhotos.Where(x => !x.IsFavorite).ToList();
            if (unFavoritePhotos.Count > 0)
            {
                foreach (ImageModel i in unFavoritePhotos)
                {
                    i.IsDownloaded = false;
                    DataKeeper.DeleteImage(i, true);
                }
                unFavoritePhotos.ForEach(x => DataKeeper.AddImage(x));
            }
        }

        protected void GiveEachImageAnId()
        {
            int interestId = images[0].InterestId;
            //This sets every image in this ground to have an associated interestId
            images.ForEach(x => x.InterestId = interestId);
            List<ImageModel> existingImages = _fileCollection.AllImages;
            int currentId = 1;
            if (existingImages.Count > 0)
            {
                currentId = existingImages.Max(x => x.Id) + 1;
            }
            if (!isLocalGet)
            {
                foreach (ImageModel i in images)
                {
                    i.IsDownloaded = true;
                    i.Id = currentId;
                    currentId++;
                }
            }
        }

        public void OnFileUpdate()
        {
            _fileCollection = DataKeeper.GetFileSnapShot();
        }
    }
}