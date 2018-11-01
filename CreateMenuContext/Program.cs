using AutomatedDesktopBackgroundLibrary.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CreateMenuContext
{
    class Program
    {
        private static WindowsShellExtension shell = new WindowsShellExtension();
        static void Main(string[] args)
        {
                if(!WindowsShellExtension.IsElevated())
            {
                
                WindowsShellExtension.RunAsAdmin(GetAssembly());
                Environment.Exit(0);

            }
            try
            {
                CheckInput();
            }
            catch
            {
                Console.ReadLine();
            }
        }
        private static void AddRegistryEntry()
        {
            shell.CreateMenuOption(GetAssembly());
        }
        private static void RemoveRegistryEntry()
        {
            shell.RemoveMenuOption(GetAssembly());
        }
        private static string GetAssembly()
        {
            var path = Assembly.GetExecutingAssembly().Location;
            return path;
        }
        private static void CheckInput()
        {
            Console.WriteLine("\nPress A to add context menu shortcut or R to remove it ");
            char input = Console.ReadKey().KeyChar;
            if (input == 'a' || input == 'A')
            {
                AddRegistryEntry();
            }
            else if (input == 'r' || input == 'R')
            {
                RemoveRegistryEntry();
            }
            else
            {
                CheckInput();
            }
        }
    }
}
