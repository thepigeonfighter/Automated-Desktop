namespace AutomatedDesktopBackgroundLibrary
{
    public class ImageModel : ISaveable
    {
        public int Id { get; set; }
        public string LocalUrl { get; set; }
        public string Name { get; set; }
        public string InfoFileDir { get; set; }
        public int InterestId { get; set; }
        public bool IsDownloaded { get; set; }
        public string Url { get; set; }
        private readonly int _interenalId;
        public bool IsHated { get; set; } = false;
        public bool IsFavorite { get; set; } = false;
        public string InterestName { get; set; }
        public bool IsWallpaper { get; set; }

        public ImageModel()
        {
            _interenalId = (Id * 350) ^ InterestId;
        }

        public override bool Equals(object obj)
        {
            if (obj?.GetType().Equals(this.GetType()) != true)
            {
                return false;
            }
            else
            {
                ImageModel image = obj as ImageModel;

                if (LocalUrl != image.LocalUrl)
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            return _interenalId;
        }
    }
}