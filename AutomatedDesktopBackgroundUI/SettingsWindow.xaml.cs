using AutomatedDesktopBackgroundLibrary;
using AutomatedDesktopBackgroundLibrary.StringExtensions;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
        private readonly string path;

        public SettingsWindow()
        {
            string contents = Properties.Resources.ReadMe;
            path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
            path = Path.Combine(path, "ReadMe.txt");
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            FileStream fileStream = new FileStream(path, FileMode.CreateNew);
            using (StreamWriter sw = new StreamWriter(fileStream))
            {
                sw.Write(contents);
            }
            this.Closing += SettingsWindow_Closing;
            InitializeComponent();
            WireupLists();
        }

        private void SettingsWindow_Closing(object sender, CancelEventArgs e)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
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
        }

        private void CollectionRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            string amount = collectionTextBox.Text;
            string timeType = collectionComboBox.SelectedValue.ToString();
            TimeSettings ts = viewController.ConvertStringToTime(timeType);
            viewController.ChangeCollectionRefreshRate(amount, ts);
        }

        private void ChangeBackgroundButton_Click(object sender, RoutedEventArgs e)
        {
            if (!viewController.ChangeDesktopBackground())
            {
                MessageBox.Show("No images are downloaded please download images before attempting to change the wallpaper.");
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
            viewController.ResetApplication();
            resetApplicationButton.IsEnabled = false;
        }

        private void OpenInstructionsButton_Click(object sender, RoutedEventArgs e)
        {
            // string path1 = @"C:\Users\georg\source\repos\AutomatedDesktopBackground\AutomatedDesktopBackground_Library\ReadMe.txt";
            Process.Start("notepad.exe", path);
        }
    }
}