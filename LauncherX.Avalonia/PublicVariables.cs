using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using FluentAvalonia.Styling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LauncherX.Avalonia
{
    public static class PublicVariables
    {
        //this variable used to define file version
        public static string PV_CurrentVersion = "2.0.0";

        //variable to store the current instance of MainWindow
        public static MainWindow? PV_MainWindow;

        //variable to store the current instance of SettingsWindow
        public static SettingsWindow? PV_SettingsWindow;

        //function to enable Mica on both windows
        public static void EnableMica()
        {
            if (PV_MainWindow != null && PV_SettingsWindow != null)
            {
                //enable mica by making the window background be null, configuring the visibility of the experimentalacrylicborder
                PV_MainWindow.Background = null;
                PV_SettingsWindow.Background = null;

                PV_MainWindow.MainWindowMicaMaterial.IsVisible = true;
                PV_SettingsWindow.SettingsWindowMicaMaterial.IsVisible = true;

            }
            
        }

        //function to change the application theme
        public static void ChangeApplicationTheme(string theme)
        {
            //get the currently running fluentavalonia theme manager
            var thmMgr = AvaloniaLocator.Current.GetService<FluentAvaloniaTheme>();

            //change the theme accordingly only if the theme manager, mainwindow, and settings window are all not null (they have been initialized)
            if (PV_MainWindow != null && PV_SettingsWindow != null && thmMgr != null)
            {
                //only enable Mica for both windows if LauncherX is running on Windows 11
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && Environment.OSVersion.Version.Build >= 22000)
                {
                    EnableMica();
                }

                if (theme.ToLower() == "light")
                {
                    //set light theme for both windows
                    thmMgr.RequestedTheme = "Light";

                    PV_MainWindow.MainWindowMicaMaterial.Material.TintColor = Colors.White;
                    PV_SettingsWindow.SettingsWindowMicaMaterial.Material.TintColor = Colors.White;

                    //check the lightthemeradiobutton in settings window
                    PV_SettingsWindow.LightThmRadioBtn.IsChecked = true;

                }
                else if (theme.ToLower() == "dark")
                {
                    //set light theme for both windows
                    thmMgr.RequestedTheme = "Dark";

                    PV_MainWindow.MainWindowMicaMaterial.Material.TintColor = Colors.Black;
                    PV_SettingsWindow.SettingsWindowMicaMaterial.Material.TintColor = Colors.Black;

                    //check the darkthemeradiobutton in settings window
                    PV_SettingsWindow.DarkThmRadioBtn.IsChecked = true;

                }

            }

        }


    }
}
