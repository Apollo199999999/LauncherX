using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Diagnostics;
using System.Xml;
using System.Text.Json.Serialization;
using LauncherXWinUI.Controls;
using Windows.Storage;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media.Imaging;
using static System.Net.Mime.MediaTypeNames;

namespace LauncherXWinUI.Classes
{
    /// <summary>
    /// Class to store variables to read/write user settings to a json file
    /// </summary>
    public class UserSettingsJson
    {
        public string headerText { get; set; }
        public double gridScale { get; set; }
    }

    /// <summary>
    /// To be used for json source generation
    /// </summary>
    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(UserSettingsJson))]
    internal partial class SourceGenerationContext : JsonSerializerContext
    {
    }

    /// <summary>
    /// Class that handles all methods and variables relating to storing and retrieving user settings
    /// </summary>
    public static class UserSettingsClass
    {
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
        /// Variable which stores items that could not be added to LauncherX as their file paths no longer exist
        /// </summary>
        public static List<string> ErrorPaths = new List<string>();

        // Helper methods
        /// <summary>
        /// Determines if a given path belongs to that of a file or folder
        /// </summary>
        /// <param name="path">Path of file/folder</param>
        /// <returns>Returns true if path is a folder, false otherwise</returns>
        /// <exception cref="ArgumentNullException">Thrown when no path argument is input.</exception>
        private static bool IsPathDirectory(string path)
        {
            if (path == null) throw new ArgumentNullException("path");
            path = path.Trim();

            if (Directory.Exists(path))
                return true;

            if (File.Exists(path))
                return false;

            // neither file nor directory exists. guess intention

            // if has trailing slash then it's a directory
            if (new[] { "\\", "/" }.Any(x => path.EndsWith(x)))
                return true; // ends with slash

            // if has extension then its a file; directory otherwise
            return string.IsNullOrWhiteSpace(Path.GetExtension(path));
        }

        // Public methods
        /// <summary>
        /// Method to create all of the directories that LauncherX will store user data in
        /// </summary>
        public static void CreateSettingsDirectories()
        {
            Directory.CreateDirectory(SettingsDir);
            Directory.CreateDirectory(DataDir);
        }

        /// <summary>
        /// Method that deletes the directories where file/folder/website icons are stored, that were used for older versions of LauncherX
        /// </summary>
        public static void ClearOldTempDirectories()
        {
            string oldTempDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\LauncherX\\Temp\\";
            if (Directory.Exists(oldTempDir))
            {
                Directory.Delete(oldTempDir, true);
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

            if (!Directory.Exists(oldSettingsDir))
            {
                return false;
            }

            List<string> oldSettingsUserConfigFiles = new List<string>();

            foreach (string settingsDir in Directory.GetDirectories(oldSettingsDir))
            {
                if (settingsDir.Contains("LauncherX.exe"))
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

            foreach (string settingsDir in Directory.GetDirectories(oldSettingsDir))
            {
                if (settingsDir.Contains("LauncherX.exe"))
                {
                    oldSettingsUserConfigFiles.AddRange(Directory.GetFiles(settingsDir, "*.config", SearchOption.AllDirectories).ToList());
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
            string jsonString = JsonSerializer.Serialize(userSettingsJson!, SourceGenerationContext.Default.UserSettingsJson);
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
                UserSettingsJson userSettingsJson = JsonSerializer.Deserialize<UserSettingsJson>(jsonString, SourceGenerationContext.Default.UserSettingsJson);

                // Assign variables
                HeaderText = userSettingsJson.headerText;
                GridScale = userSettingsJson.gridScale;
            }
        }

        /// <summary>
        /// Method to add items from versions of LauncherX less than 2.1.0 to versions of LauncherX greater than 2.1.0
        /// Dictionary<string, object> is in the format of {"ExecutingPath": string, "ExecutingArguments": string, "DisplayText": string, "ImageSource": BitmapImage or SoftwareBitmapSource}
        /// </summary>
        /// <returns>List of dictionaries that stores all the necessary properties to create the GridViewTiles in MainWindow.xaml.cs</returns>
        public async static Task<List<Dictionary<string, object>>> UpgradeOldLauncherXItems()
        {
            // List of Dictionaries that will be returned
            List<Dictionary<string, object>> gridViewTilesProps = new List<Dictionary<string, object>>();

            if (Directory.GetFiles(DataDir).Length == 0)
            {
                return gridViewTilesProps;
            }

            // Create a new list and sort the files
            var list = Directory.GetFiles(DataDir);
            Array.Sort(list, new AlphanumComparatorFast());

            // Loop through the files, and create GridViewTiles as we go
            foreach (string file in list)
            {
                // Open the file to read from.
                StreamReader sr = File.OpenText(file);
                string executingPath = "";

                // Read the first line of the text document, which will give us the ExecutingPath for a GridViewTile
                executingPath = sr.ReadLine();

                // Create a new dictionary to store props
                Dictionary<string, object> tileProps = new Dictionary<string, object>();
                tileProps.Add("ExecutingPath", executingPath);
                tileProps.Add("ExecutingArguments", "");

                // Next, depending on if the executingPath is a file, folder, or website, we need to add props differently
                if (executingPath.StartsWith("https://") || executingPath.StartsWith("http://"))
                {
                    // Website
                    tileProps.Add("DisplayText", executingPath);
                    tileProps.Add("ImageSource", IconHelpers.GetWebsiteIcon(executingPath));
                    gridViewTilesProps.Add(tileProps);
                }
                else if (IsPathDirectory(executingPath) && Path.Exists(executingPath))
                {
                    // Folder
                    StorageFolder storageFolder = await StorageFolder.GetFolderFromPathAsync(executingPath);
                    BitmapImage folderIcon = await IconHelpers.GetFolderIcon(storageFolder);
                    tileProps.Add("DisplayText", storageFolder.Name);
                    tileProps.Add("ImageSource", folderIcon);
                    gridViewTilesProps.Add(tileProps);
                }
                else if (!IsPathDirectory(executingPath) && Path.Exists(executingPath))
                {
                    // File
                    tileProps.Add("DisplayText", Path.GetFileName(executingPath));
                    string ext = Path.GetExtension(executingPath);
                    if (ext == ".lnk" || ext == ".url" || ext == ".wsh")
                    {
                        // Use Win32 methods
                        SoftwareBitmapSource src =  await IconHelpers.GetFileIconWin32(executingPath);
                        tileProps.Add("ImageSource", src);
                    }
                    else
                    {
                        try
                        {
                            // Use WinRT methods
                            StorageFile storageFile = await StorageFile.GetFileFromPathAsync(executingPath);
                            BitmapImage fileIcon = await IconHelpers.GetFileIcon(storageFile);
                            tileProps.Add("ImageSource", fileIcon);
                        }
                        catch
                        {
                            // Use Win32 methods
                            SoftwareBitmapSource src = await IconHelpers.GetFileIconWin32(executingPath);
                            tileProps.Add("ImageSource", src);
                        }
                    }
                    gridViewTilesProps.Add(tileProps);
                }
                else if (!Path.Exists(executingPath))
                {
                    // Not a website, and path doesn't exist
                    ErrorPaths.Add(executingPath);
                }
            }

            return gridViewTilesProps;
        }

        /// <summary>
        /// Method that checks whether there were any errors adding items to LauncherX
        /// </summary>
        /// <returns></returns>
        public static bool ErrorAddingItems()
        {
            return ErrorPaths.Count > 0;
        }
    }
}
