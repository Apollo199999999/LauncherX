using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LauncherXWinUI.Classes
{
    /// <summary>
    /// Class that handles checking for updates
    /// </summary>
    public static class UpdatesClass
    {
        //Check Internet Connection Function
        //Creating the extern function...  
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int Description, int ReservedValue);

        /// <summary>
        /// Use Win32 API to check whether an internet connection exists
        /// </summary>
        /// <returns></returns>
        private static bool CheckForInternetConnection()
        {
            int Desc;
            return InternetGetConnectedState(out Desc, 0);
        }

        /// <summary>
        /// Asynchronous method that checks whether there's a new version of LauncherX available by checking GitHub and updates an internal variable
        /// </summary>
        /// <returns>Boolean stating whether there is an update or not, or null if it failed to check for updates</returns>
        public async static Task<bool?> IsUpdateAvailable()
        {
            if (CheckForInternetConnection() != true)
            {
                // No internet connection, do not proceed
                return null;
            }

            // Compare the latest release tag with the app version. If they are different, update is available
            GitHubClient client = new GitHubClient(new ProductHeaderValue("LauncherX"));
            IReadOnlyList<Release> releases = await client.Repository.Release.GetAll("Apollo199999999", "LauncherX");

            if (releases.Count <= 0)
            {
                // No releases to compare to, installed version is the latest one
                return false;
            }

            // NOTE: This assumes that the latest release tag is in the format of "vX.X.X", e.g. "v1.0.2"
            // This also assumes that we take the current version in the format of X.X.X, e.g. "2.1.0"

            // Get the latest release by retrieving its tag
            string latestReleaseTag = releases[0].TagName;

            // If the tag starts with "v", remove it so we can compare versions
            if (latestReleaseTag.StartsWith("v"))
            {
                latestReleaseTag = latestReleaseTag.Substring(1, latestReleaseTag.Length - 1);
            }

            // Get the installed version of LauncherX from the assembly version, and remove the ".0" at the end
            string installedVersionString = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            installedVersionString = installedVersionString.Substring(0, installedVersionString.Length - 2);

            // Compare the versions
            Version latestGitHubRelease = new Version(latestReleaseTag);
            Version localInstalledVersion = new Version(installedVersionString);
            int versionComparison = localInstalledVersion.CompareTo(latestGitHubRelease);
            
            if (versionComparison < 0)
            {
                // Updates are available
                return true;

            }
            else
            {
                // Local installed version is newer or as late as the release on GitHub. No updates available
                return false;
            }
        }
    }
}
