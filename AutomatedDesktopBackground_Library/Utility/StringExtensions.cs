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
                return "Invalid Format";
            }
        }



        public static string GetImageFileName(this string hreflink)
        {
            Uri uri = new Uri(hreflink);

            string filename = Path.GetFileName(uri.LocalPath) + ".JPEG";

            return filename;
        }

    }
}