using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using FluentAvalonia.Styling;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.Interop;

namespace LauncherX.Avalonia
{
    public static class PublicVariables
    {
        //directories to store files
        public static string PV_WebsiteIconDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "LauncherX", "Temp", "WebsiteIcons");

        //this variable used to define file version
        public static string PV_CurrentVersion = "2.0.0";

        //variable to store the current instance of MainWindow
        public static MainWindow? PV_MainWindow;

        //variable to store the current instance of SettingsWindow
        public static SettingsWindow? PV_SettingsWindow;

        //bool if user is on Windows 11
        public static bool PV_IsOnWindows11()
        {
            Win32Interop.OSVERSIONINFOEX version = new Win32Interop.OSVERSIONINFOEX
            {
                OSVersionInfoSize = Marshal.SizeOf<Win32Interop.OSVERSIONINFOEX>()
            };

            Win32Interop.RtlGetVersion(ref version);

            return version.BuildNumber >= 22000; 
        }

        //function to change the application theme
        public static void PV_ChangeApplicationTheme(string theme)
        {
            //get the currently running fluentavalonia theme manager
            var thmMgr = AvaloniaLocator.Current.GetService<FluentAvaloniaTheme>();

            //change the theme accordingly only if the theme manager, mainwindow, and settings window are all not null (they have been initialized)
            if (PV_MainWindow != null && PV_SettingsWindow != null && thmMgr != null)
            {
                if (theme.ToLower() == "light")
                {
                    //set light theme for both windows
                    thmMgr.RequestedTheme = FluentAvaloniaTheme.LightModeString;
                    //check the lightthemeradiobutton in settings window
                    PV_SettingsWindow.LightThmRadioBtn.IsChecked = true;

                }
                else if (theme.ToLower() == "dark")
                {
                    //set light theme for both windows
                    thmMgr.RequestedTheme = FluentAvaloniaTheme.DarkModeString;

                    //check the darkthemeradiobutton in settings window
                    PV_SettingsWindow.DarkThmRadioBtn.IsChecked = true;

                }

            }

        }

        //Check Internet Connection Function
        public static bool PV_CheckForInternetConnection(int timeoutMs = 10000, string? url = null)
        {
            try
            {
                url ??= CultureInfo.InstalledUICulture switch
                {
                    { Name: var n } when n.StartsWith("fa") => // Iran
                        "http://www.aparat.com",
                    { Name: var n } when n.StartsWith("zh") => // China
                        "http://www.baidu.com",
                    _ =>
                        "http://www.gstatic.com/generate_204",
                };

                var request = (HttpWebRequest)WebRequest.Create(url);
                request.KeepAlive = false;
                request.Timeout = timeoutMs;
                using (var response = (HttpWebResponse)request.GetResponse())
                    return true;
            }
            catch
            {
                return false;
            }

        }

        //function to check for updates
        public static bool PV_CheckForUpdates()
        {
            //Check for updates

            //updtae link is a pastebin link
            var url = "https://pastebin.com/raw/PGcXcxnT";

            //init a webclient
            WebClient client = new WebClient();

            //download all text from the pastebin raw link
            string reply = client.DownloadString(url).ToString();

            //check if version matches pastebin link
            if (reply != PV_CurrentVersion)
            {
                //this is the output when an update is available. Modify it if you wish
                return true;
            }
            else
            {
                return false;
            }

        }


        //function to start url
        public async static void PV_OpenBrowser(string url)
        {
            // NOTE: Will not open avares files or anything embedded in the assembly
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true, Verb = "open" });

        }
    }
}
