using AutomatedDesktopBackgroundLibrary;
using AutomatedDesktopBackgroundLibrary.StringExtensions;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using AutomatedDesktopBackgroundLibrary.Utility;
using System.Windows.Input;

namespace AutomatedDesktopBackgroundUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewController viewController = new MainViewController();
        public NotifyIcon ADIcon;

        public MainWindow()
        {
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
        }
        private void GetVersionNumber()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo info = FileVersionInfo.GetVersionInfo(assembly.Location);
            versionLabel.Content = $"v.{info.FileVersion}";
        }

        private void HideWindow()
        {
            Hide();
            ADIcon.Visible = true;
        }

        private void OnNotifyIconDoubleClik(object sender, EventArgs e)
        {
            Show();
            this.WindowState = WindowState.Normal;
            ADIcon.Visible = false;
        }

        private void RemoveInterestButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                viewController.RemoveInterest(interestListView.SelectedValue.ToString());
                interestListView.ItemsSource = viewController.interests;
                interestListView.Items.Refresh();
            }
            catch
            {
                CustomMessageBox.Show("Couldn't remove this interest");
            }
        }

        private void WireDependencies()
        {
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
            ImageModel image = viewController.GetCurrentWallPaper();
            if (image != null)
            {
                currentImageLabel.Content = $"Current image is {image.Name}";
                HateImageButton.IsEnabled = true;
                if (!viewController.IsFavorited())
                {
                    LikeImageButton.IsEnabled = true;
                }
            }
            else
            {
                currentImageLabel.Content = $"Unknown";
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
                    CustomMessageBox.Show("No internet Connection");
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Couldn't make this search");
                CustomMessageBox.Show($"{ex.StackTrace}");
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
                            downloadButton.IsEnabled = false;
                            CustomMessageBox.Show("There are no images associated with that interest, please remove and try a different interest, Sent from MainWindow.cs");
                        }
                    }
                    else
                    {
                        CustomMessageBox.Show("No internet Connection");
                    }
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("The Download Process encountered a bug");
                CustomMessageBox.Show(ex.StackTrace);

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
                Window warningWindow = new WarningWindow();
                warningWindow.Show();
            }
        }
        #region Background and Collection Buttons

        private void StartBackgroundRefreshButton_Copy_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                if (viewController.AreAnyImagesDownloaded())
                {
                    this.Dispatcher.Invoke(() => viewController.StartBackGroundRefresh());
                    viewController.SetPageState(ButtonCommands.StartBackground);
                }
                else
                {
                    CustomMessageBox.Show("No Images downloaded. Please download before some images before attempting to set the background.");
                }
            }
            catch (Exception ex)
            {

                CustomMessageBox.Show("The background refreshing process failed to start");
                CustomMessageBox.Show(ex.StackTrace);
            }
        }

        private void StartCollectionRefreshButton_Copy_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                if (viewController.AreAnyImagesDownloaded())
                {
                    this.Dispatcher.Invoke(() =>
                viewController.StartCollectionRefresh());
                    viewController.SetPageState(ButtonCommands.StartCollections);
                }
                else
                {
                    CustomMessageBox.Show("No Images downloaded. Please download before some images before attempting to set the background.");
                }
            }
            catch (Exception ex)
            {

                CustomMessageBox.Show("Failed to start the collection refreshing process");
                CustomMessageBox.Show(ex.StackTrace);
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
                CustomMessageBox.Show(ex.StackTrace);
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
                CustomMessageBox.Show(ex.StackTrace);
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
            interestListView.ItemsSource = viewController.interests;
            viewController.SetPageState(ButtonCommands.SetToStartState);
            currentImageLabel.Content = "";
            interestListView.Items.Refresh();
        }

        private void EventSystem_ImageHatingHasCompleted(object sender, string e)
        {
            this.Dispatcher.Invoke(() => HateImageButton.IsEnabled = true);
        }

        private void EventSystem_UpdateBackgroundEvent(object sender, string e)
        {
            ImageModel currentWallpaper = viewController.GetCurrentWallPaper();
            if (currentWallpaper.Name != "Unknown")
            {
                this.Dispatcher.Invoke(() =>
                currentImageLabel.Content = $"Current image is {currentWallpaper.Name}"
                );
                this.Dispatcher.Invoke(() => HateImageButton.IsEnabled = true);
                if (viewController.IsFavorited())
                {
                    this.Dispatcher.Invoke(() =>
                    LikeImageButton.IsEnabled = false
                    );
                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    LikeImageButton.IsEnabled = true
                    );
                }
            }
        }

        private void CheckPageState(object sender, string e)
        {
            if (GlobalConfig.IsConnected())
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

                connectionLabel.Content = "Connected";
                connectionLabel.Foreground = System.Windows.Media.Brushes.Green;
            }
            else if (!GlobalConfig.IsConnected())
            {
                connectionLabel.Content = "Offline";
                connectionLabel.Foreground = System.Windows.Media.Brushes.Red;
            }
        }

        private void EventSystem_DownloadPercentageEvent(object sender, int e)
        {
            this.Dispatcher.Invoke(() => downloadProgressBar.Value = e);
        }

        private void EventSystem_DownloadedImageEvent(object sender, string e)
        {
            this.Dispatcher.Invoke(() => amountImagesDownloadedLabel.Content = e);
        }

        private void EventSystem_DownloadCompleteEvent(object sender, bool e)
        {
            if (e) { this.Dispatcher.Invoke(() => CustomMessageBox.Show("Download Complete!")); }
            this.Dispatcher.Invoke(() => downloadButton.IsEnabled = true);
            this.Dispatcher.Invoke(() => amountImagesDownloadedLabel.Content = "");
            this.Dispatcher.Invoke(() => amountImagesDownloadedLabel.Visibility = Visibility.Hidden);
            this.Dispatcher.Invoke(() => downloadProgressBar.Visibility = Visibility.Hidden);
        }

        private void FavoriteAImage_Click(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.Invoke(() => viewController.SetImageAsFavorite());
            this.Dispatcher.Invoke(() => LikeImageButton.IsEnabled = false);
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
                Window window = new SettingsWindow();
                window.Show();
                
                
            }
        }
    }
}