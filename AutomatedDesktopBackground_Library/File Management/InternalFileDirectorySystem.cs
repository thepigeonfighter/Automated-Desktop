﻿using System;
using System.Diagnostics;
using System.IO;

namespace AutomatedDesktopBackgroundLibrary
{
    public static class InternalFileDirectorySystem
    {
        private const string interestInfoFileEnding = ".intInfo";

        private const string imageInfoFileEnding = ".imgInfo";

        private const string imageFileEnding = ".JPEG";

        private const string _imageInfoFolderName = "Image Info";

        private const string _interestInfoFolderName = "Interest Info";

        private const string _imagesFolder = "Images";

        private const string _settings = "Settings.txt";

        private const string _wallpaperCache = "wallpaperCache.temp";

        public readonly static string SettingsFile = _settings.FullFilePath();

        public readonly static string WallpaperCacheFile = _wallpaperCache.FullFilePath();

        public readonly static string ImageInfoFolder = _imageInfoFolderName.FullFilePath();

        public readonly static string InterestInfoFolder = _interestInfoFolderName.FullFilePath();

        public readonly static string ImagesFolder = _imagesFolder.FullFilePath();

        public readonly static string ChangeBackgroundOnceSource = Directory.GetCurrentDirectory() + @"\ChangeBackGroundOnce\ChangeBackgroundOnce.exe";

        public readonly static string ApplicationDirectory = FileSavePath;
        private static string GetCurrentVersion()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo info = FileVersionInfo.GetVersionInfo(assembly.Location);
            return info.FileVersion;
        }

        public static string GetFileEnding(this FileType file)
        {
            switch (file)
            {
                case FileType.ImageInfo:
                    return imageInfoFileEnding;

                case FileType.InterestInfo:
                    return interestInfoFileEnding;

                case FileType.ImageFile:
                    return imageFileEnding;

                default:
                    return "ERROR";
            }
        }

        private static string FileSavePath
        {
            get
            {
                string baseUrl = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string fullUrl = baseUrl + @"\AutomatedDesktop\Data";
                return Directory.CreateDirectory(fullUrl).FullName;
            }
            set { FileSavePath = value; }
        }

        public static string FullFilePath(this string fileName)
        {
            return $@"{FileSavePath}\{fileName}";
        }
    }
}