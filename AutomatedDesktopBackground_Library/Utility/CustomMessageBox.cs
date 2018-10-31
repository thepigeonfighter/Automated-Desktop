using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutomatedDesktopBackgroundLibrary.Utility
{
    public static class CustomMessageBox
    {
        public static void Show(string message)
        {
            Form form = new CustomMessageForm(message);
            form.Show();
        }
    }
}
