namespace AutomatedDesktopBackgroundLibrary
{
    public enum TimeSettings
    {
        Minutes,
        Hours,
        Days
    }

    public enum PageRefreshState { BGOnly, ColOnly, BGAndCol, None }

    public enum ButtonCommands { StartCollections, StartBackground, StopCollections, StopBackground, SetToStartState }

    public enum FileOperation { Read, Write, Delete, Copy }

    //In the future if we wanted to save somewhere else we can just add the option here.
    public enum Database { Textfile, WebServer, JsonFile };

    public enum CurrentAPIClient { Unsplash };

    public enum JobType { BackgroundRefresh, CollectionRefresh }

    public enum FileType { ImageInfo, InterestInfo, ImageFile }

}