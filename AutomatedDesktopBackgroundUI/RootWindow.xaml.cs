using AutomatedDesktopBackgroundLibrary;
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

            Hide();
            window.Show();
        }

    }
}
