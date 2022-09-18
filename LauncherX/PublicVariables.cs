using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LauncherX
{
    static class PublicVariables
    {
        //this variable is used to temporarily store the scale, and to transfer data between the SettingsWindow and the MainWindow
        public static double scale = 1.0;

        //this variable used to see if updates are required
        public static bool updaterequired;

        //this variable used to define file version
        public static string currentversion = "1.0.3";

        //this variable is used to pass the url
        public static string url;

        //this variable is used to store the full url (without removal of https:// or http://) for the use of textblock
        public static string original_url;

        //this variable is used to check if the button is clicked in the websitedialog
        public static bool websiteok = false;

        //this variable is used to store the update link
        public static string updateLink;

        //this variable is used to temporarily store the headerText, and to transfer data between the SettingsWindow and the MainWindow
        public static string headerText;

        //function to check and update theme
        public static void CheckAndUpdateTheme()
        {
            //next, check if the system is in light or dark theme
            bool is_light_mode = true;
            try
            {
                var v = Microsoft.Win32.Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", "1");
                if (v != null && v.ToString() == "0")
                    is_light_mode = false;
            }
            catch { }


            if (is_light_mode == true)
            {
                Wpf.Ui.Appearance.Theme.Apply(
                    Wpf.Ui.Appearance.ThemeType.Light,     // Theme type
                    Wpf.Ui.Appearance.BackgroundType.Mica, // Background type
                    true                                   // Whether to change accents automatically
                );
            }
            else if (is_light_mode == false)
            {
                Wpf.Ui.Appearance.Theme.Apply(
                   Wpf.Ui.Appearance.ThemeType.Dark,     // Theme type
                   Wpf.Ui.Appearance.BackgroundType.Mica, // Background type
                   true                                   // Whether to change accents automatically
               );
            }

        }


    }
}
