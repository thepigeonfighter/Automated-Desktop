
using AutomatedDesktopBackgroundLibrary;
using AutomatedDesktopBackgroundLibrary.StringExtensions;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using AutomatedDesktopBackgroundLibrary.Utility;
using System.Windows.Input;
using log4net;
using System.Threading.Tasks;
using Squirrel;
using System.Linq;

namespace AutomatedDesktopBackgroundUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewController viewController = new MainViewController();
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public NotifyIcon ADIcon;

        public MainWindow()
        {
            log.Debug("Entering the main window ctor");
            InitializeComponent();
            GetVersionNumber();
            CheckSettings();
            WireEvents();
            BuildNotifyIcon();
            WireDependencies();

        }

        /// <summary>
        /// I am leaving this in the code behind because it is specific to this UI
        /// This Builds a system tray icon so when the app is minimized it becomes a visible icon in the system tray
        /// </summary>
        private void BuildNotifyIcon()
        {
            log.Debug("Building notify icon");
            ADIcon = new NotifyIcon
            {
                BalloonTipTitle = "Automated Desktop Backgrounds",
                BalloonTipIcon = ToolTipIcon.Info,
                BalloonTipText = "Double Click To Open",
                Text = "Automated Desktop Backgrounds",
                Visible = false
            };
            var myIcon = Properties.Resources.Icon1;
            ADIcon.Icon = myIcon;
            ADIcon.DoubleClick += OnNotifyIconDoubleClik;
            log.Debug("Notify icon built");
        }
        private void GetVersionNumber()
        {
            log.Debug("Getting version number");
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo info = FileVersionInfo.GetVersionInfo(assembly.Location);
            versionLabel.Content = $"v.{info.FileVersion}";
        }

        private void HideWindow()
        {
            log.Debug("Hide window requested");
            Hide();
            ADIcon.Visible = true;
        }

        private void OnNotifyIconDoubleClik(object sender, EventArgs e)
        {
            log.Debug("Notify icon has been double clicked");
            Show();
            this.WindowState = WindowState.Normal;
            ADIcon.Visible = false;
        }

        private void RemoveInterestButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string interestToRemove = interestListView.SelectedValue.ToString();
                log.Debug($"The interest named {interestToRemove} is trying to be removed ");
                viewController.RemoveInterest(interestToRemove);
                interestListView.ItemsSource = viewController.interests;
                interestListView.Items.Refresh();
                log.Info($"The interest named {interestToRemove} has been removed");
            }
            catch(Exception ex)
            {

                log.Warn($"The interest named {interestListView.SelectedValue.ToString()} could not be found");
                CustomMessageBox.Show("Couldn't remove this interest");
            }
        }

        private void WireDependencies()
        {
            log.Debug("Setting window state to start state");
            viewController.SetPageState(ButtonCommands.SetToStartState);
            amountImagesDownloadedLabel.Visibility = Visibility.Hidden;
            downloadProgressBar.Visibility = Visibility.Hidden;
            queryTextBox.Text = "";
            interestListView.ItemsSource = viewController.interests;
            interestListView.SelectedValuePath = "Name";
            EventSystem_UpdateBackgroundEvent(this, "");
            downloadButton.IsEnabled = false;
            removeInterestButton.IsEnabled = false;

            queryTextBox.KeyUp += QueryTextBox_KeyUp;
            log.Debug("Getting the current wallpaper");
            ImageModel image = viewController.GetCurrentWallPaper();
            if (image != null)
            {
                log.Info("Have found an existing wallpaper");
                currentImageTextBlock.Text = $"Current image: \"{image.Name}\"";
                HateImageButton.IsEnabled = true;
                if (!viewController.IsFavorited())
                {
                    LikeImageButton.IsEnabled = true;
                }
            }
            else
            {
                log.Info("No previous wallpaper found");
                currentImageTextBlock.Text = "Unknown";
                HateImageButton.IsEnabled = false;
                LikeImageButton.IsEnabled = false;
            }
        }

        private void QueryTextBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                QuerySearchButton_Click(this, new RoutedEventArgs());
            }
        }

        private async void QuerySearchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (GlobalConfig.IsConnected())
                {
                    log.Info($"user searched for {queryTextBox.Text} ");
                    querySearchButton.IsEnabled = false;
                    if (!string.IsNullOrEmpty(queryTextBox.Text))
                    {
                        string formatedText = queryTextBox.Text.MakePrettyString();
                        if (!viewController.InterestExists(formatedText))
                        {
                            await viewController.AddInterest(formatedText);
                            queryTextBox.Text = "";
                        }
                        else
                        {
                            queryTextBox.Text = "";
                        }
                    }

                    interestListView.ItemsSource = viewController.interests;
                    interestListView.Items.Refresh();
                    querySearchButton.IsEnabled = true;
                }
                else
                {
                    log.Info($"User attempted to search for {queryTextBox.Text} but was not connected");
                    CustomMessageBox.Show("No internet Connection");
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Couldn't make this search");
                log.Error($"{queryTextBox.Text} was searched resulting in an error");
                log.Info($"{ex.InnerException}");
            }
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (interestListView.SelectedValue != null)
                {
                    if (GlobalConfig.IsConnected())
                    {
                        log.Debug($"User is downloading a collection associated with {interestListView.SelectedValue.ToString()}");
                        int totalImages = viewController.GetInterestTotalImages(interestListView.SelectedValue.ToString());
                        if (totalImages > 0)
                        {
                            
                            viewController.DownloadNewCollection(interestListView.SelectedValue.ToString());
                            downloadButton.IsEnabled = false;
                            amountImagesDownloadedLabel.Visibility = Visibility.Visible;
                            downloadProgressBar.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            log.Info("User attempted to download images that had zero images");
                            downloadButton.IsEnabled = false;
                            CustomMessageBox.Show("There are no images associated with that interest, please remove and try a different interest, Sent from MainWindow.cs");
                        }
                    }
                    else
                    {
                        log.Debug("User attempted to download without being connected to the internet");
                        CustomMessageBox.Show("No internet Connection");
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error($"Something exploded when the user attempted to download {interestListView.SelectedValue.ToString()}");
                CustomMessageBox.Show("The Download Process encountered a bug");
                log.Info(ex.InnerException);

            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Window window = new SettingsWindow();
            window.Show();
        }

        private void CloseWindowButtonClick(object sender, RoutedEventArgs e)
        {
            HideWindow();
            DisplayWarningWindow();
        }

        private void DisplayWarningWindow()
        {
            
            if (viewController.ShouldDisplayWarning())
            {
                log.Info("User will be warned about the program running in background");
                Window warningWindow = new WarningWindow();
                warningWindow.Show();
            }
            else { log.Info("User will not be warned about the program running in background"); }
        }
        #region Background and Collection Buttons

        private void StartBackgroundRefreshButton_Copy_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                if (viewController.AreAnyImagesDownloaded())
                {
                    Task.Run(() => viewController.StartBackGroundRefresh()).ConfigureAwait(false);
                }
                else
                {
                    CustomMessageBox.Show("No Images downloaded. Please download before some images before attempting to set the background.");
                }
            }
            catch (Exception ex)
            {

                CustomMessageBox.Show("The background refreshing process failed to start");
                log.Error("Background refresh has failed to start");
                log.Info(ex.InnerException);
            }
        }

        private void StartCollectionRefreshButton_Copy_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                if (!GlobalConfig.IsConnected())
                {
                    CustomMessageBox.Show("Not Connected to the internet. Please connect before attempting to refresh your collections");
                }
                else
                {
                    if (viewController.AreAnyImagesDownloaded())
                    {

                        Task.Run(() => viewController.StartCollectionRefresh()).ConfigureAwait(false);

                    }
                    else
                    {
                        CustomMessageBox.Show("No Images downloaded. Please download before some images before attempting to set the background.");
                    }
                }
            }
            catch (Exception ex)
            {

                CustomMessageBox.Show("Failed to start the collection refreshing process");
                log.Error("Collection refresh has failed to start");
                log.Info(ex.InnerException);
            }
        }

        private void StopCollectionRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Dispatcher.Invoke(() =>
                viewController.StopCollectionChange());
                viewController.SetPageState(ButtonCommands.StopCollections);
            }
            catch (Exception ex)
            {

                CustomMessageBox.Show("Failed to stop the collection refresh process");
                log.Error("Collection refresh has failed to stop");
                log.Info(ex.InnerException);
            }
        }

        private void StopBackgroundRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Dispatcher.Invoke(() =>
                    viewController.StopBackGroundRefresh());
                viewController.SetPageState(ButtonCommands.StopBackground);
            }
            catch (Exception ex)
            {

                CustomMessageBox.Show("Failed to stop the background refresh job");
                log.Error("Background refresh has failed to stop");
                log.Info(ex.InnerException);
            }
        }

        #endregion Background and Collection Buttons

        private void WireEvents()
        {

            GlobalConfig.EventSystem.DownloadCompleteEvent += EventSystem_DownloadCompleteEvent;
            GlobalConfig.EventSystem.DownloadedImageEvent += EventSystem_DownloadedImageEvent;
            GlobalConfig.EventSystem.DownloadPercentageEvent += EventSystem_DownloadPercentageEvent;
            GlobalConfig.EventSystem.UpdateBackgroundEvent += EventSystem_UpdateBackgroundEvent;
            GlobalConfig.EventSystem.ImageHatingHasCompletedEvent += EventSystem_ImageHatingHasCompleted;
            GlobalConfig.EventSystem.ApplicationResetEvent += EventSystem_ApplicationResetEvent;
            GlobalConfig.EventSystem.OnErrorsEncounteredDuringDownloadEvent += ErrorsEncounteredEvent;
            interestListView.MouseDoubleClick += InterestListView_MouseDoubleClick;
            viewController.OnPageStateChange += CheckPageState;
            this.MouseLeftButtonDown += DragWindow;
        }
        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }

        private void ErrorsEncounteredEvent(object sender, string e)
        {
            this.Dispatcher.Invoke(() => CustomMessageBox.Show(e));
        }

        private void InterestListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (interestListView.SelectedValue != null)
            {
                string folderName = interestListView.SelectedValue.ToString().MakePrettyString();
                string filePath = InternalFileDirectorySystem.ImagesFolder + $@"\{folderName}";
                ProcessStartInfo info = new ProcessStartInfo()
                {
                    FileName = "explorer.exe",
                    Arguments = filePath
                };
                Process.Start(info);
            }
        }

        private void EventSystem_ApplicationResetEvent(object sender, string e)
        {
            log.Info("Application reset has been triggered");
            interestListView.ItemsSource = viewController.interests;
            viewController.SetPageState(ButtonCommands.SetToStartState);
            currentImageTextBlock.Text = "";
            interestListView.Items.Refresh();
        }

        private void EventSystem_ImageHatingHasCompleted(object sender, string e)
        {
            log.Info("Image has been successfully marked as hated");
            this.Dispatcher.Invoke(() => HateImageButton.IsEnabled = true);
        }

        private void EventSystem_UpdateBackgroundEvent(object sender, string e)
        {
            log.Debug("Wallpaper changed");
            ImageModel currentWallpaper = viewController.GetCurrentWallPaper();
            if (currentWallpaper.Name != "Unknown")
            {
                this.Dispatcher.Invoke(() =>
                currentImageTextBlock.Text = $"Current image : \"{currentWallpaper.Name}\""
                );
                this.Dispatcher.Invoke(() => HateImageButton.IsEnabled = true);
                if (viewController.IsFavorited())
                {
                    this.Dispatcher.Invoke(() =>
                    LikeImageButton.Visibility = Visibility.Hidden
                    );
                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    LikeImageButton.Visibility = Visibility.Visible
                    );
                }
            }
        }

        private void CheckPageState(object sender, string e)
        {
            if (GlobalConfig.IsConnected())
            {
                SetButtonState();
                this.Dispatcher.Invoke(()=>connectionLabel.Content = "Connected");
                this.Dispatcher.Invoke(()=>connectionLabel.Foreground = System.Windows.Media.Brushes.Green);
            }
            else if (!GlobalConfig.IsConnected())
            {
              this.Dispatcher.Invoke(()=>  connectionLabel.Content = "Offline");
              this.Dispatcher.Invoke(()=> connectionLabel.Foreground = System.Windows.Media.Brushes.Red);
              SetButtonState();
            }
        }
        private void SetButtonState()
        {
            switch (viewController.refreshState)
            {
                case PageRefreshState.BGAndCol:
                    BGUpdating(true);
                    ColUpdating(true);
                    break;

                case PageRefreshState.BGOnly:
                    BGUpdating(true);
                    ColUpdating(false);
                    break;

                case PageRefreshState.ColOnly:
                    BGUpdating(false);
                    ColUpdating(true);
                    break;

                case PageRefreshState.None:
                    BGUpdating(false);
                    ColUpdating(false);
                    break;
            }
        }

        private void EventSystem_DownloadPercentageEvent(object sender, int e)
        {
           
            this.Dispatcher.Invoke(() => downloadProgressBar.Value = e);
        }

        private void EventSystem_DownloadedImageEvent(object sender, string e)
        {
            log.Debug("Downloaded an image");
            this.Dispatcher.Invoke(() => amountImagesDownloadedLabel.Content = e);
        }

        private void EventSystem_DownloadCompleteEvent(object sender, bool e)
        {
            log.Debug("Collection has finished downloading");
            if (e) { this.Dispatcher.Invoke(() => CustomMessageBox.Show("Download Complete!")); }
            this.Dispatcher.Invoke(() => downloadButton.IsEnabled = true);
            this.Dispatcher.Invoke(() => amountImagesDownloadedLabel.Content = "");
            this.Dispatcher.Invoke(() => amountImagesDownloadedLabel.Visibility = Visibility.Hidden);
            this.Dispatcher.Invoke(() => downloadProgressBar.Visibility = Visibility.Hidden);
        }

        private void FavoriteAImage_Click(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.Invoke(() => viewController.SetImageAsFavorite());
            this.Dispatcher.Invoke(() => LikeImageButton.Visibility= Visibility.Hidden);
        }

        private void HateImage_Click(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.Invoke(() => viewController.SetImageAsHated());
            this.Dispatcher.Invoke(() => HateImageButton.IsEnabled = false);
        }

        private void InterestListView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (interestListView.SelectedValue != null)
            {
                if (GlobalConfig.IsConnected())
                {
                    downloadButton.IsEnabled = true;
                    removeInterestButton.IsEnabled = true;

                    queryTextBox.Text = interestListView.SelectedValue.ToString();
                }
            }
            else
            {
                downloadButton.IsEnabled = false;
                removeInterestButton.IsEnabled = false;
            }
        }

        private void BGUpdating(bool status)
        {
            if (status)
            {
                this.Dispatcher.Invoke(() => backgroundRefreshLabel.Content = "Background is refreshing");
                this.Dispatcher.Invoke(() => stopBackgroundRefreshButton.IsEnabled = true);
                this.Dispatcher.Invoke(() => startBackgroundRefreshButton.IsEnabled = false);
            }
            else
            {
                this.Dispatcher.Invoke(() => backgroundRefreshLabel.Content = "Background not refreshing");
                this.Dispatcher.Invoke(() => stopBackgroundRefreshButton.IsEnabled = false);
                this.Dispatcher.Invoke(() => startBackgroundRefreshButton.IsEnabled = true);
            }
        }

        private void ColUpdating(bool status)
        {
            if (status)
            {
                this.Dispatcher.Invoke(() => collectionRefreshLabel.Content = "Collections are refreshing");
                this.Dispatcher.Invoke(() => stopCollectionRefreshButton.IsEnabled = true);
                this.Dispatcher.Invoke(() => startCollectionRefreshButton.IsEnabled = false);
            }
            else
            {
                this.Dispatcher.Invoke(() => collectionRefreshLabel.Content = "Collections not refreshing");
                this.Dispatcher.Invoke(() => stopCollectionRefreshButton.IsEnabled = false);
                this.Dispatcher.Invoke(() => startCollectionRefreshButton.IsEnabled = true);
            }
        }

        private void ExitApplicationButtonClick(object sender, RoutedEventArgs e)
        {
            log.Info("Application close requested");
            this.Dispatcher.Invoke(() => viewController.CloseWindow());
        }

        private void OnMinimizeClick(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void CheckSettings()
        {
            if(viewController.ShowSettingsWindow())
            {
                log.Info("Setting window should be shown at start up");
                Window window = new SettingsWindow();
                window.Show();
                
                
            }
        }

        private void NextBackground_Click(object sender, RoutedEventArgs e)
        {
            if (!viewController.ChangeDesktopBackground())
            {
                log.Debug("Settings window did not find any photos");
                CustomMessageBox.Show("No images are downloaded please download images before attempting to change the wallpaper.");
            }
            else
            {
                log.Info("User has forced that background to be changed");
            }
        }




    }
}