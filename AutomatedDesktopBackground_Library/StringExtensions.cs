using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedDesktopBackgroundLibrary
{
    public static class StringExtensions
    {
        /// <summary>
        /// Takes a string and formats it so that the first letter of every word is capitalized
        /// This is to make for nice folder names 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string MakePrettyString(string input)
        {
            try
            {
                string lowercaseInput = input.ToLower();
                string[] words = lowercaseInput.Split(' ');
                StringBuilder outputString = new StringBuilder();
                foreach (string word in words)
                {
                    string formattedWord = char.ToUpper(word[0]) + word.Substring(1) + " ";
                    outputString.Append(formattedWord);
                }


                return outputString.ToString();
            }
            catch
            {
                return "Invalid Format";
            }
        }
    }
}
