using AutomatedDesktopBackgroundLibrary;
using AutomatedDesktopBackgroundLibrary.StringExtensions;
using AutomatedDesktopBackgroundLibrary.Utility;
using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace AutomatedDesktopBackgroundUI
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private readonly List<string> timeSettings = new List<string>();
        private readonly SettingsViewController viewController = new SettingsViewController();
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string path;

        public SettingsWindow()
        {
            log.Debug("Settings window opened");
            string contents = Properties.Resources.ReadMe;
            path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
            path = Path.Combine(path, "ReadMe.txt");
            if (File.Exists(path))
            {
                log.Info("Deleting old file name ReadMe.txt");
                File.Delete(path);
            }
            try
            {
                FileStream fileStream = new FileStream(path, FileMode.CreateNew);
                using (StreamWriter sw = new StreamWriter(fileStream))
                {
                    sw.Write(contents);
                }
                log.Debug("Sucessfully created a readme file");
            }
            catch(Exception e)
            {
                log.Error("Failed to create a readme file");
                log.Info(e.InnerException);
            }
            this.Closing += SettingsWindow_Closing;
            InitializeComponent();
            WireupLists();
        }

        private void SettingsWindow_Closing(object sender, CancelEventArgs e)
        {
            log.Debug("Settings window close requested");
            if (File.Exists(path))
            {
                log.Debug("Found  readme file");
                try
                {
                    File.Delete(path);
                    log.Info("Deleted readme file");
                }
                catch(Exception ex)
                {
                    log.Warn("Readme file has not been removed");
                    log.Info(ex.InnerException);
                }
            }
        }

        private void WireupLists()
        {
            timeSettings.Add(nameof(TimeSettings.Days));
            timeSettings.Add(nameof(TimeSettings.Minutes));
            timeSettings.Add(nameof(TimeSettings.Hours));
            backgroundCombobox.ItemsSource = timeSettings;
            collectionComboBox.ItemsSource = timeSettings;
            SetTimes();
            fileSavePathLabel.Content = StringExtensions.GetApplicationDirectory();
        }

        private void BackgroundRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            
            string amount = backgroundRefreshTextBox.Text;
            string timeType = backgroundCombobox.SelectedValue.ToString();
            TimeSettings ts = viewController.ConvertStringToTime(timeType);
            viewController.ChangeBackgroundRefreshRate(amount, ts);
            log.Info($"User has changed the background refresh time to {amount} {timeType} ");
        }

        private void CollectionRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            string amount = collectionTextBox.Text;
            string timeType = collectionComboBox.SelectedValue.ToString();
            TimeSettings ts = viewController.ConvertStringToTime(timeType);
            viewController.ChangeCollectionRefreshRate(amount, ts);
            log.Info($"User has changed the collection refresh time to {amount} {timeType} ");
        }

        private void ChangeBackgroundButton_Click(object sender, RoutedEventArgs e)
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

        private void CheckIfNumber(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !viewController.IsTextAllowed(e.Text);
        }

        private void SetTimes()
        {
            TimeModel backgroundTime = viewController.CurrentBackgroundRefreshRate();
            TimeModel collectionTime = viewController.CurrentCollectionRefreshRate();
            int backgroundIndex = timeSettings.FindIndex(x => x == backgroundTime.TimeSetting.ToString());
            int collectionIndex = timeSettings.FindIndex(x => x == collectionTime.TimeSetting.ToString());
            backgroundRefreshTextBox.Text = backgroundTime.Amount.ToString();
            backgroundCombobox.SelectedIndex = backgroundIndex;
            collectionTextBox.Text = collectionTime.Amount.ToString();
            collectionComboBox.SelectedIndex = collectionIndex;
        }

        private void ResetApplicationButton_Click(object sender, RoutedEventArgs e)
        {
            log.Info("User reseting application");
            viewController.ResetApplication();
            resetApplicationButton.IsEnabled = false;
        }

        private void OpenInstructionsButton_Click(object sender, RoutedEventArgs e)
        {      
            Process.Start("notepad.exe", path);
        }

        private void OnCloseSettingsClick(object sender, RoutedEventArgs e)
        {
            log.Debug("Settings window being closed");
            this.Close();
        }

        private string GetAssembly()
        {
            var path = Assembly.GetExecutingAssembly().Location;
            return path;
        }
        //TODO Change this to where the check box will be disabled until the user has downloaded
        //The change once button 
        private void CheckBoxChanged(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(InternalFileDirectorySystem.ChangeBackgroundOnceSource))
            {
                CustomMessageBox.Show("You have not installed the change background once exe. Without it installed this feature does not work");
            }
            else {

                if (!WindowsShellExtension.IsElevated())
                {
                    log.Info("User does not have elevated permissions");
                    try
                    {
                        SettingsModel newSettings = new SettingsModel().LoadSettings();
                        newSettings.StartWithSettingsWindowOpen = true;
                        newSettings.SaveSettings(newSettings);
                        log.Info("Attempting to elevated permissions");
                        WindowManager.CloseRootWindow();
                        WindowsShellExtension.RunAsAdmin(GetAssembly());
                    }
                    catch(Exception ex)
                    {
                        log.Error("Something went wrong when trying to elevate permissions");
                        log.Info(ex.InnerException);
                    }
                }
                else
                {
                    try
                    {
                        WindowsShellExtension shell = new WindowsShellExtension();
                        bool value = contextMenuCheckBox.IsChecked.HasValue ? contextMenuCheckBox.IsChecked.Value : false;
                        if (value)
                        {
                            log.Info("Creating the context menu button");
                            shell.CreateMenuOption(GetAssembly());
                        }
                        else
                        {
                            log.Info("Removing the context menu button");
                            shell.RemoveMenuOption(GetAssembly());
                        }
                    }
                    catch(Exception ex)
                    {
                        log.Error("Failed to add/remove context menu button");
                        log.Info(ex.InnerException);
                    }
                   
                }

            }
            try
            {
                SettingsModel settings = new SettingsModel().LoadSettings();
                settings.StartWithSettingsWindowOpen = false;
                settings.SaveSettings(settings);
                log.Info("Sucessfully updated the settings");
            }
            catch(Exception ex)
            {
                log.Error("Failed to update settings");
                log.Info(ex.InnerException);
            }
        }
    }
}