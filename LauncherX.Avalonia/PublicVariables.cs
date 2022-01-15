using Avalonia;
using Avalonia.Media;
using FluentAvalonia.Styling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LauncherX.Avalonia
{
    public static class PublicVariables
    {
        //this variable used to define file version
        public static string PV_CurrentVersion = "2.0.0";

        //string to store the system theme
        public static string PV_SysTheme = "";

        //variable to store the current instance of MainWindow
        public static MainWindow? PV_mainWindow;

        //variable to store the current instance of SettingsWindow
        public static SettingsWindow? PV_settingsWindow;

        //function to change the application theme
        public static void ChangeApplicationTheme(string theme)
        {
            //get the currently running fluentavalonia theme manager
            var thmMgr = AvaloniaLocator.Current.GetService<FluentAvaloniaTheme>();

            //change the theme accordingly only if the theme manager, mainwindow, and settings window are all not null (they have been initialized)
            if (PV_mainWindow != null && PV_settingsWindow != null && thmMgr != null)
            {
                if (theme.ToLower() == "light")
                {
                    //set light theme for both windows
                    thmMgr.RequestedTheme = "Light";

                    PV_mainWindow.MainWindowMicaMaterial.Material.TintColor = Colors.White;
                    PV_settingsWindow.SettingsWindowMicaMaterial.Material.TintColor = Colors.White;
                }
                else if (theme.ToLower() == "dark")
                {
                    //set light theme for both windows
                    thmMgr.RequestedTheme = "Dark";

                    PV_mainWindow.MainWindowMicaMaterial.Material.TintColor = Colors.Black;
                    PV_settingsWindow.SettingsWindowMicaMaterial.Material.TintColor = Colors.Black;
                }
                else if (theme.ToLower() == "system")
                {
                    //set light theme for both windows
                    thmMgr.RequestedTheme = PV_SysTheme;

                    /*if the systheme is dark, set the mica material for both windows to black. Otherwise, if the systheme is light,
                    set the mica material for both windows to white*/

                    if (PV_SysTheme.ToLower() == "light")
                    {
                        PV_mainWindow.MainWindowMicaMaterial.Material.TintColor = Colors.White;
                        PV_settingsWindow.SettingsWindowMicaMaterial.Material.TintColor = Colors.White;
                    }
                    else if (PV_SysTheme.ToLower() == "dark")
                    {
                        PV_mainWindow.MainWindowMicaMaterial.Material.TintColor = Colors.Black;
                        PV_settingsWindow.SettingsWindowMicaMaterial.Material.TintColor = Colors.Black;
                    }
                }

            }

        }


    }
}
