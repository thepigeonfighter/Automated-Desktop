using System.IO;

namespace AutomatedDesktopBackgroundLibrary
{
    public class DirectoryNavigator
    {
        private readonly static string _imageInfoFolder = InternalFileDirectorySystem.ImageInfoFolder;

        private readonly static string _interestInfoFolder = InternalFileDirectorySystem.InterestInfoFolder;

        private readonly static string _imagesFolder = InternalFileDirectorySystem.ImagesFolder;

        private readonly static string _aplicationDirectory = InternalFileDirectorySystem.ApplicationDirectory;

        public DirectoryNavigator()
        {
            InitializeStartingDirectories();
        }

        private void InitializeStartingDirectories()
        {
            Directory.CreateDirectory(_imageInfoFolder);
            Directory.CreateDirectory(_interestInfoFolder);
            Directory.CreateDirectory(_imagesFolder);
        }

        protected string[] GetAllDirectoriesInRootFile()
        {
            return Directory.GetDirectories(_aplicationDirectory);
        }

        protected string[] GetAllInterestInfoFiles(FileType type)
        {
            Directory.CreateDirectory(_interestInfoFolder);
            return Directory.GetFiles(_interestInfoFolder, $"*{type.GetFileEnding()}");
        }

        protected string[] GetAllImageFilesInDirectory(FileType type)
        {
            Directory.CreateDirectory(_imagesFolder);
            return Directory.GetFiles(_imagesFolder, $"*{type.GetFileEnding()}");
        }

        protected string[] GetAllImageInfoFilesInDirectory(FileType type)
        {
            Directory.CreateDirectory(_imageInfoFolder);
            return Directory.GetFiles(_imageInfoFolder, $"*{type.GetFileEnding()}");
        }
    }
}