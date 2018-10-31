using AutomatedDesktopBackgroundLibrary;
using Squirrel;
using System.Threading.Tasks;
using System.Windows;


namespace AutomatedDesktopBackgroundUI
{
    /// <summary>
    /// Interaction logic for RootWindow.xaml
    /// </summary>
    public partial class RootWindow : Window
    {
        public RootWindow()
        {
            InitializeComponent();
            WindowManager.RegisterWindow(this);
            Window window = new MainWindow();
            Task.Run(()=>CheckForUpdates());
            Hide();
            window.Show();
        }
        private async Task CheckForUpdates()
        {
            //TODO hook this up to an online repository
            string path = @"https://github.com/thepigeonfighter/Automated-Desktop";
            using (var manager = UpdateManager.GitHubUpdateManager(path))
            {
                await manager.Result.UpdateApp();
            }
        }

    }
}
