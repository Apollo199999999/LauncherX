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
                else if (gridViewItem is GridViewTileGroup)
                {
                    ((GridViewTileGroup)gridViewItem).Size = UserSettingsClass.GridScale;
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
                    gridViewTile.DragEnter += GridViewTile_DragEnter;
                    gridViewTile.DragLeave += GridViewTile_DragLeave;
                    gridViewTile.Drop += GridViewTile_Drop;
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
                    gridViewTile.Drop += GridViewTile_Drop;
                    gridViewTile.DragEnter += GridViewTile_DragEnter;
                    gridViewTile.DragLeave += GridViewTile_DragLeave;
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
                gridViewTile.Drop += GridViewTile_Drop;
                gridViewTile.DragEnter += GridViewTile_DragEnter;
                gridViewTile.DragLeave += GridViewTile_DragLeave;

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
            if (e.Data.Properties["DraggedControl"] != null && e.Data.Properties["DraggedControl"] is GridViewTile)
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
            if (e.Data.Properties["DraggedControl"] != null && e.Data.Properties["DraggedControl"] is GridViewTile)
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
            if (e.Data.Properties["DraggedControl"] != null && e.Data.Properties["DraggedControl"] is GridViewTile)
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
            if (e.Data.Properties["DraggedControl"] != null && e.Data.Properties["DraggedControl"] is GridViewTile)
            {
                GridViewTileGroup DraggedOverTileGroup = sender as GridViewTileGroup;

                // Show some indication that a item can be added to the group
                DraggedOverTileGroup.ShowAddItemIndicator();
            }
        }

        private void GridViewTileGroup_DragLeave(object sender, DragEventArgs e)
        {
            if (e.Data.Properties["DraggedControl"] != null && e.Data.Properties["DraggedControl"] is GridViewTile)
            {
                GridViewTileGroup DraggedOverTileGroup = sender as GridViewTileGroup;

                // Show some indication that a item can be added to the group
                DraggedOverTileGroup.HideAddItemIndicator();
            }
        }

        private void GridViewTileGroup_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.Properties["DraggedControl"] != null && e.Data.Properties["DraggedControl"] is GridViewTile)
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
    }
}
