using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundLibrary
{
	/// <summary>
	/// Need to get this to a place where it can track which images are currently and then pick a new list of images in a cycle 
	/// So far the bool entireCollection downloaded has been sucessfully set up but the all images associated by interest is a little goofy right now for some reason 
	/// </summary>
	public class LocalImageGetter
	{
		private static   List<ImageModel> GetLocalImagesToDownload(string interestName)
		{
			List<ImageModel> output = new List<ImageModel>();
			InterestModel interest = TextConnectorProcessor.LoadFromTextFile<InterestModel>(GlobalConfig.InterestFile).First(x=> x.Name == interestName);
            //On first time through this sets the entirecollectiondownloaded flag to true
            if(!interest.EntireCollectionDownloaded)
            {
                UpdateInterest(interest);
            }
			List<ImageModel> allImages = TextConnectorProcessor.LoadFromTextFile<ImageModel>(GlobalConfig.ImageFile);
			List<ImageModel> allImagesAssociatedByInterest = allImages.Where(x => x.InterestId == interest.Id).ToList();
			allImagesAssociatedByInterest.ForEach(x => allImages.Remove(x));
		    allImagesAssociatedByInterest = allImagesAssociatedByInterest.OrderBy(x => x.Id).ToList();
			int lastPhotoIndex = GetIndexOfLastDownloadedPhoto(allImagesAssociatedByInterest);
			UpdateImageFile(allImagesAssociatedByInterest);
            //if we are at the end of the collection we will start downloading from the begining of the collection               
            if (lastPhotoIndex >= allImagesAssociatedByInterest.Count - 1)
            {
                //If the collection is bigger than ten we download the first ten 
                if (allImagesAssociatedByInterest.Count > 10)
                {
                    List<ImageModel> imagesToBeDownloaded = allImagesAssociatedByInterest.GetRange(0, 10);
                    foreach (ImageModel i in imagesToBeDownloaded)
                    {
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
                        allImagesAssociatedByInterest.Remove(i);
                        i.IsDownloaded = true;
                        output.Add(i);
                    }
                }
            }
            //If we can download ten photos without getting to the end upf the list
            //Then we will do it 
            else if (lastPhotoIndex + 10 < allImagesAssociatedByInterest.Count)
            {
                List<ImageModel> imagesToBeDownloaded = allImagesAssociatedByInterest.GetRange(lastPhotoIndex, 10);
                foreach (ImageModel i in imagesToBeDownloaded)
                {
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
				TextConnectorProcessor.SaveToTextFile(allImages, GlobalConfig.ImageFile);
			
			
			return output;
		}
		private static void UpdateImageFile(List<ImageModel> images)
		{
			List<ImageModel> updatedImages = new List<ImageModel>();
			foreach(ImageModel i in images)
			{
				i.IsDownloaded = false;
                File.Delete(i.FileDir);
				updatedImages.Add(i);
			}
			TextConnectorProcessor.UpdateCollection(images, updatedImages, GlobalConfig.ImageFile);
		}
		private static int GetIndexOfLastDownloadedPhoto(List<ImageModel> images)
		{
			int lastPhotoId = images.Where(x => x.IsDownloaded == true).Max(x => x.Id);
			int lastPhotoIndex = images.FindIndex(x => x.Id == lastPhotoId);
			return lastPhotoIndex;
		}
		private static InterestModel UpdateInterest(InterestModel interest)
		{
			InterestModel updatedInterest = interest;

			updatedInterest.EntireCollectionDownloaded = true;

			TextConnectorProcessor.UpdateEntry(interest, updatedInterest, GlobalConfig.InterestFile);

			return updatedInterest;
		}
		private static  void DownloadImages(List<ImageModel> imagesToDownload, bool userRequested)
		{
			ImageGetter imageGetter = new ImageGetter();
            imageGetter.ExpectedDownloadAmount = imagesToDownload.Count;
			foreach(ImageModel i in imagesToDownload)
			{
				imageGetter.GetImageLocal(i.DownloadPath, i.FileDir, userRequested);
			}
		}
		public static void GetLocalImages(string interestName, bool userRequested)
		{
			List<ImageModel> imagesToDownload = GetLocalImagesToDownload(interestName);
			DownloadImages(imagesToDownload, userRequested);
		}
	
	}
}
