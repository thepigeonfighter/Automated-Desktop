using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomatedDesktopBackgroundLibrary;
namespace ChangeBackgroundOnce
{
    class Program
    {
        static void Main(string[] args)
        {
            
            List<ImageModel> images = DataKeeper.GetFreshFileSnapShot().AllImages;
            if (images.Count > 0)
            {
                BackGroundPicker backGroundPicker = new BackGroundPicker();
                backGroundPicker.PickRandomBackground();
                
            }
        }
    }
}
