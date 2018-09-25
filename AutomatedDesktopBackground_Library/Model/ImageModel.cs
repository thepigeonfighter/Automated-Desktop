namespace AutomatedDesktopBackgroundLibrary
{
    public class ImageModel : ISaveable
    {
        public int Id { get; set; }
        public string LocalUrl { get; set; }
        public string Name { get; set; }
        public int InterestId { get; set; }
        public bool IsDownloaded { get; set; }
        public string Url { get; set; }
        private readonly int _interenalId;

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
                if (Id != image.Id)
                {
                    return false;
                }
                if (LocalUrl != image.LocalUrl)
                {
                    return false;
                }
                if (Name != image.Name)
                {
                    return false;
                }
                if (InterestId != image.InterestId)
                {
                    return false;
                }
                if (IsDownloaded != image.IsDownloaded)
                {
                    return false;
                }
                if (Url != image.Url)
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