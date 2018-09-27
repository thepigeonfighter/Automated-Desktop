namespace AutomatedDesktopBackgroundLibrary
{
    public class InterestModel : ISaveable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TotalImages { get; set; }
        public int TotalPages { get; set; }
        public bool EntireCollectionDownloaded { get; set; }
        public string InfoFileDir { get; set; }
    }
}