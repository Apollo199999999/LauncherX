using LauncherXWinUI.Classes;
using LauncherXWinUI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.IO;
using System.Diagnostics;
using Windows.ApplicationModel.DataTransfer;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LauncherXWinUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : WinUIEx.WindowEx
    {
        public MainWindow()
        {
            this.InitializeComponent();

            // Create a new event handler for when the items in the ItemsGridView have changed (either new items added/removed or items are reset)
            ItemsGridView.Items.VectorChanged += ItemsGridViewItems_VectorChanged;

            // Create settings directories
            UserSettingsClass.CreateSettingsDirectories();

            // Upgrade settings and write new settings file if necessary
            if (UserSettingsClass.UpgradeRequired())
            {
                UserSettingsClass.UpgradeUserSettings();
                UserSettingsClass.WriteSettingsFile();
                UserSettingsClass.ClearOldTempDirectories();
            }

            // Retrieve user settings from file
            UserSettingsClass.TryReadSettingsFile();

            // Once we have initialised the UserSettingsClass with the correct values, update the UI
            UpdateUIFromSettings();
        }

        // Helper methods
        private void UpdateUIFromSettings()
        {
            // Set header text
            HeaderTextBlock.Text = UserSettingsClass.HeaderText;

            // Adjust the size of items in ItemsGridView
            foreach (var gridViewItem in ItemsGridView.Items) {
                if (gridViewItem is GridViewTile)
                {
                    ((GridViewTile)gridViewItem).Size = UserSettingsClass.GridScale;
                }
            }
        }

        // Event Handlers
        private void GetUpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to GitHub releases page and exit application
            Process.Start(new ProcessStartInfo { FileName = "https://github.com/Apollo199999999/LauncherX/releases", UseShellExecute = true });
            Application.Current.Exit();
        }

        private void ItemsGridViewItems_VectorChanged(Windows.Foundation.Collections.IObservableVector<object> sender, Windows.Foundation.Collections.IVectorChangedEventArgs @event)
        {
            // Show/Hide the EmptyNotice depending on whether there are items in the ItemsGridView
            if (ItemsGridView.Items.Count > 0)
            {
                EmptyNotice.Visibility = Visibility.Collapsed;
            }
            else
            {
                EmptyNotice.Visibility = Visibility.Visible;
            }
        }

        private async void Container_Loaded(object sender, RoutedEventArgs e)
        {
            // Set placeholder titlebar for now, before WASDK 1.6
            this.ExtendsContentIntoTitleBar = true;
            this.SetTitleBar(AppTitleBar);

            // Set Window icon
            UIFunctionsClass.SetWindowLauncherXIcon(this);

            // Set Window Background
            UIFunctionsClass.SetWindowBackground(this, ContainerFallbackBackgroundBrush);

            // Check for updates and display the InfoBar if necessary
            bool? isUpdateAvailable = await UpdatesClass.IsUpdateAvailable();
            if (isUpdateAvailable == true)
            {
                UpdateInfoBar.IsOpen = true;
            }
        }

        private async void AddFileBtn_Click(object sender, RoutedEventArgs e)
        {
            AddFileDialog addFileDialog = new AddFileDialog()
            {
                XamlRoot = Container.XamlRoot
            };

            ContentDialogResult result = await addFileDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                // Add the files from the addFileDialog
                foreach (AddFileDialogListViewItem fileItem in addFileDialog.AddedFiles)
                {
                    // Create new GridViewTile for each item
                    GridViewTile gridViewTile = new GridViewTile();
                    gridViewTile.ExecutingPath = fileItem.ExecutingPath;
                    gridViewTile.ExecutingArguments = fileItem.ExecutingArguments;
                    gridViewTile.DisplayText = fileItem.DisplayText;
                    gridViewTile.ImageSource = fileItem.FileIcon;
                    gridViewTile.Size = UserSettingsClass.GridScale;
                    gridViewTile.Drop += GridViewTtem_Drop;
                    ItemsGridView.Items.Add(gridViewTile);
                }
            }
        }

        private async void AddFolderBtn_Click(object sender, RoutedEventArgs e)
        {
            AddFolderDialog addFolderDialog = new AddFolderDialog()
            {
                XamlRoot = Container.XamlRoot
            };

            ContentDialogResult result = await addFolderDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                // Add the files from the addFolderDialog
                foreach (AddFolderDialogListViewItem folderItem in addFolderDialog.AddedFolders)
                {
                    // Create new GridViewTile for each item
                    GridViewTile gridViewTile = new GridViewTile();
                    gridViewTile.ExecutingPath = folderItem.ExecutingPath;
                    gridViewTile.ExecutingArguments = "";
                    gridViewTile.DisplayText = folderItem.DisplayText;
                    gridViewTile.ImageSource = folderItem.FolderIcon;
                    gridViewTile.Size = UserSettingsClass.GridScale;
                    gridViewTile.Drop += GridViewTtem_Drop;
                    ItemsGridView.Items.Add(gridViewTile);
                }
            }
        }

        private async void AddWebsiteBtn_Click(object sender, RoutedEventArgs e)
        {
            AddWebsiteDialog addWebsiteDialog = new AddWebsiteDialog()
            {
                XamlRoot = Container.XamlRoot
            };

            ContentDialogResult result = await addWebsiteDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                // Create new GridViewTile to display the website
                GridViewTile gridViewTile = new GridViewTile();
                gridViewTile.ExecutingPath = addWebsiteDialog.InputWebsiteUrl;
                gridViewTile.ExecutingArguments = "";
                gridViewTile.DisplayText = addWebsiteDialog.InputWebsiteUrl;
                gridViewTile.Size = UserSettingsClass.GridScale;
                gridViewTile.Drop += GridViewTtem_Drop;

                // Get the icon of the website, using Google's favicon service
                BitmapImage websiteIcon = new BitmapImage();
                // Fallback icon
                Uri defaultImageUri = new Uri(Path.GetFullPath(@"Resources\websitePlaceholder.png"), UriKind.Absolute);
                websiteIcon.ImageFailed += (s, e) => 
                {
                    websiteIcon.UriSource = defaultImageUri;
                    gridViewTile.ImageSource = websiteIcon;
                };
                // Try getting website icon
                Uri iconUri = new Uri("https://www.google.com/s2/favicons?sz=128&domain_url=" + addWebsiteDialog.InputWebsiteUrl, UriKind.Absolute);
                websiteIcon.UriSource = iconUri;
                gridViewTile.ImageSource = websiteIcon;

                ItemsGridView.Items.Add(gridViewTile);
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            // Open Settings Window as a modal window
            SettingsWindow settingsWindow = new SettingsWindow();
            UIFunctionsClass.CreateModalWindow(settingsWindow, this);

            // Update the UI once the SettingsWindow is closed
            settingsWindow.Closed += (s, e) => UpdateUIFromSettings();
        }

        // This section of event handlers handles dragging items in the GridView to make groups
        private void ItemsGridView_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            e.Data.Properties.Add("DraggedControl", (e.Items[0] as GridViewTile));
            Debug.WriteLine("drag: " + (e.Items[0] as GridViewTile).UniqueId);
        }

        private void GridViewTtem_Drop(object sender, DragEventArgs e)
        {
            GridViewTile DroppedOnTile = sender as GridViewTile;
            GridViewTile DraggedTile = e.Data.Properties["DraggedControl"] as GridViewTile;
            Debug.WriteLine("drop: " + DraggedTile.UniqueId);
        }

    }
}
