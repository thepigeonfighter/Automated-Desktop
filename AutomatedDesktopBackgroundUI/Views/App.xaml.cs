using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config", Watch = true)]
namespace AutomatedDesktopBackgroundUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //TODO uncomment this section to start logging
        /*
        private static readonly ILog log = LogManager.GetLogger(typeof(App));
        protected override void OnStartup(StartupEventArgs e)
        {
            log4net.Config.XmlConfigurator.Configure();
            log.Info("        =============  Started Logging  =============        ");
            base.OnStartup(e);
        }
        */
    }
}
