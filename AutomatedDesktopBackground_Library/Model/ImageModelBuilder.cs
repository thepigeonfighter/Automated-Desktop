using AutomatedDesktopBackgroundLibrary.StringExtensions;

namespace AutomatedDesktopBackgroundLibrary
{
    public class ImageModelBuilder
    {
        public ImageModel Build(string imageUrl, InterestModel interest)
        {
            ImageModel image = new ImageModel
            {
                Name = imageUrl.GetImageFileName(),
                Url = imageUrl
            };
            image.LocalUrl = $@"{InternalFileDirectorySystem.ImagesFolder}\{interest.Name}\{image.Name}";
            image.InterestId = interest.Id;
            image.InterestName = interest.Name;
            image.InfoFileDir = $@"{InternalFileDirectorySystem.ImageInfoFolder}\{image.Name.Substring(0, image.Name.Length - 4)}{FileType.ImageInfo.GetFileEnding()}";
            return image;
        }
    }
}