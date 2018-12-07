using AutomatedDesktopBackgroundUI.Properties;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
[assembly: log4net.Config.XmlConfigurator(ConfigFile = @"log4net.config", Watch = true)]
namespace AutomatedDesktopBackgroundUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(App));
        protected override void OnStartup(StartupEventArgs e)
        {
            log4net.Config.XmlConfigurator.Configure();
            log.Info("        =============  Started Logging  =============        ");
            if (e.Args != null)
            {
                if (e.Args.Any(x=> x== "Settings"))
                {
                    Settings.Default.ShowSettingsWindow = true;
                    Settings.Default.Save();
                }
            }
            base.OnStartup(e);
        }
        
    }
}
