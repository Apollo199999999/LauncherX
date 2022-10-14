using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.Caching;
using static System.Net.WebRequestMethods;

namespace LauncherX
{
    static class PublicVariables
    {
        //this variable is used to temporarily store the scale, and to transfer data between the SettingsWindow and the MainWindow
        public static double scale = 1.0;

        //this variable used to define file version
        public static string currentversion = "2.0.0";

        //this variable is used to pass the url
        public static string url;

        //this variable is used to store the full url (without removal of https:// or http://) for the use of textblock
        public static string original_url;

        //this variable is used to check if the button is clicked in the websitedialog
        public static bool websiteok = false;

        //this variable is used to temporarily store the headerText, and to transfer data between the SettingsWindow and the MainWindow
        public static string headerText;

        #region Methods relating to auto-update system

        //Check Internet Connection Function
        //Creating the extern function...  
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int Description, int ReservedValue);
        public static bool CheckForInternetConnection()
        {
            int Desc;
            return InternetGetConnectedState(out Desc, 0);
        }

        //Check for update function
        public static bool? CheckForUpdates()
        {
            //Check for updates

            if (CheckForInternetConnection() == true)
            {
                //update link is a pastebin link
                var url = "https://pastebin.com/raw/PGcXcxnT";

                //init a webclient
                WebClient client = new WebClient();

                //download all text from the pastebin raw link
                string reply = client.DownloadString(url);

                //check if version matches pastebin link
                if (reply.ToString() != currentversion.ToString())
                {
                    //this is the output when an update is available. Modify it if you wish
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return null;
            }
        }

        #endregion

    }
}
