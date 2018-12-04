using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AutomatedDesktopBackgroundUI.Views
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView : Window
    {
        private NotifyIcon ADIcon;
        public ShellView()
        {
            InitializeComponent();
            BuildNotifyIcon();
            this.MouseLeftButtonDown += DragWindow;
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
                base.OnMouseLeftButtonDown(e);
                this.DragMove();           
        }

        private void MinimizeWindow_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {

            this.Hide();
            ADIcon.Visible = true;

        }
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

        private void OnNotifyIconDoubleClik(object sender, EventArgs e)
        {
            this.Show();
            ADIcon.Visible = false;
        }
    }
}
