using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AutomatedDesktopBackgroundLibrary
{
    public class FileCleaner
    {
        public void Run(IFileCollection fileCollection)
        {
            CleanImageEntries(fileCollection);
            RemoveAnyHatedImages(fileCollection);
            RebuildImages(fileCollection);
        }

        private void CleanImageEntries(IFileCollection fileCollection)
        {
            List<ImageModel> images = ImageEntriesWithoutImages(fileCollection);
            images.ForEach(x => x.IsDownloaded = false);
            images.ForEach(x => DataKeeper.UpdateImage(x));
            Debug.WriteLine($"There was {images.Count} image entries that had no file associated with them. The extra images were removed");
        }

        private void RemoveAnyHatedImages(IFileCollection fileCollection)
        {
            List<ImageModel> hatedImages = new List<ImageModel>();
            foreach (ImageModel i in fileCollection.HatedImages)
            {
                if (File.Exists(i.LocalUrl))
                {
                    hatedImages.Add(i);
                }
            }
            hatedImages.ForEach(x => DataKeeper.DeleteDownloadedImageFile(x));
            Debug.WriteLine($"There were {hatedImages.Count} hated images that were not deleted. That are now deleted");
        }

        private void RebuildImages(IFileCollection fileCollection)
        {
            List<string> imageUrls = ImagesThatHaveNoRecord(fileCollection, GetImageUrls());
            List<ImageModel> rebuiltImages = new List<ImageModel>();
            imageUrls.ForEach(x => rebuiltImages.Add(BuildImageModelFromUrl(fileCollection, x)));
            List<ImageModel> favoriteImages = new List<ImageModel>();
            List<ImageModel> defaultImages = new List<ImageModel>();
            foreach (ImageModel image in rebuiltImages)
            {
                string folderName = Directory.GetParent(image.LocalUrl).Name;
                if (folderName == "Favorites")
                {
                    favoriteImages.Add(image);
                }
                else
                {
                    defaultImages.Add(image);
                }
            }
            SubmitChanges(fileCollection, favoriteImages, defaultImages);
            Debug.WriteLine($"There were {rebuiltImages.Count} images rebuilt");
        }

        private void SubmitChanges(IFileCollection fileCollection, List<ImageModel> favoriteImages, List<ImageModel> defaultImages)
        {
            favoriteImages.AddRange(fileCollection.FavoriteImages);
            defaultImages.AddRange(fileCollection.AllImages);
            DataKeeper.OverwriteImageFile(defaultImages);
            DataKeeper.OverwriteFavoriteImages(favoriteImages);
        }

        private List<ImageModel> ImageEntriesWithoutImages(IFileCollection fileCollection)
        {
            List<ImageModel> images = new List<ImageModel>();
            List<ISaveable> downloadedImages = fileCollection.GetAllDownloadedImages().GetResults();
            List<ImageModel> imagesToCheck = downloadedImages.ConvertAll(x => (ImageModel)x);
            foreach (ImageModel i in imagesToCheck)
            {
                if (!File.Exists(i.LocalUrl))
                {
                    images.Add(i);
                }
            }
            return images;
        }

        private List<string> ImagesThatHaveNoRecord(IFileCollection fileCollection, List<string> imageUrls)
        {
            List<string> images = new List<string>();
            foreach (string s in imageUrls)
            {
                ImageModel image = fileCollection.AllImages.FirstOrDefault(x => x.LocalUrl == s);
                if (image == null)
                {
                    images.Add(s);
                }
            }
            return images;
        }

        private List<string> GetImageUrls()
        {
            string[] directories = Directory.GetDirectories(StringExtensions.StringExtensions.GetApplicationDirectory());
            List<string> imageFilePaths = new List<string>();
            foreach (string dir in directories)
            {
                string[] fileDir = Directory.GetFiles(dir, "*.jpeg");
                imageFilePaths.AddRange(fileDir);
            }
            return imageFilePaths;
        }

        private ImageModel BuildImageModelFromUrl(IFileCollection fileCollection, string url)
        {
            ImageModel image = new ImageModel
            {
                LocalUrl = url,
                Id = fileCollection.AllImages.Max(x => x.Id) + 1,
                Name = StringExtensions.StringExtensions.GetImageFileName(url)
            };
            //Hacky way of removing the duplicated jpeg from the end of the file name
            image.Name = image.Name.Substring(0, image.Name.Length - 4);
            image.IsDownloaded = true;
            image.Url = "unknwon";
            string interestName = Path.GetDirectoryName(url);
            InterestModel interest = fileCollection.AllInterests.FirstOrDefault(x => x.Name == interestName);
            if (interest != null)
            {
                image.InterestId = interest.Id;
            }
            else
            {
                image.InterestId = 1;
            }
            return image;
        }
    }
}