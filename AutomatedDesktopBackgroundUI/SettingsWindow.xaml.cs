using AutomatedDesktopBackgroundLibrary;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace AutomatedDesktopBackgroundUI
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        List<string> timeSettings = new List<string>();
        SettingsViewController viewController = new SettingsViewController();
        public SettingsWindow()
        {
            InitializeComponent();
            WireupLists();
        }
        private void WireupLists()
        {
            timeSettings.Add(GlobalConfig.TimeSettings.Days.ToString());
            timeSettings.Add(GlobalConfig.TimeSettings.Minutes.ToString());
            timeSettings.Add(GlobalConfig.TimeSettings.Hours.ToString());

            backgroundCombobox.ItemsSource = timeSettings;
            collectionComboBox.ItemsSource = timeSettings;
            SetTimes();
            filePathTextBox.Text = GlobalConfig.FileSavePath.ToString();
        }

        private void backgroundRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            string amount = backgroundRefreshTextBox.Text;
            string timeType = backgroundCombobox.SelectedValue.ToString();
            GlobalConfig.TimeSettings ts = viewController.ConvertStringToTime(timeType);
            viewController.ChangeBackgroundRefreshRate(amount, ts);
        }

        private void collectionRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            string amount = collectionTextBox.Text;
            string timeType = collectionComboBox.SelectedValue.ToString();
            GlobalConfig.TimeSettings ts = viewController.ConvertStringToTime(timeType);
            viewController.ChangeCollectionRefreshRate(amount, ts);

        }

        private void changeBackgroundButton_Click(object sender, RoutedEventArgs e)
        {
            viewController.ChangeDesktopBackground();
        }

        private void CheckIfNumber(object sender ,TextCompositionEventArgs e )
        {
            e.Handled = !viewController.IsTextAllowed(e.Text);

        }

        private void saveFilePathButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(filePathTextBox.Text))
            {
                GlobalConfig.FileSavePath = filePathTextBox.Text;
            }
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
    }
}
