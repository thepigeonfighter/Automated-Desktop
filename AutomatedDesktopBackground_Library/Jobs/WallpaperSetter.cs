using Microsoft.Win32;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace AutomatedDesktopBackgroundLibrary
{
    public static class WallpaperSetter
    {
        private const int SPI_SETDESKWALLPAPER = 20;
        private const int SPIF_UPDATEINIFILE = 0x01;
        private const int SPIF_SENDWININICHANGE = 0x02;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        public enum Style : int
        {
            Tiled,
            Centered,
            Stretched,
            Fit
        }

        public static void Set(string url, Style style)
        {
            System.Drawing.Image img = System.Drawing.Image.FromFile(url);
            var imageConverted = new System.Drawing.Bitmap(img);
            string tempPath = Path.Combine(Path.GetTempPath(), "wallpaper.bmp");
            try
            {
                imageConverted.Save(tempPath, System.Drawing.Imaging.ImageFormat.Bmp);
            }
            catch (ExternalException e)
            {
                Console.Write(e.ErrorCode);
            }
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            if (style == Style.Stretched)
            {
                key.SetValue("WallpaperStyle", 2.ToString());
                key.SetValue("TileWallpaper", 0.ToString());
            }

            if (style == Style.Centered)
            {
                key.SetValue("WallpaperStyle", 1.ToString());
                key.SetValue("TileWallpaper", 0.ToString());
            }

            if (style == Style.Tiled)
            {
                key.SetValue("WallpaperStyle", 1.ToString());
                key.SetValue("TileWallpaper", 1.ToString());
            }
            if (style == Style.Fit)
            {
                key.SetValue("WallpaperStyle", 3.ToString());
                key.SetValue("TileWallpaper", 0.ToString());
            }

            SystemParametersInfo(SPI_SETDESKWALLPAPER,
                0,
                tempPath,
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }
    }
}