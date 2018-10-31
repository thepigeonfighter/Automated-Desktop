using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutomatedDesktopBackgroundLibrary.Utility
{
    public partial class CustomTextBox : TextBox
    {
        public CustomTextBox()
        {
            InitializeComponent();
            SetStyle(ControlStyles.SupportsTransparentBackColor |
              ControlStyles.OptimizedDoubleBuffer |
              ControlStyles.AllPaintingInWmPaint |
              ControlStyles.ResizeRedraw |
              ControlStyles.UserPaint, true);
            Color color =  (Color)ColorConverter.ConvertFromString("#FFDBDBDB");
            var drawingColor = System.Drawing.Color.FromArgb(color.A, color.R, color.B, color.G);
            this.BackColor = drawingColor;
        }

    }
    
}
