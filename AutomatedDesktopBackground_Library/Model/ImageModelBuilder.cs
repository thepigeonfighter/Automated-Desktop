using AutomatedDesktopBackgroundLibrary.StringExtensions;

namespace AutomatedDesktopBackgroundLibrary
{
    public class ImageModelBuilder
    {
        public ImageModel Build(string imageUrl,string description, InterestModel interest)
        {
            ImageModel image = new ImageModel
            {
                Name = description.MakePrettyString(),
                Url = imageUrl
            };
            string nameWithoutwhiteSpace = image.Name.Replace(" ", "") +".JPEG";
            image.LocalUrl = $@"{InternalFileDirectorySystem.ImagesFolder}\{interest.Name}\{nameWithoutwhiteSpace}";
            image.InterestId = interest.Id;
            image.InterestName = interest.Name;
            image.InfoFileDir = $@"{InternalFileDirectorySystem.ImageInfoFolder}\{nameWithoutwhiteSpace.Substring(0, nameWithoutwhiteSpace.Length - 4)}{FileType.ImageInfo.GetFileEnding()}";
            return image;
        }
    }
}