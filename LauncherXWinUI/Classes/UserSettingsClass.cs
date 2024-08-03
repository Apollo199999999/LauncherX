using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace LauncherXWinUI.Classes
{
    public static class UserSettingsClass
    {
        /// <summary>
        /// Function to retrieve user settings from LauncherX versions <= 2.0.1 
        /// </summary>
        public static void UpgradeUserSettings()
        {
            // First, navigate to the directory where old user settings were stored
            string OldSettingsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ClickPhase");
            List<string> OldSettingsUserConfigFiles = new List<string>();

            foreach (string SettingsDir in Directory.GetDirectories(OldSettingsDir))
            {
                if (SettingsDir.Contains("LauncherX.exe")) 
                {
                    OldSettingsUserConfigFiles.AddRange(Directory.GetFiles(SettingsDir, "*.config", SearchOption.AllDirectories).ToList());
                }
            }

            // Iterate through all the user.config files and find the latest one, which we will take as the one to use in upgrading
            DateTime minTime = DateTime.MinValue;
            string newestOldSettingsFile = null;

            foreach (string SettingsFile in OldSettingsUserConfigFiles)
            {
                DateTime fileLastWriteTime = File.GetLastWriteTimeUtc(SettingsFile);
                if (fileLastWriteTime > minTime && Path.GetFileName(SettingsFile) == "user.config")
                {
                    minTime = fileLastWriteTime;
                    newestOldSettingsFile = SettingsFile;
                }
            }


        }
    }
}
