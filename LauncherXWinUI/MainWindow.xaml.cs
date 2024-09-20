using LauncherXWinUI.Classes;
using LauncherXWinUI.Controls.GridViewItems;
using LauncherXWinUI.Controls.Dialogs;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using CommunityToolkit.WinUI;
using System.IO;

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

            // Workaround for full screen window messing up the taskbar
            // https://github.com/microsoft/microsoft-ui-xaml/issues/8431
            // This property should only be set if the "Automatically hide the taskbar" in Windows 11,
            // or "Automatically hide the taskbar in desktop mode" in Windows 10 is enabled.
            // Setting this property when the setting is disabled will result in the taskbar overlapping the application
            if (Shell32.IsAutoHideTaskbarEnabled())
                Shell32.SetPropW(WinRT.Interop.WindowNative.GetWindowHandle(this), "NonRudeHWND", new IntPtr(1));

            // Used in-tandem with the code in App.xaml.cs, for WinUIEx to save window position: https://github.com/dotMorten/WinUIEx/issues/61
            this.PersistenceId = "LauncherX-250f2258-1995-4edb-9db7-329a61a90a07";
        }

        // Helper methods
        /// <summary>
        /// Method that updates the UI based on the UserSettingsClass
        /// </summary>
        private void UpdateUIFromSettings()
        {
            // Set header text (Update from HeaderText)
            HeaderTextBlock.Text = UserSettingsClass.HeaderText;

            // Adjust the size of items in ItemsGridView (Update from GridScale)
            foreach (var gridViewItem in ItemsGridView.Items)
            {
                if (gridViewItem is GridViewTile)
                {
                    ((GridViewTile)gridViewItem).Size = UserSettingsClass.GridScale;
                }
                else if (gridViewItem is GridViewTileGroup)
                {
                    GridViewTileGroup gridViewTileGroup = gridViewItem as GridViewTileGroup;
                    gridViewTileGroup.Size = UserSettingsClass.GridScale;

                    // Update the size of the items in the GridViewTileGroup as well
                    foreach (GridViewTile gridViewTile in gridViewTileGroup.Items)
                    {
                        gridViewTile.Size = UserSettingsClass.GridScale;
                    }
                }
            }

            // Set windowing mode to fullscreen if applicable (Update from UseFullscreen)
            if (UserSettingsClass.UseFullscreen == true)
            {
                // Hide custom titlebar
                this.ExtendsContentIntoTitleBar = false;
                AppTitleBar.Visibility = Visibility.Collapsed;

                // Adjust controls
                CloseButton.Visibility = Visibility.Visible;
                ControlsGrid.Margin = new Thickness(20, 10, 20, 0);

                // Set fullscreen
                this.AppWindow.SetPresenter(Microsoft.UI.Windowing.AppWindowPresenterKind.FullScreen);
            }
            else
            {
                // Set custom titlebar
                this.ExtendsContentIntoTitleBar = true;
                AppTitleBar.Visibility = Visibility.Visible;
                this.SetTitleBar(AppTitleBar);

                // Adjust controls
                CloseButton.Visibility = Visibility.Collapsed;
                ControlsGrid.Margin = new Thickness(20, 0, 20, 0);

                // Set normal windowing mode
                this.AppWindow.SetPresenter(Microsoft.UI.Windowing.AppWindowPresenterKind.Default);
            }

            // Align the GridView (Update from GridPosition)
            if (UserSettingsClass.GridPosition == "Left")
            {
                AlignGridViewLeft();
            }
            else if (UserSettingsClass.GridPosition == "Center")
            {
                AlignGridViewCenter();
            }
        }

        /// <summary>
        /// Method that aligns the ItemsGridView to the left
        /// </summary>
        private void AlignGridViewLeft()
        {
            ItemsGridView.HorizontalAlignment = HorizontalAlignment.Stretch;

            // Fix the width of the ItemsGridView to take up the entire space available
            ItemsGridView.Width = ControlsGrid.Width;

            // Adjust scrollbar margins as well
            ScrollViewerExtensions.SetVerticalScrollBarMargin(ItemsGridView, new Thickness(0, 0, 0, 0));
        }

        private void AlignGridViewCenter()
        {
            ItemsGridView.HorizontalAlignment = HorizontalAlignment.Center;

            // Fix the width of the ItemsGridView to perfectly match the row of GridViewTiles/GridViewTileGroups
            // Since the ItemsGridView has HorizontalAlignment = Center, this will thus center the ItemsGridView
            // +4 is because by default, a GridViewItem has a right margin of 4
            ItemsGridView.Width = Math.Floor(ControlsGrid.Width / (105 * Math.Sqrt(UserSettingsClass.GridScale) + 4)) * ((105 * Math.Sqrt(UserSettingsClass.GridScale) + 4));

            // Adjust scrollbar margins as well
            ScrollViewerExtensions.SetVerticalScrollBarMargin(ItemsGridView, new Thickness(0, 0, -20, 0));
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
        /// <returns>The GridViewTile created</returns>
        private GridViewTile AddGridViewTile(string executingPath, string executingArguments, string displayText, BitmapImage imageSource)
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

            return gridViewTile;
        }

        // Event Handlers
        private async void Container_Loaded(object sender, RoutedEventArgs e)
        {
            // Set Window icon
            UIFunctionsClass.SetWindowLauncherXIcon(this);

            // Set Window Background
            UIFunctionsClass.SetWindowBackground(this, ContainerFallbackBackgroundBrush);

            // Show LoadingDialog while loading items and settings
            LoadingDialog.Visibility = Visibility.Visible;

            // Create settings directories
            UserSettingsClass.CreateSettingsDirectories();

            // Upgrade settings and write new settings file if necessary
            if (UserSettingsClass.UpgradeRequired())
            {
                UserSettingsClass.UpgradeUserSettings();
                UserSettingsClass.WriteSettingsFile();
                UserSettingsClass.ClearOldTempDirectories();

                // Retrieve user settings from file
                UserSettingsClass.TryReadSettingsFile();

                // Once we have initialised the UserSettingsClass with the correct values, update the UI
                UpdateUIFromSettings();

                // Monitor when the window is resized so that we can adjust the position of the GridView as necesssary
                this.SizeChanged += WindowEx_SizeChanged;

                // Upgrade items as well
                List<Dictionary<string, object>> oldLauncherXItems = await UserSettingsClass.UpgradeOldLauncherXItems();

                foreach (Dictionary<string, object> gridViewTileProps in oldLauncherXItems)
                {
                    string executingPath = gridViewTileProps["ExecutingPath"] as string;
                    string executingArguments = gridViewTileProps["ExecutingArguments"] as string;
                    string displayText = gridViewTileProps["DisplayText"] as string;
                    BitmapImage imageSource = gridViewTileProps["ImageSource"] as BitmapImage;
                    AddGridViewTile(executingPath, executingArguments, displayText, imageSource);
                }

                // Save items after
                UserSettingsClass.SaveLauncherXItems(ItemsGridView.Items);
            }
            else
            {
                // Retrieve user settings from file
                UserSettingsClass.TryReadSettingsFile();

                // Once we have initialised the UserSettingsClass with the correct values, update the UI
                UpdateUIFromSettings();

                // Monitor when the window is resized so that we can adjust the position of the GridView as necesssary
                this.SizeChanged += WindowEx_SizeChanged;

                // Load LauncherX items as normal
                List<UserControl> controls = await UserSettingsClass.LoadLauncherXItems();

                // Add the loaded controls to the ItemsGridView
                foreach (UserControl control in controls)
                {
                    if (control is GridViewTile)
                    {
                        // Hook up event handlers
                        GridViewTile gridViewTile = (GridViewTile)control;
                        gridViewTile.Drop += GridViewTile_Drop;
                        gridViewTile.DragEnter += GridViewTile_DragEnter;
                        gridViewTile.DragLeave += GridViewTile_DragLeave;
                        ItemsGridView.Items.Add(gridViewTile);
                    }
                    else if (control is GridViewTileGroup)
                    {
                        // Hook up event handlers
                        GridViewTileGroup gridViewTileGroup = (GridViewTileGroup)control;
                        gridViewTileGroup.DragEnter += GridViewTileGroup_DragEnter;
                        gridViewTileGroup.DragLeave += GridViewTileGroup_DragLeave;
                        gridViewTileGroup.Drop += GridViewTileGroup_Drop;
                        ItemsGridView.Items.Add(gridViewTileGroup);

                    }
                }
            }

            // Check if there were errors adding files
            if (UserSettingsClass.ErrorAddingItems() == true)
            {
                AddItemsErrorWindow addItemsErrorWindow = new AddItemsErrorWindow();

                foreach (string path in UserSettingsClass.ErrorPaths)
                {
                    addItemsErrorWindow.Items.Add(path);
                }

                UIFunctionsClass.CreateModalWindow(addItemsErrorWindow, this);
            }

            // Hide LoadingDialog once done
            await Task.Delay(20);
            LoadingDialog.Visibility = Visibility.Collapsed;

            // Check for updates and display the InfoBar if necessary
            bool? isUpdateAvailable = await UpdatesClass.IsUpdateAvailable();
            if (isUpdateAvailable == true)
            {
                UpdateInfoBar.IsOpen = true;
            }

        }

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

        private async void AddFileBtn_Click(object sender, RoutedEventArgs e)
        {
            AddFileDialog addFileDialog = new AddFileDialog()
            {
                XamlRoot = Container.XamlRoot
            };

            ContentDialogResult result = await addFileDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                // Show LoadingDialog while loading items and settings
                LoadingDialog.Visibility = Visibility.Visible;
                await Task.Delay(10);

                // Add the files from the addFileDialog
                foreach (AddFileDialogListViewItem fileItem in addFileDialog.AddedFiles)
                {
                    // Create new GridViewTile for each item
                    AddGridViewTile(fileItem.ExecutingPath, fileItem.ExecutingArguments, fileItem.DisplayText, fileItem.FileIcon);
                }

                // Save items
                UserSettingsClass.SaveLauncherXItems(ItemsGridView.Items);

                // Hide LoadingDialog
                await Task.Delay(20);
                LoadingDialog.Visibility = Visibility.Collapsed;
            }
        }

        private async void AddFolderBtn_Click(object sender, RoutedEventArgs e)
        {
            AddFolderDialog addFolderDialog = new AddFolderDialog()
            {
                XamlRoot = Container.XamlRoot
            };

            ContentDialogResult result = await addFolderDialog.ShowAsync();

            if (result != ContentDialogResult.Primary)
            {
                return;    
            }

            // Show LoadingDialog while loading items and settings
            LoadingDialog.Visibility = Visibility.Visible;
            await Task.Delay(10);

            // Add the folders from the addFolderDialog
            foreach (AddFolderDialogListViewItem folderItem in addFolderDialog.AddedFolders)
            {
                // Create new GridViewTile for each item
                // Depending if FolderType is Shortcut or Linked, we need to add it differently
                if (folderItem.FolderType == "Shortcut")
                {
                    AddGridViewTile(folderItem.ExecutingPath, "", folderItem.DisplayText, folderItem.FolderIcon);
                }
                else if (folderItem.FolderType == "Linked")
                {
                    // Get all the paths in the folder
                    List<string> files = Directory.GetFiles(folderItem.ExecutingPath).ToList();
                    foreach (string filePath in files)
                    {
                        GridViewTile gridViewTile = AddGridViewTile(filePath, "", Path.GetFileName(filePath), await IconHelpers.GetFileIcon(filePath));
                        gridViewTile.IsLinkedFolder = true;
                    }

                    List<string> folders = Directory.GetDirectories(folderItem.ExecutingPath).ToList();
                    foreach (string folderPath in folders)
                    {
                        GridViewTile gridViewTile = AddGridViewTile(folderPath, "", new DirectoryInfo(folderPath).Name, await IconHelpers.GetFolderIcon(folderPath));
                        gridViewTile.IsLinkedFolder = true;
                    }
                }

            }

            // Save items
            UserSettingsClass.SaveLauncherXItems(ItemsGridView.Items);

            // Hide LoadingDialog
            await Task.Delay(20);
            LoadingDialog.Visibility = Visibility.Collapsed;
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
                // Show LoadingDialog while loading items and settings
                LoadingDialog.Visibility = Visibility.Visible;
                await Task.Delay(10);

                // Get the icon of the website
                BitmapImage websiteIcon = IconHelpers.GetWebsiteIcon(addWebsiteDialog.InputWebsiteUrl);

                // Create new GridViewTile to display the website
                AddGridViewTile(addWebsiteDialog.InputWebsiteUrl, "", addWebsiteDialog.InputWebsiteUrl, websiteIcon);
            }

            // Save items
            UserSettingsClass.SaveLauncherXItems(ItemsGridView.Items);

            // Hide LoadingDialog
            await Task.Delay(20);
            LoadingDialog.Visibility = Visibility.Collapsed;
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

                if (DroppedOnTile.UniqueId == DraggedTile.UniqueId)
                {
                    return;
                }

                // Create a new group when a GridViewTile is dropped over another GridViewTile
                GridViewTileGroup newGridViewTileGroup = new GridViewTileGroup();
                newGridViewTileGroup.Size = UserSettingsClass.GridScale;
                newGridViewTileGroup.DisplayText = "New group";
                newGridViewTileGroup.DragEnter += GridViewTileGroup_DragEnter;
                newGridViewTileGroup.DragLeave += GridViewTileGroup_DragLeave;
                newGridViewTileGroup.Drop += GridViewTileGroup_Drop;

                // Add GridViewTiles
                DraggedTile.AssociateGroup(newGridViewTileGroup);
                DroppedOnTile.AssociateGroup(newGridViewTileGroup);
                newGridViewTileGroup.Items.Add(DraggedTile);
                newGridViewTileGroup.Items.Add(DroppedOnTile);

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

                // Add the DraggedTile to the existingGridViewTileGroup
                existingGridViewTileGroup.HideAddItemIndicator();
                DraggedTile.AssociateGroup(existingGridViewTileGroup);
                existingGridViewTileGroup.Items.Add(DraggedTile);

                // Mark objects for deletion
                GridViewItemsToRemove.Add(DraggedTile);
            }
        }

        private void ItemsGridView_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
        {
            // Remove the old GridView items if applicable
            foreach (UserControl control in GridViewItemsToRemove)
            {
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

            // Show LoadingDialog while loading items and settings
            LoadingDialog.Visibility = Visibility.Visible;
            await Task.Delay(10);

            // When a URL is dragged in, it technically qualifies as both a internet shortcut (.url) file, and a WebLink
            // Thus, we check if the DataView contains a WebLink first, so that URLs dragged in are added as websites instead of files
            // Source: https://stackoverflow.com/questions/66973410/drag-and-drop-items-in-windows-application-and-get-the-standarddataformats-of-th
            if (e.DataView.Contains(StandardDataFormats.WebLink))
            {
                // Dragged item is a website
                Uri websiteUri = await e.DataView.GetWebLinkAsync();

                // Add the website
                BitmapImage websiteIcon = IconHelpers.GetWebsiteIcon(websiteUri.ToString());

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
                    // Get folder icon
                    BitmapImage bitmapImage = await IconHelpers.GetFolderIcon(storageItem.Path);

                    // Add folder to ItemsGridView
                    AddGridViewTile(storageItem.Path, "", storageItem.Name, bitmapImage);
                }

                // File
                foreach (var storageItem in items.OfType<StorageFile>())
                {
                    // This is a file
                    string filePath = storageItem.Path;

                    // Get the thumbnail of the file and add it to ItemsGridView
                    AddGridViewTile(filePath, "", storageItem.Name, await IconHelpers.GetFileIcon(filePath));
                }
            }

            // Hide LoadingDialog
            await Task.Delay(20);
            LoadingDialog.Visibility = Visibility.Collapsed;
        }

        // For fullscreen mode - Exit LauncherX
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }

        // When window resized
        private void WindowEx_SizeChanged(object sender, WindowSizeChangedEventArgs args)
        {
            // Align the GridView (Update from GridPosition)
            if (UserSettingsClass.GridPosition == "Left")
            {
                AlignGridViewLeft();
            }
            else if (UserSettingsClass.GridPosition == "Center")
            {
                AlignGridViewCenter();
            }
        }

        // The last event handler - save items when the window is closed
        private void WindowEx_Closed(object sender, WindowEventArgs args)
        {
            UserSettingsClass.SaveLauncherXItems(ItemsGridView.Items);
        }
    }
}
