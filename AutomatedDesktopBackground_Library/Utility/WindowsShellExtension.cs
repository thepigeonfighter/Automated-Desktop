﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace AutomatedDesktopBackgroundLibrary.Utility
{

    public class WindowsShellExtension : IShellExtension
    {
        private const string Menu = @"Directory\Background\shell\Change Desktop Image";
        private const string Command = @"Directory\Background\shell\Change Desktop Image\command";
        public void CreateMenuOption(string executingAssembly)
        {
            if (!IsElevated())
            {
                RunAsAdmin(executingAssembly);
            }
            else
            {
                RegistryKey regmenu = null;
                RegistryKey regcmd = null;
                try
                {
                    string path = InternalFileDirectorySystem.ChangeBackgroundOnceSource;
                    regmenu = Registry.ClassesRoot.CreateSubKey(Menu);
                    regmenu.SetValue("Icon", path);
                    regmenu = Registry.ClassesRoot.CreateSubKey(Command);  
                    regmenu.SetValue("", path);
                    MessageBox.Show("Sucessfully added");

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    if (regmenu != null)
                        regmenu.Close();
                    if (regcmd != null)
                        regcmd.Close();
                }
            }
        }
        public bool ContextMenuEnabled()
        {
            var reg = Registry.ClassesRoot.OpenSubKey(Menu);
            return reg != null;
        }
        public void RemoveMenuOption(string executingAssembly)
        {
            if (!IsElevated())
            {
                RunAsAdmin(executingAssembly);
            }
            else
            {
                try
                {

                    var reg = Registry.ClassesRoot.OpenSubKey(Menu);
                    if (reg != null)
                    {
                        reg.Close();
                        Registry.ClassesRoot.DeleteSubKeyTree(Menu);
                    }
                    MessageBox.Show("Sucessfully Removed");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }
        public void RunAsAdmin(string executingAssembly)
        {
           
            
             using (var process = Process.Start(new ProcessStartInfo(executingAssembly, "/run_elevated_action")
            {
                 Arguments = "Settings",
                Verb = "runas"
               
            }))
            {
                process?.WaitForExit();
            }
            

        }


        public bool IsElevated()
        {
            using (var identity = WindowsIdentity.GetCurrent())
            {
                var principal = new WindowsPrincipal(identity);

                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }
    }
}
