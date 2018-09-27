﻿using System.Net;

namespace AutomatedDesktopBackgroundLibrary
{
    public static class InterenetConnectionChecker
    {
        public static bool CheckConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://clients3.google.com/generate_204"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}