using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AutomatedDesktopBackgroundLibrary
{
    /// <summary>
    /// This class should be able to tell you how many images are in the database
    /// how many images are associated with each interest 
    /// manage the files to make sure there arent repeats
    /// </summary>
    public class ImageFileManager
    {
        public bool InterestExists(string interestName)
        {
            string filePath = $"{GlobalConfig.FullFilePath(interestName)}";
            if (Directory.Exists(filePath))
            {
                return true;
            }
            return false;
        }
        public List<ImageModel> GetAllImagesByInterestId(int id)
        {
            List<ImageModel> images = new List<ImageModel>();
            images = TextConnectorProcessor.LoadFromTextFile<ImageModel>(GlobalConfig.ImageFile).Where(x => x.InterestId == id).ToList();
            return images;
        }
        public List<ImageModel> GetAllImagesByInterestName(string name)
        {
            int id = GetInterestIdByInterestName(name);
            List<ImageModel> images = GetAllImagesByInterestId(id);
            return images;
        }
        public int GetInterestIdByInterestName(string name)
        {
            InterestModel interest = TextConnectorProcessor.LoadFromTextFile<InterestModel>(GlobalConfig.InterestFile).FirstOrDefault(x => x.Name == name);
            return interest.Id;
        }
        public List<ImageModel> ImagesNotAssociatedToInterestId(int id)
        {
            List<ImageModel> images = new List<ImageModel>();
            images = TextConnectorProcessor.LoadFromTextFile<ImageModel>(GlobalConfig.ImageFile).Where(x => x.InterestId != id).ToList();
            return images;
        }
       public async Task RemoveImagesByInterestAsync(string interest)
        {
            List<InterestModel> interests = TextConnectorProcessor.LoadFromTextFile<InterestModel>(GlobalConfig.InterestFile);
            //This makes sure that there are at least going to be one more interest before
            //Deciding which ones to save
            if (interests.Count > 0)
            {
                //Make Sure interest list is up to date
                //Find interest that is supposed to be removed
                InterestModel interestToRemove = interests.FirstOrDefault(x => x.Name == interest);
                if(interestToRemove == null) { throw new Exception("There is no interest by that name!"); }
                // Finds all the images 
                List<ImageModel> allImages = TextConnectorProcessor.LoadFromTextFile<ImageModel>(GlobalConfig.ImageFile);
                // Notes that the images associated with the interest about to be removed are no longer downloaded
                List<ImageModel> imagesAboutToBeRemoved = allImages.Where(x => x.InterestId == interestToRemove.Id).ToList();
                List<ImageModel> favoriteImages = GetAllFavoriteImages();
                //This removes any images that have been favorited from the list of images that will be deleted. 
                favoriteImages.ForEach(x => imagesAboutToBeRemoved.Remove(x));
                imagesAboutToBeRemoved.ForEach(x => x.IsDownloaded = false);

                //If there are any images to save it will save it replacing that save file
                if (allImages.Count > 0)
                {
                    TextConnectorProcessor.SaveToTextFile(allImages, GlobalConfig.ImageFile);
                }
                //If there aren't any images that are to be kept it deletes the entire image.csv file
                else if (allImages.Count == 0)
                {
                    File.Delete(GlobalConfig.ImageFile);
                }
                //Then it removes the interest
                interests.Remove(interestToRemove);
                if (interests.Count == 0)
                {
                    File.Delete(GlobalConfig.InterestFile);
                }
                else
                {
                    TextConnectorProcessor.SaveToTextFile(interests.ToList(), GlobalConfig.InterestFile);
                }
            }
            //This deletes the entire folder of photos that is associated with the
            // interest 
            if (Directory.Exists($@"{GlobalConfig.FileSavePath}\{interest}"))
            {
                 await Task.Run(()=>DeleteDirectoryAsync(interest));
                
            }
        }
        /// <summary>
        /// Returns a page number for a source download based on the amount of previous images downloaded
        /// </summary>
        /// <param name="interestName"></param>
        /// <returns></returns>
        public int GetNewPageQuerry(string interestName)
        {
            int newPageNumber = 1;
            if (InterestExists(interestName))
            {
                InterestModel interest = TextConnectorProcessor.LoadFromTextFile<InterestModel>(GlobalConfig.InterestFile).FirstOrDefault(x=>x.Name == interestName);
                
                int interestId = interest.Id;
                List<ImageModel> images = GetAllImagesByInterestName(interestName);
                
                if (images.Count > 0)
                {
                    newPageNumber = images.Count;
                    //This makes sure to add any photos that have been downloaded before in other folders
                    List<ImageModel> anyHatedImagesWithThisId = GetAllHatedImages().Where(x => x.InterestId == interestId).ToList();
                    List<ImageModel> anyFavoritedImagesWithThisId = GetAllFavoriteImages().Where(x => x.InterestId == interestId).ToList();
                    anyHatedImagesWithThisId?.ForEach(x => newPageNumber++);
                    anyFavoritedImagesWithThisId?.ForEach(x => newPageNumber++);
                    //This cycles through all the images that are available on the image database that we are using
                    //The reason for dividing by 10 is because that is how many results we get per querry
                    //That number could and probably should be changed but if we ever get less than 
                    //10 results it should indicate that it is the last page of results
                    newPageNumber = (newPageNumber/ 10) +1;
                    if(images.Count == interest.TotalImages) { newPageNumber ++; }
                }

                
            }
            return newPageNumber;
        }
        public bool IsCollectionDownloaded(string interestName)
        {
            List<InterestModel> interests = TextConnectorProcessor.LoadFromTextFile<InterestModel>(GlobalConfig.InterestFile);
            
            InterestModel selectedInterest = interests.FirstOrDefault(x => x.Name == interestName);
            interests.Remove(selectedInterest);
            if(selectedInterest!= null)
            {
                int currentPage = GetNewPageQuerry(selectedInterest.Name);
                if(currentPage > selectedInterest.TotalPages)
                {
                    return true;
                }
                return false;
            }
            return false;
        }
        private List<ImageModel> GetAllFavoriteImages()
        {
            return TextConnectorProcessor.LoadFromTextFile<ImageModel>(GlobalConfig.FavoritesFile);
        }
        public void LikeImage(ImageModel image)
        {

            DirectoryInfo directoryInfo =  Directory.CreateDirectory(GlobalConfig.FullFilePath("Favorited Images"));
            string newPath = $@"{directoryInfo.FullName}/{image.Name}";
            if (File.Exists(image.FileDir))
            {
               
                File.Copy(image.FileDir, newPath );
                RemoveImageEntry(image);
                image.FileDir = newPath;
            }
            else
            {
                using (WebClient wc = new WebClient())
                {

                    wc.DownloadFileAsync(new Uri(image.DownloadPath), image.FileDir);
                    File.Copy(image.FileDir, newPath);
                    RemoveImageEntry(image);
                    image.FileDir = newPath;

                }
               
            }
            TextConnectorProcessor.CreateEntry(image, GlobalConfig.FavoritesFile);
        }
        private void RemoveImageEntry(ImageModel imageToRemove)
        {
            List<ImageModel> allImages = TextConnectorProcessor.LoadFromTextFile<ImageModel>(GlobalConfig.ImageFile);
            ImageModel selectedImage = allImages.First(x=>x.FileDir == imageToRemove.FileDir);
            allImages.Remove(selectedImage);
            TextConnectorProcessor.SaveToTextFile(allImages, GlobalConfig.ImageFile);
        }
        public List<ImageModel> GetAllHatedImages()
        {
            return TextConnectorProcessor.LoadFromTextFile<ImageModel>(GlobalConfig.HatedFile);
        }
        private async Task DeleteDirectoryAsync(string interest)
        {
            await Task.Run(() => Directory.Delete($@"{GlobalConfig.FileSavePath}\{interest}",true));
        }
        public async Task HateImage(ImageModel image)
        {
            List<ImageModel> images = TextConnectorProcessor.LoadFromTextFile<ImageModel>(GlobalConfig.ImageFile);
            
            
            //If there are at least two photos still downloaded than pick a random photo
            //Then delete the picture that is hated then save the new list of leftover images
            if (images.Count > 1)
            {

                foreach (ImageModel i in images.ToArray())
                {
                    if (i.Id == image.Id)
                    {
                        images.Remove(i);
                        break;
                    }
                }
                
                images.ToList();
                TextConnectorProcessor.SaveToTextFile(images, GlobalConfig.ImageFile);
                image.IsDownloaded = false;
                TextConnectorProcessor.CreateEntry(image, GlobalConfig.HatedFile);
                BackGroundPicker backGroundPicker = new BackGroundPicker();
                backGroundPicker.PickRandomBackground();
                await Task.Run(()=>DeleteImageAsync(image.FileDir));
            }
            else
            {
                //If there is only
                MessageBox.Show("There is only one photo left in your collection. You must download more photos before deleting this photo.");
            }
        }
        private async Task DeleteImageAsync(string url)
        {
            bool fileReady = false;
            int timeOut = 100;
            int tries = 0;
            while(!fileReady && tries < timeOut)
            {
                fileReady = IsFileReady(url);
                tries++;
                if(fileReady && File.Exists(url))
                {
                     File.Delete(url);
                    GlobalConfig.EventSystem.InvokeImageHatingCompleteEvent();
                }
                else if (!File.Exists(url))
                {
                    GlobalConfig.EventSystem.InvokeImageHatingCompleteEvent();
                    break;
                }
                await Task.Delay(500);
            }
            if(tries> timeOut) { MessageBox.Show("Deleting an image timed out"); }
        }
        public static bool IsFileReady(string filename)
        {
            // If the file can be opened for exclusive access it means that the file
            // is no longer locked by another process.
            try
            {
                using (FileStream inputStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.None))
                    return inputStream.Length > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<ImageModel> GetAllFavoritedImages()
        {
            return TextConnectorProcessor.LoadFromTextFile<ImageModel>(GlobalConfig.FavoritesFile);
        }


    }
}
