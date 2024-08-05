using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml;

namespace LauncherXWinUI.Classes
{
    public static class UserSettingsClass
    {
        ///<summary>
        /// Variable which stores what is displayed on the text beside the "add" buttons in MainWindow.xaml
        ///</summary>
        public static string headerText = "My files, folders, and websites";

        ///<summary>
        /// Variable which stores how large the controls are rendered in the GridView in MainWindow.xaml
        ///</summary>
        public static double gridScale = 1.0f;

        /// <summary>
        /// Function to retrieve user settings from LauncherX versions less than 2.0.1 
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

            // Now that we have the user.config file, open it using ConfigurationManager
            // Declare variables that we need to extract from the file
            string oldHeaderText = null;
            string oldScale = null;

            // Parse the user.config file as an xml file
            XmlDocument settingsXml = new XmlDocument();
            settingsXml.Load(newestOldSettingsFile);
            // Get all settings tags
            var settingsTags = settingsXml.GetElementsByTagName("setting");
            foreach (XmlNode settingsTag in settingsTags)
            {
                string oldSettingVariable = settingsTag.Attributes["name"].Value;
                
                if (oldSettingVariable == "scale")
                {
                    oldScale = settingsTag.FirstChild.InnerText;
                }
                else if (oldSettingVariable == "headerText")
                {
                    oldHeaderText = settingsTag.FirstChild.InnerText;
                }
            }

            // Assign the old settings variables just retrieved to the new variables in this class
            if (oldHeaderText != null)
            {
                headerText = oldHeaderText;
            }
            if (oldScale != null)
            {
                gridScale = Convert.ToDouble(oldScale);
            }
            
        }
    }
}
