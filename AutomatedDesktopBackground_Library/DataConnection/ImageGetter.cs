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

namespace AutomatedDesktopBackgroundLibrary
{
    /// <summary>
    /// This class needs to get an image from online and then download it to a directory 
    /// </summary>
    public class ImageGetter
    {
       /// <summary>
       /// The amount of downloads that the class expects to process
       /// </summary>
        public int ExpectedDownloadAmount { get; set; }
        /// <summary>
        /// The amount of downloads that were originally requested
        /// </summary>
        private int totalDownloadsRequested;
        /// <summary>
        /// A list of any requests that resulted in an error. 
        /// </summary>
        private List<int> errorIndex = new List<int>();
        private ImageFileManager fileManager = new ImageFileManager();
        List<ImageModel> images = new List<ImageModel>();
        string mainQuery;
        bool userRequested = false;
        public void  GetImage(string imageUrl , string folderName, bool isUserRequested)
        {
            totalDownloadsRequested = ExpectedDownloadAmount;
            Directory.CreateDirectory($"{GlobalConfig.FileSavePath}/{folderName}");
            mainQuery = folderName;
            DownloadFile(imageUrl,folderName);
            userRequested = isUserRequested;
        }

        /// <summary>
        /// Get the filename from a web url : 
        /// 
        /// www.google.com/image.png -> returns : image.png
        /// 
        /// </summary>
        /// <param name="hreflink"></param>
        /// <returns></returns>
        private string GetFilleName(string hreflink)
        {
            Uri uri = new Uri(hreflink);

            string filename = Path.GetFileName(uri.LocalPath);

            return filename;
        }
        /// <summary>
        /// Download a file asynchronously in the desktop path, show the download progress and save it with the original filename.
        /// </summary>
        private void DownloadFile(string imageUrl, string folderName)
        {
            string filename = GetFilleName(imageUrl)+".JPEG";
            bool isHated = false;
            List<ImageModel> hatedImages = fileManager.GetAllHatedImages();
            //This checks to make sure that we haven't previously marked the image
            //as hated before we download it
            foreach(ImageModel i in hatedImages.ToArray())
            {
                if(i.Name == filename)
                {
                    isHated = true;
                    //This ensures that to download doesn't become out of sync 
                    ExpectedDownloadAmount--;
                }
            }
            if (!isHated)
            {
                
                using (WebClient wc = new WebClient())
                {
                    
                    wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                    wc.DownloadFileCompleted += wc_DownloadFileCompleted;
                    wc.DownloadFileAsync(new Uri( imageUrl), $"{GlobalConfig.FileSavePath}/{folderName}/{filename}");


                }
                CreateImageModel($"{GlobalConfig.FileSavePath}/{folderName}/{filename}", filename,imageUrl);
            }
        }
        private void CreateImageModel(string filePath, string name, string downloadPath)
        {
            ImageModel imageModel = new ImageModel();
            imageModel.FileDir = filePath;
            imageModel.Name = name;
            imageModel.DownloadPath = downloadPath;
            images.Add(imageModel);


        }

        /// <summary>
        ///  Show the progress of the download in a progressbar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (userRequested)
            {
                GlobalConfig.EventSystem.InvokePercentChangeEvent(e.ProgressPercentage);
            }
        }

        private void wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
           // progressBar1.Value = 0;

            if (e.Cancelled)
            {
                
                if (userRequested)
                {
                    GlobalConfig.EventSystem.InvokeDownloadImageEvent(false);
                    MessageBox.Show("The download has been cancelled");
                }
            }

            if (e.Error != null) // We have an error! Retry a few times, then abort.
            {
                errorIndex.Add(totalDownloadsRequested - ExpectedDownloadAmount );
                if (userRequested)
                {
                    GlobalConfig.EventSystem.InvokeDownloadImageEvent(false);
                }
            }
            ExpectedDownloadAmount--;
            if(ExpectedDownloadAmount == 0)
            {
                SubmitChanges();
                if (userRequested)
                {
                    GlobalConfig.EventSystem.InvokeDownloadCompleteEvent(true);
                }
            }
            else
            {
                if (userRequested) { GlobalConfig.EventSystem.InvokeDownloadImageEvent(true); };
            }
           // MessageBox.Show("File succesfully downloaded");
        }
        private void SubmitChanges()
        {
            if (images.Count > 0)
            {

                int interestId = fileManager.GetInterestIdByInterestName(mainQuery);
                //This sets every image in this ground to have an associated interestId
                images.ForEach(x => x.InterestId = interestId);
                List<ImageModel> existingImages = TextConnectorProcessor.LoadFromTextFile<ImageModel>(GlobalConfig.ImageFile);
                int currentId = 1;
                if (existingImages.Count > 0)
                {
                    currentId = existingImages.Max(x => x.Id) + 1;
                }
                foreach (ImageModel i in images)
                {
                    i.IsDownloaded = true;
                    i.Id = currentId;
                    currentId++;
                }
                if(errorIndex.Count > 0)
                {
                    //This loops through any errors if we encountered any errors it deletes the file and 
                    //adjust the save list to not show any images that failed to download
                    
                    for(int i = 0; i < errorIndex.Count; i++)
                    {
                        int index = errorIndex[i];
                        ImageModel corruptedImage = images[index];
                        corruptedImage.IsDownloaded = false;

                    }

                    foreach(ImageModel i in images)
                    {
                        if(!i.IsDownloaded)
                        {
                            string filePath = i.FileDir;
                            if(File.Exists(filePath))
                            {
                                File.Delete(filePath);
                            }

                        }
                    }
                    images = images.Where(x => x.IsDownloaded == true).ToList();
                }
                if (images.Count > 0)
                {
                    existingImages.ForEach(x => images.Add(x));
                    TextConnectorProcessor.SaveToTextFile(images, GlobalConfig.ImageFile);
                    images.Clear();
                }
                if (userRequested)
                {
                    if (errorIndex.Count == 0)
                    {
                        MessageBox.Show("Download Complete!");
                    }
                    else
                    {
                        MessageBox.Show($"Encountered {errorIndex.Count} errors in download process, corrupted files have been deleted and download is complete");
                    }
                }
            }
        }


    }

    }
