﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Diagnostics;
using System.Xml;

namespace LauncherXWinUI.Classes
{
    /// <summary>
    /// Class that handles all methods and variables relating to storing and retrieving user settings
    /// </summary>
    public static class UserSettingsClass
    {
        /// <summary>
        /// Class to store variables to read/write user settings to a json file
        /// </summary>
        public class UserSettingsJson
        {
            public string headerText { get; set; }
            public double gridScale { get; set; }
        }

        ///<summary>
        /// Variable which stores what is displayed on the text beside the "add" buttons in MainWindow.xaml
        ///</summary>
        public static string HeaderText = "My files, folders, and websites";

        ///<summary>
        /// Variable which stores how large the controls are rendered in the GridView in MainWindow.xaml
        ///</summary>
        public static double GridScale = 1.0f;

        /// <summary>
        /// Root directory where user data for LauncherX is stored
        /// </summary>
        public static string SettingsDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\LauncherX\\";

        /// <summary>
        /// Directory where the items added to LauncherX are stored
        /// </summary>
        public static string DataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\LauncherX\\Files";

        /// <summary>
        /// Temporary directory to store the icons of files added to LauncherX
        /// </summary>
        public static string FileIconDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\LauncherX\\Temp\\AppIcons\\";

        /// <summary>
        /// Temporary directory to store the icons of folders added to LauncherX
        /// </summary>
        public static string FolderIconDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\LauncherX\\Temp\\FolderIcons\\";

        /// <summary>
        /// Temporary directory to store the icons of websites added to LauncherX
        /// </summary>
        public static string WebsiteIconDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\LauncherX\\Temp\\WebsiteIcons\\";

        /// <summary>
        /// Method to create all of the directories that LauncherX will store user data in
        /// </summary>
        public static void CreateSettingsDirectories()
        {
            Directory.CreateDirectory(SettingsDir);
            Directory.CreateDirectory(DataDir);
            Directory.CreateDirectory(FileIconDir);
            Directory.CreateDirectory(FolderIconDir);
            Directory.CreateDirectory(WebsiteIconDir);
        }

        /// <summary>
        /// Method that clears the directories where icons are stored
        /// </summary>
        public static void ClearIconDirectories()
        {
            // Create a list of temp directories
            List<string> tempDirs = new List<string>() { FileIconDir, FolderIconDir, WebsiteIconDir };

            foreach (string dir in tempDirs)
            {
                System.IO.DirectoryInfo di = new DirectoryInfo(dir);

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }

            }
        }

        /// <summary>
        /// Method that checks if we need to retrieve settings from an older version of LauncherX less than 2.0.1
        /// </summary>
        /// <returns>A boolean indicating whether it is necessary to upgrade user settings</returns>
        public static bool UpgradeRequired()
        {
            // To check whether we need to upgrade settings, we need 2 conditions: The old settings file exists, and the new settings file (json) doesn't exist
            bool oldSettingsFileExists = false;
            bool newSettingsFileExists = false;

            // First, navigate to the directory where old user settings were stored
            string oldSettingsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ClickPhase");
            List<string> oldSettingsUserConfigFiles = new List<string>();

            foreach (string SettingsDir in Directory.GetDirectories(oldSettingsDir))
            {
                if (SettingsDir.Contains("LauncherX.exe"))
                {
                    // Old settings exist
                    oldSettingsFileExists = true;
                }
            }


            // Next, we check if the new settings file exists
            if (Directory.Exists(SettingsDir) && File.Exists(Path.Combine(SettingsDir, "userSettings.json")))
            {
                newSettingsFileExists = true;
            }

            return oldSettingsFileExists == true && newSettingsFileExists == false;
        }

        /// <summary>
        /// Method to retrieve user settings from LauncherX versions less than 2.0.1 
        /// </summary>
        public static void UpgradeUserSettings()
        {
            // First, navigate to the directory where old user settings were stored
            string oldSettingsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ClickPhase");
            List<string> oldSettingsUserConfigFiles = new List<string>();

            foreach (string SettingsDir in Directory.GetDirectories(oldSettingsDir))
            {
                if (SettingsDir.Contains("LauncherX.exe")) 
                {
                    oldSettingsUserConfigFiles.AddRange(Directory.GetFiles(SettingsDir, "*.config", SearchOption.AllDirectories).ToList());
                }
            }

            // Iterate through all the user.config files and find the latest one, which we will take as the one to use in upgrading
            DateTime minTime = DateTime.MinValue;
            string newestOldSettingsFile = null;

            foreach (string settingsFile in oldSettingsUserConfigFiles)
            {
                DateTime fileLastWriteTime = File.GetLastWriteTimeUtc(settingsFile);
                if (fileLastWriteTime > minTime && Path.GetFileName(settingsFile) == "user.config")
                {
                    minTime = fileLastWriteTime;
                    newestOldSettingsFile = settingsFile;
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
                HeaderText = oldHeaderText;
            }
            if (oldScale != null)
            {
                GridScale = Convert.ToDouble(oldScale);
            }
            
        }

        /// <summary>
        /// Method to write the settings to a json file to store user settings
        /// </summary>
        public static void WriteSettingsFile()
        {
            // Use the UserSettingsJson class to write a json file
            var userSettingsJson = new UserSettingsJson
            {
                headerText = HeaderText,
                gridScale = GridScale
            };

            string settingsFilePath = Path.Combine(SettingsDir, "userSettings.json");
            string jsonString = JsonSerializer.Serialize(userSettingsJson);
            File.WriteAllText(settingsFilePath, jsonString);
        }

        /// <summary>
        /// Method to try read user settings from a json file if it exists
        /// </summary>
        public static void TryReadSettingsFile()
        {
            string settingsFilePath = Path.Combine(SettingsDir, "userSettings.json");

            if (File.Exists(settingsFilePath))
            {
                string jsonString = File.ReadAllText(settingsFilePath);
                UserSettingsJson userSettingsJson = JsonSerializer.Deserialize<UserSettingsJson>(jsonString);

                // Assign variables
                HeaderText = userSettingsJson.headerText;
                GridScale = userSettingsJson.gridScale;
            }
        }
    }
}