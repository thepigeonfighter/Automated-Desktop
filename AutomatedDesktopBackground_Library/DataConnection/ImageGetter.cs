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
        bool isUserRequested = false;
        public void  GetImage(string imageUrl , string folderName, bool userRequested)
        {
            totalDownloadsRequested = ExpectedDownloadAmount;
            string downloadPath = Directory.CreateDirectory($"{GlobalConfig.FileSavePath}/{folderName}").FullName;
            mainQuery = folderName;        
                DownloadFile(imageUrl, downloadPath);
       

            isUserRequested = userRequested;
        }
        public void GetImageLocal(string imageUrl, string downloadPath, bool userRequested)
        {
            totalDownloadsRequested = ExpectedDownloadAmount;
            string dirPath = Path.GetDirectoryName(downloadPath);
            Directory.CreateDirectory(dirPath);
            DownloadFileLocal(imageUrl, downloadPath);
            isUserRequested = userRequested;
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
        private void DownloadFileLocal(string imageUrl, string downloadPath)
        {

                using (WebClient wc = new WebClient())
                {

                    wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                    wc.DownloadFileCompleted += wc_DownloadFileCompleted;
                    wc.DownloadFileAsync(new Uri(imageUrl), downloadPath);

                }

        }
        /// <summary>
        /// Download a file asynchronously in the desktop path, show the download progress and save it with the original filename.
        /// </summary>
        private void DownloadFile(string imageUrl, string downloadPath)
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
                string path = $@"{downloadPath}\{filename}";
                using (WebClient wc = new WebClient())
                {
                    
                    wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                    wc.DownloadFileCompleted += wc_DownloadFileCompleted;                  
                    wc.DownloadFileAsync(new Uri( imageUrl),path);


                }
                    CreateImageModel(path, filename, imageUrl);
                
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
            if (isUserRequested)
            {
                GlobalConfig.EventSystem.InvokePercentChangeEvent(e.ProgressPercentage);
            }
        }

        private void wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
           // progressBar1.Value = 0;

            if (e.Cancelled)
            {
                
                if (isUserRequested)
                {

                    GlobalConfig.EventSystem.InvokeDownloadImageEvent("Download Cancelled");
                    MessageBox.Show("The download has been cancelled");
                }
            }

            if (e.Error != null) // We have an error! Retry a few times, then abort.
            {

                errorIndex.Add(totalDownloadsRequested - ExpectedDownloadAmount );
                if (isUserRequested)
                {
                    GlobalConfig.EventSystem.InvokeDownloadImageEvent("!");
                }
            }
            ExpectedDownloadAmount--;
            if(ExpectedDownloadAmount == 0)
            {
                
                if (isUserRequested)
                {
                    string message = $"{totalDownloadsRequested}/{totalDownloadsRequested}";
                    GlobalConfig.EventSystem.InvokeDownloadImageEvent(message);
                    GlobalConfig.EventSystem.InvokeDownloadCompleteEvent(true);
                }
                SubmitChanges();
            }
            else
            {
                if (isUserRequested) {
                    int downloadsComplete = -(ExpectedDownloadAmount - totalDownloadsRequested);
                    string message = $"{downloadsComplete}/{totalDownloadsRequested}";
                    GlobalConfig.EventSystem.InvokeDownloadImageEvent(message);
                };
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
                if (isUserRequested)
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
        }


    }

    }
