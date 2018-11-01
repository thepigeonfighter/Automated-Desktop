using AutomatedDesktopBackgroundLibrary;
using Squirrel;
using System;
using System.ComponentModel;
using System.Diagnostics;
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
            this.Closing += OnApplicationClosed;
            Window window = new MainWindow();
            Task.Run(()=>CheckForUpdates());
            Hide();
            window.Show();
        }

        private void OnApplicationClosed(object sender, CancelEventArgs e)
        {
            Task.Run(() => CheckForUpdates());
        }

        private async Task CheckForUpdates()
        {
            string path = @"https://github.com/thepigeonfighter/Automated-Desktop";

                using (var manager = UpdateManager.GitHubUpdateManager(path))
                {
                    await manager.Result.UpdateApp();
                     manager.Result.Dispose();
                }

        }

    }
}
