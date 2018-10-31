using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutomatedDesktopBackgroundLibrary.Utility
{
    public partial class CustomMessageForm : Form
    {
        public CustomMessageForm(string message)
        {
           
            InitializeComponent();
            
            Color color = (Color)ColorConverter.ConvertFromString("#FFDBDBDB");
            var drawingColor = System.Drawing.Color.FromArgb(color.A, color.R, color.B, color.G);
            this.messageTextBox.BackColor = drawingColor;
            closeButton.BackColor = drawingColor;
            closeButton.Click += CloseButtonClickEvent;
            messageTextBox.Text = message;


        }



        private void CloseButtonClickEvent(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
