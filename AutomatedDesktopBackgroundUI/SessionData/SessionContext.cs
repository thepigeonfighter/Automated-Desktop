using AutomatedDesktopBackgroundUI.Config;
using AutomatedDesktopBackgroundUI.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
namespace AutomatedDesktopBackgroundUI.SessionData
{
    public class SessionContext : INotifyPropertyChanged, ISessionContext
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public RefreshStateModel CurrentRefreshState
        {
            get { return _currentRefreshStateModel; }
            set
            {
                _currentRefreshStateModel = value;
                OnPropertyChanged(PropertyNames.RefreshState);
            }
        }

        public List<InterestInfoModel> Interests
        {
            get { return _interests; }
            set
            {
                _interests = value;
                OnPropertyChanged(PropertyNames.Interests);
            }
        }

        public InterestInfoModel SelectedInterest
        {
            get { return _selectedInterestModel; }
            set
            {
                _selectedInterestModel = value;
                OnPropertyChanged(PropertyNames.SelectedInterest);
            }
        }

        public CurrentImageModel CurrentWallpaper
        {
            get { return _currentWallpaper; }
            set
            {
                _currentWallpaper = value;
                OnPropertyChanged(PropertyNames.CurrentWallpaper);
            }
        }

        public SettingsModel CurrentSettings
        {
            get { return _currentSettings; }
            set {
                _currentSettings = value;
                OnPropertyChanged(PropertyNames.CurrentSettings);
            }
        }

        public bool IsDownloading
        {
            get
            { return _isDownloading; }
            set
            {
                _isDownloading = value;
                OnPropertyChanged(PropertyNames.IsDownloading);
            }

        }

        private SettingsModel _currentSettings;

        private RefreshStateModel _currentRefreshStateModel;

        private IDataAccess _dataAccess;

        private List<InterestInfoModel> _interests;

        private InterestInfoModel _selectedInterestModel;

        private CurrentImageModel _currentWallpaper;

        private bool _isDownloading;


        public SessionContext(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
            InitializeDataContext();

        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public void ForceSettingsUpdate()
        {
            CurrentSettings = _dataAccess.GetCurrentSettings();
        }
        public void AddInterest(InterestInfoModel item)
        {
            Task.Run(() => _dataAccess.AddInterest(item));
        }
        private void InitializeDataContext()
        {
            Interests = _dataAccess.GetAllInterests();
            CurrentWallpaper = _dataAccess.GetCurrentWallpaper();
            CurrentRefreshState = _dataAccess.GetCurrentRefreshState();
            CurrentSettings = _dataAccess.GetCurrentSettings();
            _dataAccess.PropertyChanged += DataAccessPropertyChanged;

        }

        private void DataAccessPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case PropertyNames.CollectionDownloaded:
                    IsDownloading = false;
                    break;
                case PropertyNames.ImageDownloaded:
                    OnPropertyChanged(e.PropertyName);
                    break;
                case PropertyNames.RefreshState:
                    CurrentRefreshState = _dataAccess.GetCurrentRefreshState();
                    break;
                case PropertyNames.CurrentSettings:
                    UpdateSettings();
                    break;
                default:
                    Interests = _dataAccess.GetAllInterests();
                    CurrentWallpaper = _dataAccess.GetCurrentWallpaper();
                    break;
            }

        }
        private void UpdateSettings()
        {
            CurrentSettings = _dataAccess.GetCurrentSettings();
            _dataAccess.SetRefreshState(new Utility.EventContainer() { Command = CommandNames.UpdateBackgroundCycle, Data = CurrentRefreshState });
        }
    }
}
