using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LauncherXWinUI.Classes
{
    /// <summary>
    /// A class that allows you to monitor multiple directories using FileSystemWatcher
    /// </summary>
    public class MultiFileSystemWatcher
    {
        /// <summary>
        /// Stores all of the paths currently being watched
        /// </summary>
        public ObservableCollection<string> WatchedPaths = new ObservableCollection<string>();

        /// <summary>
        /// Event that is raised when the contents of any of the watched directory changes
        /// </summary>
        public event EventHandler WatchedChanged = delegate { };

        /// <summary>
        /// Stores all the FileSystemWatchers created to watch each directory
        /// </summary>
        private List<FileSystemWatcher> FileSystemWatchers = new List<FileSystemWatcher>();

        public MultiFileSystemWatcher() 
        {
            // Monitor when the WatchedPaths changes
            WatchedPaths.CollectionChanged += WatchedPaths_CollectionChanged;
        }

        public void Dispose()
        {
            // Dispose of all existing FileSystemWatchers
            foreach (FileSystemWatcher watcher in FileSystemWatchers)
            {
                watcher.Dispose();
            }
        }

        private void WatchedPaths_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Dispose of all existing FileSystemWatchers
            foreach (FileSystemWatcher watcher in FileSystemWatchers)
            {
                watcher.Dispose();
            }

            // Re-initialise FileSystemWatchers for all of the WatchedPaths
            foreach (string watchedPath in WatchedPaths)
            {
                FileSystemWatcher fileSystemWatcher = new FileSystemWatcher(watchedPath);
                fileSystemWatcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size;
                fileSystemWatcher.EnableRaisingEvents = true;
                fileSystemWatcher.Changed += FileSystemWatcher_Changed;
                fileSystemWatcher.Created += FileSystemWatcher_Changed;
                fileSystemWatcher.Deleted += FileSystemWatcher_Changed;
                fileSystemWatcher.Renamed += FileSystemWatcher_Changed;
                FileSystemWatchers.Add(fileSystemWatcher);
            }
        }

        private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            // Raise an event
            WatchedChanged(sender, e);
        }
    }
}
