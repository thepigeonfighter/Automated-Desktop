using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomatedDesktopBackgroundLibrary.StringExtensions;
namespace AutomatedDesktopBackgroundLibrary
{
    public static class FileCollectionExtensions
    {
        //Have this class process requests like figuring out how many images are downloaded 
        // for an interest etc
        public static IFilteredFileResult GetAllDownloadedImages(this IFileCollection fileCollection)
        {
            List<ISaveable> imageData = new List<ISaveable>();
            IFilteredFileResult requestResult = new FilteredFileResult();

            try
            {

                imageData.AddRange(fileCollection.AllImages.Where(x => x.IsDownloaded == true));
                imageData.AddRange(fileCollection.FavoriteImages.Where(x => x.IsDownloaded == true));
                requestResult.SetResults(imageData);
                requestResult.SucessfulQuery = true;
            }
            catch
            {

                requestResult.SucessfulQuery = false;
            }

            return requestResult;

        }
        public static IFilteredFileResult GetAllImageEntries(this IFileCollection fileCollection)
        {
            IFilteredFileResult result = new FilteredFileResult();
            List<ImageModel> images = fileCollection.AllImages;
            images.AddRange(fileCollection.HatedImages);
            images.AddRange(fileCollection.FavoriteImages);
            List<ISaveable> allImages = new List<ISaveable>(images);
            result.SetResults(allImages);
            return result;
        }
        public static IFilteredFileResult GetLastImageDownloaded(this IFileCollection fileCollection)
        {
            IFilteredFileResult filteredFileResult = new FilteredFileResult();
            try
            {
                List<ISaveable> downloadedImages = new List<ISaveable>();
                filteredFileResult = fileCollection.GetAllDownloadedImages();
                downloadedImages = filteredFileResult.GetResults();
                int maxId = downloadedImages.Max(x => x.Id);
                ISaveable selectedImage = downloadedImages.FirstOrDefault(x => x.Id == maxId);
                filteredFileResult.SelectedResult = selectedImage;

                filteredFileResult.SucessfulQuery = true;
            }

            catch
            {
                filteredFileResult.SucessfulQuery = false;

            }
            return filteredFileResult;
        }
        public static IFilteredFileResult GetInterestByName(this IFileCollection fileCollection, string interestName)
        {
            InterestModel interest = fileCollection.AllInterests.FirstOrDefault(x => x.Name == interestName);
            IFilteredFileResult result = new FilteredFileResult() { SelectedResult = interest };
            return result;
        }
        public static IFilteredFileResult GetAllImagesAssociatedByInterest(this IFileCollection fileCollection, string interestName)
        {
            IFilteredFileResult filteredFileResult = new FilteredFileResult();
            try
            {
                ISaveable interest = GetInterestByName(fileCollection, interestName).SelectedResult;
                List<ISaveable> results = new List<ISaveable>();

                List<ImageModel> images = fileCollection.AllImages.Where(x => x.InterestId == interest.Id).ToList();
                List<ImageModel> favoriteImages = fileCollection.FavoriteImages.Where(x => x.InterestId == interest.Id).ToList();
                List<ImageModel> hatedImages = fileCollection.HatedImages.Where(x => x.InterestId == interest.Id).ToList();
                images.AddRange(favoriteImages);
                images.AddRange(hatedImages);
                 results = new List<ISaveable>(images);
                filteredFileResult.SetResults(results);
                filteredFileResult.SucessfulQuery = true;
            }
            catch
            {
                filteredFileResult.SucessfulQuery = false;
            }
            return filteredFileResult;
        }
        //WARNING hard coded into the unsplash model of each page returning ten items
        public static int GetNextPageInCollection(this IFileCollection fileCollection, string interestName)
        {
            int imagesAlreadyDownloaded = fileCollection.GetAllImagesAssociatedByInterest(interestName).GetResults().Count;
            InterestModel interest = (InterestModel)fileCollection.GetInterestByName(interestName).SelectedResult;
            int newPageNumber = 1;
            if (interest != null)
            {
                int maxImages = interest.TotalImages;
                
                if (imagesAlreadyDownloaded > 0 && imagesAlreadyDownloaded < 10)
                {
                    newPageNumber++;
                }
                else if (imagesAlreadyDownloaded < maxImages)
                {
                    newPageNumber = (imagesAlreadyDownloaded / 10) + 1;
                }
                else if (imagesAlreadyDownloaded == maxImages)
                {
                    newPageNumber = (imagesAlreadyDownloaded / 10) + 2;
                }
            }
            return newPageNumber;
        }
        public static bool IsEntireCollectionDownloaded(this IFileCollection fileCollection, string interestName)
        {
            int pageNumber = fileCollection.GetNextPageInCollection(interestName);
            try
            {
                InterestModel interest = (InterestModel)fileCollection.GetInterestByName(interestName).SelectedResult;
                if (pageNumber > interest.TotalPages)
                {
                    return true;
                }
            }
            catch(InvalidCastException) { return false; }


            return false;

        }
        
        
    }
}
