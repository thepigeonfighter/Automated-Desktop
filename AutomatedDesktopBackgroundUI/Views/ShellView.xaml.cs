using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using AutomatedDesktopBackgroundUI.Utility;
using Caliburn.Micro;

namespace AutomatedDesktopBackgroundUI.Views
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView : Window
    {
        private NotifyIcon ADIcon;
        private IEventAggregator _eventAggregator;
        public ShellView(IEventAggregator eventAggregator)
        {
            InitializeComponent();
            BuildNotifyIcon();
            _eventAggregator = eventAggregator;
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
            ADIcon.DoubleClick += OnNotifyIconDoubleClick;
            
            ADIcon.MouseUp += ADI_MouseUp;

        }
        private void ADI_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var contextMenu = BuildContextMenu();
                ADIcon.ContextMenu = contextMenu;
                MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
                mi.Invoke(ADIcon, null);
            }
        }
        private System.Windows.Forms.ContextMenu BuildContextMenu()
        {
            System.Windows.Forms.ContextMenu contextMenu = new System.Windows.Forms.ContextMenu();
            System.Windows.Forms.MenuItem mainWindow = new System.Windows.Forms.MenuItem()
            {
                Index = 0,              
                Text = "Show Main Window"
            };
            mainWindow.Click += ShowMainWindowEvent;
            System.Windows.Forms.MenuItem skipImage = new System.Windows.Forms.MenuItem()
            {
                Index = 1,
                Text = "Skip Current Image"
            };
            skipImage.Click += SkipImageEvent;
            System.Windows.Forms.MenuItem likeImage = new System.Windows.Forms.MenuItem()
            {
                Index = 2,
                Text = "Like Current Image"
            };
            likeImage.Click += LikeImageEvent;
            System.Windows.Forms.MenuItem hateImage = new System.Windows.Forms.MenuItem()
            {
                Index = 0,
                Text = "Trash Current Image"
            };
            hateImage.Click += HateImageEvent;
            
            contextMenu.MenuItems.Add(mainWindow);
            contextMenu.MenuItems.Add(skipImage);
            contextMenu.MenuItems.Add(likeImage);
            contextMenu.MenuItems.Add(hateImage);
            
            return contextMenu;
        }
        private void ResetIcon()
        {
            ADIcon.ContextMenu.MenuItems.Clear();
        }

        private void HateImageEvent(object sender, EventArgs e)
        {
            _eventAggregator.PublishOnUIThread(new EventContainer() { Command = Config.CommandNames.HateImage });
            ResetIcon();
        }

        private void LikeImageEvent(object sender, EventArgs e)
        {
            _eventAggregator.PublishOnUIThread(new EventContainer() { Command = Config.CommandNames.LikeImage});
            ResetIcon();
        }

        private void SkipImageEvent(object sender, EventArgs e)
        {
            _eventAggregator.PublishOnUIThread(new EventContainer() { Command = Config.CommandNames.SkipWallpaper });
            ResetIcon();
        }

        private void ShowMainWindowEvent(object sender, EventArgs e)
        {
            this.Show();
            ADIcon.Visible = false;
        }

        private void OnNotifyIconDoubleClick(object sender, EventArgs e)
        {
            this.Show();
            ADIcon.Visible = false;
        }
    }
}
