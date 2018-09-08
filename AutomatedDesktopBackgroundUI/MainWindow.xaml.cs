using System.Windows;
using AutomatedDesktopBackgroundLibrary;
using Quartz;
using Quartz.Impl;
using System.Threading;
using System;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainViewController viewController = new MainViewController();
        NotifyIcon notifyIcon;
        public MainWindow()
        { 
            InitializeComponent();
            WireEvents();
            BuildNotifyIcon();
            WireListBox();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            System.Windows.MessageBox.Show("Program only closes when the close program button is clicked");
                e.Cancel = true;
                HideWindow();
        }
        /// <summary>
        /// I am leaving this in the code behind because it is specific to this UI
        /// This Builds a system tray icon so when the app is minimized it becomes a visible icon in the system tray
        /// </summary>
        private void BuildNotifyIcon()
        {
            notifyIcon = new NotifyIcon();
            notifyIcon.BalloonTipTitle = "Automated Desktop Backgrounds";
            notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            notifyIcon.BalloonTipText = "Double Click To Open";
            notifyIcon.Text = "Automated Desktop Backgrounds";
            notifyIcon.Visible = false;
            string currentDir = Environment.CurrentDirectory;
            //TODO Remove switch to new path when building the app
             string path = @"C:\Users\georg\source\repos\AutomatedDesktopBackground\AutomatedDesktopBackgroundUI\Images\Icon.ico";

            //string path = Path.Combine(currentDir, @"Images\Icon.ico");
           // string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\Icon.ico");
            Icon imageSource = new Icon(path);
            notifyIcon.Icon = imageSource;
            notifyIcon.DoubleClick += OnNotifyIconDoubleClik;
            
        }
        private void OnResize(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized )
            {
                HideWindow();
                
            }
        }
        private void HideWindow()
        {
            Hide();
            notifyIcon.Visible = true;
        }
        private void OnNotifyIconDoubleClik(object sender, EventArgs e)
        {
            Show();
            this.WindowState = WindowState.Normal;
            notifyIcon.Visible = false;
        }
    
        private void removeInterestButton_Click(object sender, RoutedEventArgs e)
        {
            viewController.RemoveInterest(interestListBox.SelectedValue.ToString());
            interestListBox.ItemsSource = viewController.interests;
            interestListBox.Items.Refresh();
        }
        private void WireListBox()
        {
            amountImagesDownloadedLabel.Visibility = Visibility.Hidden;
            downloadProgressBar.Visibility = Visibility.Hidden;
            queryTextBox.Text = "";
            viewController.RefreshInterestList();
            interestListBox.DataContext = viewController.interests;
            interestListBox.DisplayMemberPath = "Name";
            interestListBox.SelectedValuePath = "Name";
            interestListBox.ItemsSource = viewController.interests;
            EventSystem_ConfigSettingChangedEvent(this, "");
            if (viewController.GetCurrentWallPaperFromFile().Id != -1 )
            {
                currentImageLabel.Content = $"Current image is {viewController.GetCurrentWallPaperFromFile().Name}";
            }
        }


        private void querySearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(queryTextBox.Text))
            {
                viewController.AddInterest(queryTextBox.Text);
                queryTextBox.Text = "";
            }
            viewController.RefreshInterestList();
            interestListBox.ItemsSource = viewController.interests;
            interestListBox.Items.Refresh();
        }



        private void downloadButton_Click(object sender, RoutedEventArgs e)
        {
            if (interestListBox.SelectedValue != null)
            {
                viewController.DownloadNewCollection(interestListBox.SelectedValue.ToString());
                downloadButton.IsEnabled = false;
                amountImagesDownloadedLabel.Visibility = Visibility.Visible;
                downloadProgressBar.Visibility = Visibility.Visible;
            }
        }


        private void settingsButton_Click(object sender, RoutedEventArgs e)
        {
            Window window = new SettingsWindow();
            window.Show();
        }

        private void closeProgramButton_Click(object sender, RoutedEventArgs e)
        {
            viewController.CloseProgram();
            
        }

        private   void stopCollectionRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.Invoke(()=>
            viewController.StopCollectionChange());
        }

        private  void stopBackgroundRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.Invoke(()=>
            viewController.StopBackGroundRefresh());

        }

        private void startBackgroundRefreshButton_Copy_Click(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
             viewController.StartBackGroundRefresh()
             );
        }
        private void WireEvents()
        {
            this.StateChanged += OnResize;
            this.Closing += MainWindow_Closing;
            GlobalConfig.EventSystem.DownloadCompleteEvent += EventSystem_DownloadCompleteEvent;
            GlobalConfig.EventSystem.DownloadedImageEvent += EventSystem_DownloadedImageEvent;
            GlobalConfig.EventSystem.DownloadPercentageEvent += EventSystem_DownloadPercentageEvent;
            GlobalConfig.EventSystem.ConfigSettingChangedEvent += EventSystem_ConfigSettingChangedEvent;
            GlobalConfig.EventSystem.UpdateBackgroundEvent += EventSystem_UpdateBackgroundEvent;
        }

        private  void EventSystem_UpdateBackgroundEvent(object sender, string e)
        {
            if (GlobalConfig.CurrentWallpaper != null)
            {
                ImageModel currentWallpaper = GlobalConfig.CurrentWallpaper;
                
                this.Dispatcher.Invoke(() =>
                currentImageLabel.Content = $"Current image is {currentWallpaper.Name}"
                );
            }
        }

        private void EventSystem_ConfigSettingChangedEvent(object sender, string e)
        {
            if (GlobalConfig.BackGroundUpdating)
            {

                this.Dispatcher.Invoke(()=>backgroundRefreshLabel.Content = "Background is refreshing");
                this.Dispatcher.Invoke(() => stopBackgroundRefreshButton.IsEnabled = true);
                this.Dispatcher.Invoke(() => startBackgroundRefreshButton_Copy.IsEnabled = false);
       
            }
            else
            {
                this.Dispatcher.Invoke(() => backgroundRefreshLabel.Content = "Background not refreshing");
                this.Dispatcher.Invoke(() => stopBackgroundRefreshButton.IsEnabled = false);
                this.Dispatcher.Invoke(() => startBackgroundRefreshButton_Copy.IsEnabled = true);
     
            }
            if (GlobalConfig.CollectionUpdating)
            {
                this.Dispatcher.Invoke(() => collectionRefreshLabel.Content = "Collections are refreshing");
                this.Dispatcher.Invoke(() => stopCollectionRefreshButton.IsEnabled = true);
                this.Dispatcher.Invoke(() => startCollectionRefreshButton_Copy.IsEnabled = false);
        
            }
            else
            {
                this.Dispatcher.Invoke(() => collectionRefreshLabel.Content = "Collections not refreshing");
                this.Dispatcher.Invoke(() => stopCollectionRefreshButton.IsEnabled = false);
                this.Dispatcher.Invoke(() => startCollectionRefreshButton_Copy.IsEnabled = true);

            }

        }

        private void EventSystem_DownloadPercentageEvent(object sender, int e)
        {
            downloadProgressBar.Value = e;
        }

        private void EventSystem_DownloadedImageEvent(object sender, bool e)
        {
            if (e)
            {
                if (!string.IsNullOrEmpty(amountImagesDownloadedLabel.Content.ToString()))
                {
                    string content = amountImagesDownloadedLabel.Content.ToString();
                    string number = content[0].ToString();
                    int currentAmount = int.Parse(number) + 1;
                    string newMessage = $"{currentAmount}/10";
                    amountImagesDownloadedLabel.Content =newMessage;
                }
                else
                {
                    amountImagesDownloadedLabel.Content = "1/10";
                }
            }
        }

        private void EventSystem_DownloadCompleteEvent(object sender, bool e)
        {
            downloadButton.IsEnabled = true;
            amountImagesDownloadedLabel.Content = "";
            amountImagesDownloadedLabel.Visibility = Visibility.Hidden;
            downloadProgressBar.Visibility = Visibility.Hidden;
        }
    }
    
}
