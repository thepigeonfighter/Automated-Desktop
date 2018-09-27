using AutomatedDesktopBackgroundLibrary.StringExtensions;
using System.Collections.Generic;
using System.Linq;

namespace AutomatedDesktopBackgroundLibrary
{
    //This class is a hot mess of garbage but works ok so i am going to leave it for now
    public class LocalImageGetter
    {
        private readonly IFileCollection _fileCollection;

        public LocalImageGetter(IFileCollection fileCollection)
        {
            _fileCollection = fileCollection;
        }

        private List<ImageModel> GetLocalImagesToDownload(string interestName)
        {
            List<ImageModel> output = new List<ImageModel>();
            InterestModel interest = interestName.GetInterestByName();
            const int defaultCollectionSize = 10;
            //On first time through this sets the entirecollectiondownloaded flag to true
            if (!interest.EntireCollectionDownloaded)
            {
                UpdateInterest(interest);
            }
            List<ImageModel> allImages = _fileCollection.AllImages;
            List<ImageModel> allImagesAssociatedByInterest = allImages.Where(x => x.InterestId == interest.Id).ToList();
            //Removes the images we are about to manipulate so that we do not create duplicate records
            allImagesAssociatedByInterest.ForEach(x => allImages.Remove(x));
            allImagesAssociatedByInterest = allImagesAssociatedByInterest.OrderBy(x => x.Id).ToList();
            int lastPhotoIndex = GetIndexOfLastDownloadedPhoto(allImagesAssociatedByInterest);
            //Sets all the image's IsDownloaded value to false
            List<ImageModel> unFavoritedPhotos = allImagesAssociatedByInterest.Where(x => !x.IsDownloaded).ToList();
            SetIsDownloadedToFalse(unFavoritedPhotos);
            //if we are at the end of the collection we will start downloading from the begining of the collection
            if (lastPhotoIndex >= allImagesAssociatedByInterest.Count - 1)
            {
                //If the collection is bigger than ten we download the first ten
                if (allImagesAssociatedByInterest.Count > defaultCollectionSize)
                {
                    List<ImageModel> imagesToBeDownloaded = allImagesAssociatedByInterest.GetRange(0, defaultCollectionSize);
                    foreach (ImageModel i in imagesToBeDownloaded)
                    {
                        //Remove the duplicate values from the list
                        allImagesAssociatedByInterest.Remove(i);
                        i.IsDownloaded = true;
                        output.Add(i);
                    }
                    imagesToBeDownloaded.ForEach(x => allImagesAssociatedByInterest.Add(x));
                }
                //if the collection is smaller than ten we download the whole collection
                else
                {
                    foreach (ImageModel i in allImagesAssociatedByInterest)
                    {
                        //  allImagesAssociatedByInterest.Remove(i);
                        i.IsDownloaded = true;
                        output.Add(i);
                    }
                }
            }
            //If we can download ten photos without getting to the end of the list
            //Then we will do it
            else if (lastPhotoIndex + defaultCollectionSize < allImagesAssociatedByInterest.Count)
            {
                List<ImageModel> imagesToBeDownloaded = allImagesAssociatedByInterest.GetRange(lastPhotoIndex, defaultCollectionSize);
                foreach (ImageModel i in imagesToBeDownloaded)
                {
                    //removes the duplicate values from the list
                    allImagesAssociatedByInterest.Remove(i);
                    i.IsDownloaded = true;
                    output.Add(i);
                }
                imagesToBeDownloaded.ForEach(x => allImagesAssociatedByInterest.Add(x));
            }
            //Else we will download what ever is left over from the collection to start the cycle over
            else
            {
                int imagesLeftInCollection = allImagesAssociatedByInterest.Count - lastPhotoIndex;
                List<ImageModel> imagesToBeDownloaded = allImagesAssociatedByInterest.GetRange(lastPhotoIndex, imagesLeftInCollection);
                if (imagesToBeDownloaded.Count < defaultCollectionSize)
                {
                    int imagesNeededToCompleteSet = defaultCollectionSize - imagesToBeDownloaded.Count;
                    List<ImageModel> extraImages = allImagesAssociatedByInterest.GetRange(0, imagesNeededToCompleteSet);
                    extraImages.ForEach(x => imagesToBeDownloaded.Add(x));
                }
                foreach (ImageModel i in imagesToBeDownloaded)
                {
                    allImagesAssociatedByInterest.Remove(i);
                    i.IsDownloaded = true;
                    output.Add(i);
                }
                imagesToBeDownloaded.ForEach(x => allImagesAssociatedByInterest.Add(x));
            }
            allImagesAssociatedByInterest.ForEach(x => allImages.Add(x));
            allImages = allImages.OrderBy(x => x.Id).ToList();
            allImages.ForEach(x => DataKeeper.AddImage(x));

            return output;
        }

        private static int GetIndexOfLastDownloadedPhoto(List<ImageModel> images)
        {
            try
            {
                int lastPhotoId = images.Where(x => x.IsDownloaded).Max(x => x.Id);
                int lastPhotoIndex = images.FindIndex(x => x.Id == lastPhotoId);
                return lastPhotoIndex;
            }
            catch
            {
                return 0;
            }
        }

        private static InterestModel UpdateInterest(InterestModel interest)
        {
            InterestModel updatedInterest = interest;

            updatedInterest.EntireCollectionDownloaded = true;
            DataKeeper.AddInterest(updatedInterest);

            return updatedInterest;
        }

        private static void DownloadImages(List<ImageModel> imagesToDownload, bool userRequested)
        {
            ImageGetter imageGetter = new ImageGetter
            {
                ExpectedDownloadAmount = imagesToDownload.Count
            };
            foreach (ImageModel i in imagesToDownload)
            {
                imageGetter.GetImageLocal(i, userRequested);
            }
        }

        public void GetLocalImages(string interestName, bool userRequested)
        {
            List<ImageModel> imagesToDownload = GetLocalImagesToDownload(interestName);
            DownloadImages(imagesToDownload, userRequested);
        }

        private static void SetIsDownloadedToFalse(List<ImageModel> images)
        {
            List<ImageModel> updatedImages = new List<ImageModel>();
            foreach (ImageModel i in images)
            {
                i.IsDownloaded = false;
            }
        }
    }
}