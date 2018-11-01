using System;
using System.IO;

namespace AutomatedDesktopBackgroundLibrary
{
    public static class InternalFileDirectorySystem
    {
        private const string interestInfoFileEnding = ".intInfo";

        private const string imageInfoFileEnding = ".imgInfo";

        private const string imageFileEnding = ".JPEG";

        private const string _imageInfoFolderName = "Image Info";

        private const string _interestInfoFolderName = "Interest Info";

        private const string _imagesFolder = "Images";

        private const string _settings = "Settings.txt";

        public readonly static string SettingsFile = _settings.FullFilePath();

        public readonly static string ImageInfoFolder = _imageInfoFolderName.FullFilePath();

        public readonly static string InterestInfoFolder = _interestInfoFolderName.FullFilePath();

        public readonly static string ImagesFolder = _imagesFolder.FullFilePath();

        public readonly static string ChangeBackgroundOnceSource = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +@"\ChangeBackGroundOnce\ChangeBackgroundOnce.exe";

        public readonly static string ApplicationDirectory = FileSavePath;

        public static string GetFileEnding(this FileType file)
        {
            switch (file)
            {
                case FileType.ImageInfo:
                    return imageInfoFileEnding;

                case FileType.InterestInfo:
                    return interestInfoFileEnding;

                case FileType.ImageFile:
                    return imageFileEnding;

                default:
                    return "ERROR";
            }
        }

        private static string FileSavePath
        {
            get
            {
                string baseUrl = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string fullUrl = baseUrl + @"\DesktopBackgrounds";
                return Directory.CreateDirectory(fullUrl).FullName;
            }
            set { FileSavePath = value; }
        }

        public static string FullFilePath(this string fileName)
        {
            return $@"{FileSavePath}\{fileName}";
        }
    }
}