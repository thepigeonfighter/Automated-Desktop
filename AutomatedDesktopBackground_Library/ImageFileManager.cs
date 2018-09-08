using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundLibrary
{
    /// <summary>
    /// This class should be able to tell you how many images are in the database
    /// how many images are associated with each interest 
    /// manage the files to make sure there arent repeats
    /// </summary>
    public class ImageFileManager
    {
        private  bool InterestExists(string interestName)
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
       public void RemoveImagesByInterest(string interest)
        {
            List<InterestModel> interests = TextConnectorProcessor.LoadFromTextFile<InterestModel>(GlobalConfig.InterestFile);
            //This makes sure that there are at least going to be one more interest before
            //Deciding which ones to save
            if (interests.Count > 1)
            {
                //Make Sure interest list is up to date
                //Find interest that is supposed to be removed
                InterestModel interestToRemove = interests.FirstOrDefault(x => x.Name == interest);
                if(interestToRemove == null) { throw new Exception("There is no interest by that name!"); }
                // Finds all the images that are not associated with the interest that is about to be removed
                List<ImageModel> imagesToKeep = ImagesNotAssociatedToInterestId(interestToRemove.Id);
                List<ImageModel> favoriteImages = GetAllFavoriteImages().Where(x => x.InterestId == interestToRemove.Id).ToList();
                favoriteImages?.ForEach(x => imagesToKeep.Add(x));
                //If there are any images to save it will save it replacing that save file
                if (imagesToKeep.Count > 0)
                {
                    TextConnectorProcessor.SaveToTextFile(imagesToKeep, GlobalConfig.ImageFile);
                }
                //If there aren't any images that are to be kept it deletes the entire image.csv file
                else if (imagesToKeep.Count == 0)
                {
                    File.Delete(GlobalConfig.ImageFile);
                }
                //Then it removes the interest
                interests.Remove(interestToRemove);
                TextConnectorProcessor.SaveToTextFile(interests.ToList(), GlobalConfig.InterestFile);
            }
            //Else if there is only one interest left we know that this is the
            // interest that the user is wanting to remove in which case 
            // we will delete the entire file
            else if (interests.Count == 1)
            {
                interests.Clear();
                File.Delete(GlobalConfig.InterestFile);
                File.Delete(GlobalConfig.ImageFile);
            }
            //This deletes the entire folder of photos that is associated with the
            // interest 
            if (Directory.Exists($@"{GlobalConfig.FileSavePath}\{interest}"))
            {
                //TODO make sure folder is not in use before deleting 
                Directory.Delete($@"{GlobalConfig.FileSavePath}\{interest}", true);
            }
        }
        
        public int GetNewPageQuerry(string interestName)
        {
            int newPageNumber = 1;
            if (InterestExists(interestName))
            {
                List<ImageModel> images = GetAllImagesByInterestName(interestName);
                if (images.Count > 1)
                {
                    //This cycles through all the images that are available on the image database that we are using
                    newPageNumber = (images.Count / 10) +1;
                }
            }
            return newPageNumber;
        }
        private List<ImageModel> GetAllFavoriteImages()
        {
            return TextConnectorProcessor.LoadFromTextFile<ImageModel>(GlobalConfig.FavoritesFile);
        }
        public void LikeImage(ImageModel image)
        {
            TextConnectorProcessor.CreateEntry(image, GlobalConfig.FavoritesFile);
        }
        /*
         * if you want to get results out of a task you overload the method with the vars you want effected
         * i.e. to get a progress report it would like like async Task Function(IProgress<ProgressModel> progress)
         * Then you can call an event that is associated with that var i.e. progress.Report() that would then load the changed 
         * data
         */

    }
}
