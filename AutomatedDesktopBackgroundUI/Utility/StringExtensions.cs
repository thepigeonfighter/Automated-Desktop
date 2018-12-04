
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AutomatedDesktopBackgroundUI.Utility
{
    public static class StringExtensions
    {
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
                MessageBox.Show(input + " was not a valid string");
                return "Invalid Format";
            }
        }
    }
}
