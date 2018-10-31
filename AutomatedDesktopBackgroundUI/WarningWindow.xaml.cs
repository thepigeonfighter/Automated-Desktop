using System.Windows;
using AutomatedDesktopBackgroundLibrary.ViewController;
namespace AutomatedDesktopBackgroundUI
{
    /// <summary>
    /// Interaction logic for WarningWindow.xaml
    /// </summary>
    public partial class WarningWindow : Window
    {
        private WarningWindowController viewController = new WarningWindowController();
        public WarningWindow()
        {
            InitializeComponent();
        }

        private void CloseWarningWindowButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateWarningCheckBox();
            this.Close();
        }
        private void UpdateWarningCheckBox()
        {
            bool newBool = warningCheckBox.IsChecked.HasValue ? warningCheckBox.IsChecked.Value : false;
            viewController.UpdateWarningTriggerValue(newBool);
        }
    }
}
