using AutomatedDesktopBackgroundLibrary.Utility;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace AutomatedDesktopBackgroundLibrary.StringExtensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Takes a string and formats it so that the first letter of every word is capitalized
        /// This is to make for nice folder names
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string MakePrettyString(this string input)
        {
            try
            {
                string lowercaseInput = input.ToLower();
                lowercaseInput = lowercaseInput.TrimEnd();
                string[] words = lowercaseInput.Split(' ');
                StringBuilder outputString = new StringBuilder();
                foreach (string word in words)
                {
                    string formattedWord = char.ToUpper(word[0]) + word.Substring(1) + " ";
                    outputString.Append(formattedWord);
                }
                string output = outputString.ToString();
                return output.TrimEnd();
            }
            catch
            {
                CustomMessageBox.Show(input);
                return "Invalid Format";
            }
        }

        public static InterestModel GetInterestByName(this string interestName)
        {
            InterestModel interest = DataKeeper.GetFileSnapShot().AllInterests.FirstOrDefault(x => x.Name == interestName);
            return interest;
        }

        public static DirectoryInfo CreateDirectory(this string dirName)
        {
            return Directory.CreateDirectory($"{FileSavePath}/{dirName}");
        }

        private static string FileSavePath
        {
            get
            {
                string baseUrl = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string fullUrl = baseUrl + @"\DesktopBackgrounds";
                return Directory.CreateDirectory(fullUrl).FullName;
            }
            set { FileSavePath = value; }
        }

        public static string FullFilePath(this string fileName)
        {
            return $@"{FileSavePath}\{fileName}";
        }

        public static string GetImageFileName(this string hreflink)
        {
            Uri uri = new Uri(hreflink);

            string filename = Path.GetFileName(uri.LocalPath) + ".JPEG";

            return filename;
        }

        public static string GetApplicationDirectory()
        {
            return FileSavePath;
        }
    }
}