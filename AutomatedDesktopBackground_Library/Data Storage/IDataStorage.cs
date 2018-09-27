namespace AutomatedDesktopBackgroundLibrary
{
    public interface IDataStorage
    {
        IImageFileProcessor ImageFileProcessor { get; set; }

        IInterestFileProcessor InterestFileProcessor { get; set; }

        IDatabaseConnector Database { get; set; }

        IFileCollection FileCollection { get; set; }

        void UpdateAllLists();

        void RegisterFileListeners(IFileListener fileListener);

        void ResetApplication();
    }
}