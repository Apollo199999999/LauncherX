using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using RestoreWindowPlace;

namespace LauncherX
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //save window pos
        public string loadDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\LauncherX\\";
        public WindowPlace WindowPlace { get; }

        public App()
        {
            // Set a name of config file
            this.WindowPlace = new WindowPlace(Path.Combine(loadDir, "placement.config"));
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            this.WindowPlace.Save();
        }
    }
}
