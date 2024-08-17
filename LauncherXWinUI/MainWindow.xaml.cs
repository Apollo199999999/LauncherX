using LauncherXWinUI.Classes;
using LauncherXWinUI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.IO;
using System.Diagnostics;
using Microsoft.UI.Xaml.Media;
using Windows.UI;
using Windows.ApplicationModel.DataTransfer;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.IO.Compression;
using Windows.Storage;
using Windows.Storage.FileProperties;

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
        /// <summary>
        /// Method that updates the UI based on the UserSettingsClass
        /// </summary>
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
                else if (gridViewItem is GridViewTileGroup)
                {
                    ((GridViewTileGroup)gridViewItem).Size = UserSettingsClass.GridScale;
                }
            }
        }

        /// <summary>
        /// Method that tries to show the DragDropInterface, if the data dragged in is valid
        /// </summary>
        /// <param name="e">The DragEventArgs from the event handler</param>
        public void TryShowDragDropInterface(DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems) || e.DataView.Contains(StandardDataFormats.WebLink))
            {
                // User is dragging files/folders/websites into LauncherX
                DragDropInterface.Visibility = Visibility.Visible;
            }
            else
            {
                DragDropInterface.Visibility = Visibility.Collapsed;
            }

        }

        /// <summary>
        /// Method that adds a new GridViewTile to the ItemsGridView
        /// </summary>
        /// <param name="executingPath">ExecutingPath in GridViewTile</param>
        /// <param name="executingArguments">ExecutingArguments in GridViewTile</param>
        /// <param name="displayText">DisplayText in GridViewTile</param>
        /// <param name="imageSource">ImageSource in GridViewTile</param>
        private void AddGridViewTile(string executingPath, string executingArguments, string displayText, ImageSource imageSource)
        {
            // Create new GridViewTile for each item
            GridViewTile gridViewTile = new GridViewTile();
            gridViewTile.ExecutingPath = executingPath;
            gridViewTile.ExecutingArguments = executingArguments;
            gridViewTile.DisplayText = displayText;
            gridViewTile.ImageSource = imageSource;
            gridViewTile.Size = UserSettingsClass.GridScale;
            gridViewTile.Drop += GridViewTile_Drop;
            gridViewTile.DragEnter += GridViewTile_DragEnter;
            gridViewTile.DragLeave += GridViewTile_DragLeave;
            ItemsGridView.Items.Add(gridViewTile);
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
                    AddGridViewTile(fileItem.ExecutingPath, fileItem.ExecutingArguments, fileItem.DisplayText, fileItem.FileIcon);
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
                // Add the folders from the addFolderDialog
                foreach (AddFolderDialogListViewItem folderItem in addFolderDialog.AddedFolders)
                {
                    // Create new GridViewTile for each item
                    AddGridViewTile(folderItem.ExecutingPath, "", folderItem.DisplayText, folderItem.FolderIcon);
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
                // Get the icon of the website, using Google's favicon service
                BitmapImage websiteIcon = new BitmapImage();
                // Fallback icon
                Uri defaultImageUri = new Uri(Path.GetFullPath(@"Resources\websitePlaceholder.png"), UriKind.Absolute);
                websiteIcon.ImageFailed += (s, e) => 
                {
                    websiteIcon.UriSource = defaultImageUri;
                };
                // Try getting website icon
                Uri iconUri = new Uri("https://www.google.com/s2/favicons?sz=128&domain_url=" + addWebsiteDialog.InputWebsiteUrl, UriKind.Absolute);
                websiteIcon.UriSource = iconUri;

                // Create new GridViewTile to display the website
                AddGridViewTile(addWebsiteDialog.InputWebsiteUrl, "", addWebsiteDialog.InputWebsiteUrl, websiteIcon);
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

        // When an item in the GridView is being dragged, add the control to the data package that is transferred in Drag-Drop operations
        private void ItemsGridView_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            if (e.Items[0] is GridViewTile)
            {
                e.Data.Properties.Add("DraggedControl", (e.Items[0] as GridViewTile));
            }
            else if (e.Items[0] is GridViewTileGroup)
            {
                e.Data.Properties.Add("DraggedControl", (e.Items[0] as GridViewTileGroup));
            }
        }

        // Drag event handlers for GridViewTile: When something is dragged over a GridViewTile, highlight it
        // When a GridViewTile is dropped over a GridViewTile, create a new group
        // Prevent anything from happening when a GridViewTileGroup is dragged over a GridViewTile
        private void GridViewTile_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data != null && e.Data.Properties["DraggedControl"] != null && e.Data.Properties["DraggedControl"] is GridViewTile)
            {
                GridViewTile DraggedTile = e.Data.Properties["DraggedControl"] as GridViewTile;
                GridViewTile DraggedOverTile = sender as GridViewTile;

                if (DraggedTile.UniqueId != DraggedOverTile.UniqueId)
                {
                    // Show some indication that a group can be formed
                    DraggedOverTile.ShowCreateGroupIndicator();
                }
            }
           
        }
        private void GridViewTile_DragLeave(object sender, DragEventArgs e)
        {
            if (e.Data != null && e.Data.Properties["DraggedControl"] != null && e.Data.Properties["DraggedControl"] is GridViewTile)
            {
                GridViewTile DraggedTile = e.Data.Properties["DraggedControl"] as GridViewTile;
                GridViewTile DraggedOverTile = sender as GridViewTile;

                if (DraggedTile.UniqueId != DraggedOverTile.UniqueId)
                {
                    // Hide the create group indicator
                    DraggedOverTile.HideCreateGroupIndicator();
                }
            }
        }

        List<UserControl> GridViewItemsToRemove = new List<UserControl>();

        private void GridViewTile_Drop(object sender, DragEventArgs e)
        {
            if (e.Data != null && e.Data.Properties["DraggedControl"] != null && e.Data.Properties["DraggedControl"] is GridViewTile)
            {
                GridViewTile DroppedOnTile = sender as GridViewTile;
                GridViewTile DraggedTile = e.Data.Properties["DraggedControl"] as GridViewTile;
                DroppedOnTile.HideCreateGroupIndicator();
                DraggedTile.UnhighlightControl();
                DraggedTile.ShowRemoveFromGroupOption();
                DroppedOnTile.ShowRemoveFromGroupOption();

                if (DroppedOnTile.UniqueId == DraggedTile.UniqueId)
                {
                    return;
                }

                // Create a new group when a GridViewTile is dropped over another GridViewTile
                GridViewTileGroup newGridViewTileGroup = new GridViewTileGroup();
                newGridViewTileGroup.Size = UserSettingsClass.GridScale;
                newGridViewTileGroup.DisplayText = "New group";
                newGridViewTileGroup.Items.Add(DraggedTile);
                newGridViewTileGroup.Items.Add(DroppedOnTile);
                newGridViewTileGroup.DragEnter += GridViewTileGroup_DragEnter;
                newGridViewTileGroup.DragLeave += GridViewTileGroup_DragLeave;
                newGridViewTileGroup.Drop += GridViewTileGroup_Drop;

                // Add the GridViewTileGroup
                int index = ItemsGridView.Items.IndexOf(DroppedOnTile);
                ItemsGridView.Items.Insert(index, newGridViewTileGroup);

                // Mark the old GridViewTile objects for deletion
                GridViewItemsToRemove.Add(DroppedOnTile);
                GridViewItemsToRemove.Add(DraggedTile);
            }
        }

        // GridViewTileGroup drag events
        private void GridViewTileGroup_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data != null && e.Data.Properties["DraggedControl"] != null && e.Data.Properties["DraggedControl"] is GridViewTile)
            {
                GridViewTileGroup DraggedOverTileGroup = sender as GridViewTileGroup;

                // Show some indication that a item can be added to the group
                DraggedOverTileGroup.ShowAddItemIndicator();
            }
        }

        private void GridViewTileGroup_DragLeave(object sender, DragEventArgs e)
        {
            if (e.Data != null && e.Data.Properties["DraggedControl"] != null && e.Data.Properties["DraggedControl"] is GridViewTile)
            {
                GridViewTileGroup DraggedOverTileGroup = sender as GridViewTileGroup;

                // Show some indication that a item can be added to the group
                DraggedOverTileGroup.HideAddItemIndicator();
            }
        }

        private void GridViewTileGroup_Drop(object sender, DragEventArgs e)
        {
            if (e.Data != null && e.Data.Properties["DraggedControl"] != null && e.Data.Properties["DraggedControl"] is GridViewTile)
            {
                GridViewTileGroup existingGridViewTileGroup = sender as GridViewTileGroup;
                GridViewTile DraggedTile = e.Data.Properties["DraggedControl"] as GridViewTile;
                DraggedTile.UnhighlightControl();
                DraggedTile.ShowRemoveFromGroupOption();

                // Add the DraggedTile to the existingGridViewTileGroup
                existingGridViewTileGroup.HideAddItemIndicator();
                existingGridViewTileGroup.Items.Add(DraggedTile);
               
                // Mark objects for deletion
                GridViewItemsToRemove.Add(DraggedTile);
            }
        }

        private void ItemsGridView_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
        {
            // Remove the old GridView items if applicable
            foreach (UserControl control in GridViewItemsToRemove) {
                ItemsGridView.Items.Remove(control);
            }
            GridViewItemsToRemove.Clear();

            // Unhighlight all controls, just in case
            foreach (UserControl gridViewItem in ItemsGridView.Items)
            {
                if (gridViewItem is GridViewTile)
                {
                    GridViewTile gridViewTile = gridViewItem as GridViewTile;
                    gridViewTile.UnhighlightControl();
                }
                else if (gridViewItem is GridViewTileGroup)
                {
                    GridViewTileGroup gridViewTileGroup = gridViewItem as GridViewTileGroup;
                    gridViewTileGroup.UnhighlightControl();
                }
            }
        }

        // This section of event handlers handles the AutoSuggestBox, which allows the user to search for things in LauncherX

        List<GridViewTile> AllLauncherXItems = new List<GridViewTile>();
        List<string> SearchBoxDropdownItems = new List<string>();
        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            AllLauncherXItems.Clear();

            // Retrieve all the items in LauncherX
            foreach (UserControl gridViewItem in ItemsGridView.Items)
            {
                if (gridViewItem is GridViewTile)
                {
                    GridViewTile gridViewTile = gridViewItem as GridViewTile;
                    AllLauncherXItems.Add(gridViewTile);
                }
                else if (gridViewItem is GridViewTileGroup)
                {
                    GridViewTileGroup gridViewTileGroup = gridViewItem as GridViewTileGroup;
                    foreach (GridViewTile tile in gridViewTileGroup.Items)
                    {
                        AllLauncherXItems.Add(tile);
                    }
                }
            }
        }

        private void SearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            // Only get results when it was a user typing, 
            // otherwise assume the value got filled in by TextMemberPath 
            // or the handler for SuggestionChosen.
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                // Set the ItemsSource to be your filtered dataset
                SearchBoxDropdownItems = new List<string>();
                foreach (GridViewTile gridViewTile in AllLauncherXItems)
                {
                    if (gridViewTile.DisplayText.ToLower().Contains(sender.Text.ToLower()))
                    {
                        SearchBoxDropdownItems.Add(gridViewTile.DisplayText);
                    }
                }
                sender.ItemsSource = SearchBoxDropdownItems;
            }
        }

        private void SearchBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            sender.Text = args.SelectedItem.ToString();
        }

        private async void SearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            // If there's nothing in the dropdown of the SearchBox, don't do anything
            if (SearchBoxDropdownItems.Count <= 0)
            {
                return;
            }

            string chosenSuggestion = "";
            if (args.ChosenSuggestion != null)
            {
                // User selected an item from the suggestion list, take an action on it here.
                chosenSuggestion = args.ChosenSuggestion.ToString();
            }
            else
            {
                chosenSuggestion = SearchBoxDropdownItems[0];   
            }

            sender.Text = chosenSuggestion;

            // Find the corresponding GridViewTile, and start its associated process
            foreach (GridViewTile gridViewTile in AllLauncherXItems)
            {
                if (gridViewTile.DisplayText.ToLower() == chosenSuggestion.ToLower())
                {
                    await gridViewTile.StartAssociatedProcess();
                }
            }

            AllLauncherXItems.Clear();
            SearchBoxDropdownItems.Clear();
        }

        // This section of event handlers handles the user dragging files/folders/websites into LauncherX
        private void DragDropParent_DragEnter(object sender, DragEventArgs e)
        {
            TryShowDragDropInterface(e);
        }

        private void DragDropParent_DragOver(object sender, DragEventArgs e)
        {
            TryShowDragDropInterface(e);
        }

        private void DragDropParent_DragLeave(object sender, DragEventArgs e)
        {
            DragDropInterface.Visibility = Visibility.Collapsed;
        }

        private void DragDropInterface_DragEnter(object sender, DragEventArgs e)
        {
            // Modify the caption that shows when something is dragged into LauncherX
            e.AcceptedOperation = DataPackageOperation.Copy;
            if (e.DragUIOverride != null)
            {
                e.DragUIOverride.Caption = "Add to LauncherX";
                e.DragUIOverride.IsContentVisible = true;
            }
        }

        private void DragDropInterface_DragOver(object sender, DragEventArgs e)
        {
            // Modify the caption that shows when something is dragged into LauncherX
            e.AcceptedOperation = DataPackageOperation.Copy;
            if (e.DragUIOverride != null)
            {
                e.DragUIOverride.Caption = "Add to LauncherX";
                e.DragUIOverride.IsContentVisible = true;
            }
        }

        private async void DragDropInterface_Drop(object sender, DragEventArgs e)
        {
            DragDropInterface.Visibility = Visibility.Collapsed;

            // When a URL is dragged in, it technicallu qualifies as both a internet shortcut (.url) file, and a WebLink
            // Thus, we check if the DataView contains a WebLink first, so that URLs dragged in are added as websites instead of files
            // Source: https://stackoverflow.com/questions/66973410/drag-and-drop-items-in-windows-application-and-get-the-standarddataformats-of-th
            if (e.DataView.Contains(StandardDataFormats.WebLink))
            {
                // Dragged item is a website

                // Add the website
                Uri websiteUri = await e.DataView.GetWebLinkAsync();

                // Get the icon of the website, using Google's favicon service
                BitmapImage websiteIcon = new BitmapImage();
                // Fallback icon
                Uri defaultImageUri = new Uri(Path.GetFullPath(@"Resources\websitePlaceholder.png"), UriKind.Absolute);
                websiteIcon.ImageFailed += (s, e) =>
                {
                    websiteIcon.UriSource = defaultImageUri;
                };
                // Try getting website icon
                Uri iconUri = new Uri("https://www.google.com/s2/favicons?sz=128&domain_url=" + websiteUri.ToString(), UriKind.Absolute);
                websiteIcon.UriSource = iconUri;

                // Create new GridViewTile to display the website
                AddGridViewTile(websiteUri.ToString(), "", websiteUri.ToString(), websiteIcon);
            }
            else if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                // Dragged item(s) are folders/files
                var items = await e.DataView.GetStorageItemsAsync();

                // Folder
                foreach (var storageItem in items.OfType<StorageFolder>())
                {
                    // This is a folder
                    string folderPath = storageItem.Path;

                    // Get the thumbnail of the folder
                    StorageItemThumbnail thumbnail = await storageItem.GetThumbnailAsync(ThumbnailMode.SingleItem, 256);
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.SetSource(thumbnail);

                    // Add folder to ItemsGridView
                    AddGridViewTile(folderPath, "", storageItem.Name, bitmapImage);
                }

                // File
                foreach (var storageItem in items.OfType<StorageFile>())
                {
                    // This is a file
                    string filePath = storageItem.Path;
                    string ext = Path.GetExtension(filePath);

                    // Get the thumbnail of the file, depending on its extension (explanation in AddFileDialog.xaml.cs)
                    if (ext == ".lnk" || ext == ".url" || ext == ".wsh")
                    {
                        // Get the thumbnail of the file using Win32 APIs
                        IntPtr hIcon = Shell32.GetJumboIcon(Shell32.GetIconIndex(filePath));
                        System.Drawing.Icon ico = (System.Drawing.Icon)System.Drawing.Icon.FromHandle(hIcon).Clone();
                        SoftwareBitmapSource fileIcon = await Shell32.GetWinUI3BitmapSourceFromIcon(ico);
                        AddGridViewTile(filePath, "", storageItem.Name, fileIcon);
                    }
                    else
                    {
                        // Get the thumbnail of the file using WinRT APIs
                        StorageItemThumbnail thumbnail = await storageItem.GetThumbnailAsync(ThumbnailMode.SingleItem, 256);
                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.SetSource(thumbnail);
                        AddGridViewTile(filePath, "", storageItem.Name, bitmapImage);
                    }
                }
            }
            
        }
    }
}
