using LauncherXWinUI.Classes;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Popups;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

// Because the GridViewTile may be a child of a GridViewTileGroup, which has a ContentDialog, the GridViewTile cannot implement any ContentDialogs
namespace LauncherXWinUI.Controls.GridViewItems
{
    public sealed partial class GridViewTile : UserControl
    {
        public GridViewTile()
        {
            this.InitializeComponent();

            // For some reason, StackPanel needs a background for right tap to work
            TilePanel.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        }

        // Declare properties that this control will have

        /// <summary>
        /// Size of the control
        /// </summary>
        public double Size
        {
            get => (double)GetValue(SizeProperty);
            set => SetValue(SizeProperty, value);
        }

        DependencyProperty SizeProperty = DependencyProperty.Register(
            nameof(Size),
            typeof(double),
            typeof(GridViewTile),
            new PropertyMetadata(default(double), new PropertyChangedCallback(OnSizeChanged)));

        private static void OnSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GridViewTile gridViewTile = d as GridViewTile;
            double? newScale = e.NewValue as double?;
            if (newScale != null)
            {
                // Calculate new dimensions
                double newSize = Math.Sqrt(newScale.Value);
                double newWidth = 105 * newSize;
                double newHeight = 95 * newSize;

                // Update control dimensions
                gridViewTile.ControlBorder.Width = newWidth;
                gridViewTile.ControlBorder.Height = newHeight;

                // Update image margin and dimensions
                gridViewTile.TileImage.Margin = new Thickness(0, newSize * 5, 0, 0);
                gridViewTile.TileImage.Stretch = Stretch.Uniform;
                gridViewTile.LinkedFolderImage.Margin = new Thickness(newSize * 10, newSize * 5, newSize * 22.5, 0);
                gridViewTile.LinkedFolderImage.Stretch = Stretch.Uniform;

                // Update the font size of the textblock
                gridViewTile.TileText.FontSize = newSize * 12;

                // Update the DecodePixelWidth of the Image control, for anti-aliased rendering
                BitmapImage imgSource = gridViewTile.TileImage.Source as BitmapImage;
                if (imgSource != null)
                {
                    imgSource.DecodePixelWidth = (int)gridViewTile.ControlBorder.Width;
                    gridViewTile.TileImage.Source = imgSource;
                }
            }
        }

        /// <summary>
        /// ImageSource object to be rendered in the control
        /// </summary>
        public BitmapImage ImageSource
        {
            get => (BitmapImage)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        DependencyProperty ImageSourceProperty = DependencyProperty.Register(
            nameof(ImageSource),
            typeof(BitmapImage),
            typeof(GridViewTile),
            new PropertyMetadata(default(BitmapImage), new PropertyChangedCallback(OnImageSourceChanged)));

        private static void OnImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GridViewTile gridViewTile = d as GridViewTile;
            BitmapImage newImageSource = e.NewValue as BitmapImage;

            if (newImageSource != null)
            {
                // Manually set DecodePixelWidth for anti-aliased rendering
                newImageSource.DecodePixelWidth = (int)gridViewTile.ControlBorder.Width;
                gridViewTile.TileImage.Source = newImageSource;
            }
        }

        // Properties that allow the GridViewTile to display custom images selected by the user
        /// <summary>
        /// Path to the custom image
        /// </summary>
        public string CustomImagePath
        {
            get => (string)GetValue(CustomImagePathProperty);
            set => SetValue(CustomImagePathProperty, value);
        }

        DependencyProperty CustomImagePathProperty = DependencyProperty.Register(
            nameof(CustomImagePath),
            typeof(string),
            typeof(GridViewTile),
            new PropertyMetadata(""));

        /// <summary>
        /// Text that is displayed below the image
        /// </summary>
        public string DisplayText
        {
            get => (string)GetValue(DisplayTextProperty);
            set => SetValue(DisplayTextProperty, value);
        }

        DependencyProperty DisplayTextProperty = DependencyProperty.Register(
            nameof(DisplayText),
            typeof(string),
            typeof(GridViewTile),
            new PropertyMetadata(default(string), new PropertyChangedCallback(OnDisplayTextChanged)));

        private static void OnDisplayTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GridViewTile gridViewTile = d as GridViewTile;
            string newDisplayText = e.NewValue as string;

            if (newDisplayText != null)
            {
                // Update textblock
                gridViewTile.TileText.Text = newDisplayText;

                // Update tooltip
                string tooltipString = newDisplayText + " - " + gridViewTile.ExecutingPath;
                ToolTipService.SetToolTip(gridViewTile.TilePanel, tooltipString);
            }
        }


        /// <summary>
        /// The item that will be executed when the tile is clicked
        /// </summary>
        public string ExecutingPath
        {
            get => (string)GetValue(ExecutingPathProperty);
            set => SetValue(ExecutingPathProperty, value);
        }

        DependencyProperty ExecutingPathProperty = DependencyProperty.Register(
            nameof(ExecutingPath),
            typeof(string),
            typeof(GridViewTile),
            new PropertyMetadata(default(string), new PropertyChangedCallback(OnExecutingPathChanged)));

        private static void OnExecutingPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GridViewTile gridViewTile = d as GridViewTile;
            string newExecutingPath = e.NewValue as string;

            if (newExecutingPath != null)
            {
                // Update tooltip
                string tooltipString = gridViewTile.DisplayText + " - " + newExecutingPath;
                ToolTipService.SetToolTip(gridViewTile.TilePanel, tooltipString);
            }
        }

        /// <summary>
        /// Launch arguments, if necessary
        /// </summary>
        public string ExecutingArguments
        {
            get => (string)GetValue(ExecutingArgumentsProperty);
            set => SetValue(ExecutingArgumentsProperty, value);
        }

        DependencyProperty ExecutingArgumentsProperty = DependencyProperty.Register(
            nameof(ExecutingArguments),
            typeof(string),
            typeof(GridViewTile),
            new PropertyMetadata(default(string)));


        /// <summary>
        /// Parent GridViewTileGroup that this belongs to, if applicable
        /// </summary>
        public GridViewTileGroup GroupParent
        {
            get => (GridViewTileGroup)GetValue(GroupParentProperty);
            set => SetValue(GroupParentProperty, value);
        }

        DependencyProperty GroupParentProperty = DependencyProperty.Register(
            nameof(GroupParent),
            typeof(GridViewTileGroup),
            typeof(GridViewTile),
            new PropertyMetadata(null));

        /// <summary>
        /// Whether this item belongs to a "Linked Folder"
        /// </summary>
        public bool IsLinkedFolder
        {
            get => (bool)GetValue(IsLinkedFolderProperty);
            set => SetValue(IsLinkedFolderProperty, value);
        }

        DependencyProperty IsLinkedFolderProperty = DependencyProperty.Register(
            nameof(IsLinkedFolder),
            typeof(bool),
            typeof(GridViewTile),
            new PropertyMetadata(false, new PropertyChangedCallback(OnIsLinkedFolderChanged)));

        private static void OnIsLinkedFolderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GridViewTile gridViewTile = d as GridViewTile;
            bool? newIsLinkedFolder = e.NewValue as bool?;

            if (newIsLinkedFolder == null)
            {
                return;
            }
            else if (newIsLinkedFolder == true)
            {
                gridViewTile.LinkedFolderImage.Visibility = Visibility.Visible;
                gridViewTile.MenuUnlinkOption.Visibility = Visibility.Visible;
                gridViewTile.MenuRemoveOption.Visibility = Visibility.Collapsed;
            }
            else if (newIsLinkedFolder == false)
            {
                gridViewTile.LinkedFolderImage.Visibility = Visibility.Collapsed;
                gridViewTile.MenuUnlinkOption.Visibility = Visibility.Collapsed;
                gridViewTile.MenuRemoveOption.Visibility = Visibility.Visible;
            }
        }

        // Methods

        /// <summary>
        /// Highlights the control by drawing a border around it
        /// </summary>
        public void HighlightControl()
        {
            ControlBorder.BorderBrush = Application.Current.Resources["AccentFillColorDefaultBrush"] as SolidColorBrush;
        }

        /// <summary>
        /// Unhighlights the control by removing the border around it
        /// </summary>
        public void UnhighlightControl()
        {
            ControlBorder.BorderBrush = null;
        }

        /// <summary>
        /// Show the flyout to indicate that a new group can be created and highlight the control
        /// </summary>
        public void ShowCreateGroupIndicator()
        {
            MenuFlyout flyoutBase = (MenuFlyout)FlyoutBase.GetAttachedFlyout(GridViewTileControl);
            flyoutBase.ShowAt(GridViewTileControl);
            HighlightControl();
        }

        /// <summary>
        /// Hide the flyout that indicates that a new group can be created and unhighlight the control
        /// </summary>
        public void HideCreateGroupIndicator()
        {
            MenuFlyout flyoutBase = (MenuFlyout)FlyoutBase.GetAttachedFlyout(GridViewTileControl);
            flyoutBase.Hide();
            UnhighlightControl();
        }

        /// <summary>
        /// Method that prepares this control to be a child of a GridViewTileGroup
        /// </summary>
        public void AssociateGroup(GridViewTileGroup gridViewTileGroup)
        {
            this.GroupParent = gridViewTileGroup;
            MenuRemoveGroupOption.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Method that unassociates this control with the GridViewTileGroup
        /// </summary>
        public void UnassociateGroup()
        {
            this.GroupParent = null;
            MenuRemoveGroupOption.Visibility = Visibility.Collapsed;
        }

        // Helper functions
        /// <summary>
        /// Determines if a given path belongs to that of a file or folder
        /// </summary>
        /// <param name="path">Path of file/folder</param>
        /// <returns>Returns true if path is a folder, false otherwise</returns>
        /// <exception cref="ArgumentNullException">Thrown when no path argument is input.</exception>
        private bool IsPathDirectory(string path)
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

        /// <summary>
        /// Method to start the process associated with this GridViewTile
        /// </summary>
        public async Task StartAssociatedProcess(bool runAsAdmin = false)
        {
            // Try to start the executing path
            try
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo { FileName = ExecutingPath, UseShellExecute = true, Arguments = ExecutingArguments };
                if (runAsAdmin == true)
                {
                    processStartInfo.Verb = "runas";
                }
                Process.Start(processStartInfo);
            }
            catch
            {
                // Use a MessageDialog to show the error message
                MessageDialog messageDialog = new MessageDialog("If you are attempting to run this item as an administrator, check that it is possible to do so in the first place. " +
                                  "Finally, check that the file/folder has not been moved or deleted.", "Error running item");
                WinRT.Interop.InitializeWithWindow.Initialize(messageDialog, WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow));

                await messageDialog.ShowAsync();
            }

            // Unselect this item, only if parent gridview is single select
            await Task.Delay(500);
            GridView parentGridView = this.Parent as GridView;

            if (parentGridView == null)
            {
                return;
            }

            if (parentGridView.SelectionMode == ListViewSelectionMode.Single)
            {
                UnhighlightControl();
                parentGridView.SelectedItem = null;
            }
        }

        /// <summary>
        /// Removes this item from the parent GridView
        /// </summary>
        public void RemoveFromGridView()
        {
            // Remove this group
            GridView parentGridView = this.Parent as GridView;
            if (parentGridView != null)
            {
                parentGridView.Items.Remove(this);
            }
        }

        /// <summary>
        /// Check if we should allow clicking interaction with this item, based on the selection mode of the parent GridView
        /// </summary>
        /// <returns>true if GridView is single select</returns>
        private bool IsInteractionEnabled()
        {
            GridView parentGridView = this.Parent as GridView;
            if (parentGridView.SelectionMode == ListViewSelectionMode.Single)
            {
                return true;
            }
            return false;
        }

        // Event handlers
        // For event handlers relating to left/right clicking the GridViewTile,
        // we only enable them if the parent GridView has "Single" selection mode,
        // as if we are in multiselect, we want the users to be able to select multiple items
        private void GridViewTileControl_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (IsInteractionEnabled())
            {
                HighlightControl();
            }
        }

        private void GridViewTileControl_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (IsInteractionEnabled())
            {
                UnhighlightControl();
            }
        }

        private async void GridViewTileControl_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (IsInteractionEnabled())
            {
                await StartAssociatedProcess();
            }
        }

        private void GridViewTileControl_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (IsInteractionEnabled() == false)
            {   
                return;
            }

            // Implement right click menu
            // Rename options based on the ExecutingPath
            if (ExecutingPath.StartsWith("http://") || ExecutingPath.StartsWith("https://"))
            {
                // This item belongs to a website
                MenuOpenOptionIcon.Glyph = "\uE774";
                MenuOpenOption.Text = "Open website";
                MenuOpenLocOption.Visibility = Visibility.Collapsed;
                MenuAdminOption.Visibility = Visibility.Collapsed;
                MenuRemoveGroupOption.Text = "Remove website from group";
                MenuRemoveOption.Text = "Remove website from LauncherX";
            }
            else if (IsPathDirectory(ExecutingPath))
            {
                MenuOpenOptionIcon.Glyph = "\uE8DA";
                MenuOpenOption.Text = "Open folder";
                MenuOpenLocOption.Text = "Open folder location";
                MenuRemoveGroupOption.Text = "Remove folder from group";
                MenuRemoveOption.Text = "Remove folder from LauncherX";
                MenuAdminOption.Visibility = Visibility.Collapsed;
            }
            else
            {
                MenuOpenOptionIcon.Glyph = "\uE8E5";
                MenuOpenOption.Text = "Open file";
                MenuOpenLocOption.Text = "Open file location";
                MenuRemoveGroupOption.Text = "Remove file from group";
                MenuRemoveOption.Text = "Remove file from LauncherX";
                MenuAdminOption.Visibility = Visibility.Visible;
            }

            MenuFlyout flyoutBase = (MenuFlyout)FlyoutBase.GetAttachedFlyout(TilePanel);
            flyoutBase.ShowAt(TilePanel, e.GetPosition(TilePanel));
        }

        // Event handlers for right click menu options
        private async void MenuOpenOption_Click(object sender, RoutedEventArgs e)
        {
            // Start the process
            await StartAssociatedProcess();
        }

        private async void MenuAdminOption_Click(object sender, RoutedEventArgs e)
        {
            // Start the process as admin
            await StartAssociatedProcess(true);
        }

        private async void MenuOpenLocOption_Click(object sender, RoutedEventArgs e)
        {
            // Try to open file location
            try
            {
                Process.Start(new ProcessStartInfo { FileName = "explorer.exe", UseShellExecute = true, Arguments = "/select, " + ExecutingPath });
            }
            catch
            {
                // Show error message if unable to open file location
                // Use a MessageDialog to show the error message
                MessageDialog messageDialog = new MessageDialog("Unable to open location on disk. Check that the file/folder has not been moved or deleted, or try again later.",
                    "Error opening location");
                WinRT.Interop.InitializeWithWindow.Initialize(messageDialog, WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow));

                await messageDialog.ShowAsync();
            }
        }

        private void MenuRemoveOption_Click(object sender, RoutedEventArgs e)
        {
            // Remove this item
            GridView parentGridView = this.Parent as GridView;
            if (parentGridView != null)
            {
                parentGridView.Items.Remove(this);
            }
        }

        private void MenuRemoveGroupOption_Click(object sender, RoutedEventArgs e)
        {
            // Remove this item from the group
            GridView tileGroupGridView = this.Parent as GridView;
            if (tileGroupGridView != null)
            {
                tileGroupGridView.Items.Remove(this);
            }

            // Add this item to the GridView in MainWindow
            GridView mainWindowGridView = App.MainWindow.ItemsGridView;
            UnassociateGroup();
            mainWindowGridView.Items.Add(this);

        }
        private void MenuUnlinkOption_Click(object sender, RoutedEventArgs e)
        {
            // Find all items in LauncherX that are part of linked folders
            List<UserControl> launcherXItems = new List<UserControl>();

            foreach (UserControl userControl in App.MainWindow.ItemsGridView.Items.Cast<UserControl>())
            {
                launcherXItems.Add(userControl);
            }

            List<GridViewTile> linkedFolderGridViewTiles = UserSettingsClass.FindAllLinkedFolderGridViewTiles(launcherXItems);

            // Get the linked folder associated with this GridViewTile
            string linkedFolder = Path.GetDirectoryName(ExecutingPath);
            App.MainWindow.multiFileSystemWatcher.WatchedPaths.Remove(linkedFolder);

            // Unlink all GridViewTiles with this linked folder
            foreach (GridViewTile linkedTile in linkedFolderGridViewTiles)
            {
                if (Path.GetDirectoryName(linkedTile.ExecutingPath) == linkedFolder)
                {
                    linkedTile.IsLinkedFolder = false;
                }
                
            }

            this.IsLinkedFolder = false;
        }

        // This section handles events for the EditItemWindow and its associated functions
        private string TempCustomImagePath = "";
        private EditItemWindow editItemWindow;
        private void MenuEditOption_Click(object sender, RoutedEventArgs e)
        {
            // Show the EditItemWindow
            editItemWindow = new EditItemWindow();
            editItemWindow.EditDialogImage.Source = this.ImageSource;
            editItemWindow.EditDisplayTextTextBox.Text = this.DisplayText;
            TempCustomImagePath = this.CustomImagePath;

            // Show the launch args section only if this is a file
            if (!this.ExecutingPath.StartsWith("https://") && !this.ExecutingPath.StartsWith("http://") && IsPathDirectory(this.ExecutingPath) == false)
            {
                editItemWindow.EditLaunchArgsTextBox.Visibility = Visibility.Visible;
                editItemWindow.LaunchArgsTextBlock.Visibility = Visibility.Visible;
                editItemWindow.EditLaunchArgsTextBox.Text = this.ExecutingArguments;
            }

            // Hook up event handlers
            editItemWindow.EditIconBtn.Click += EditItemWindow_EditIconBtn_Click;
            editItemWindow.ResetIconBtn.Click += EditItemWindow_ResetIconBtn_Click;
            editItemWindow.SaveBtn.Click += SaveBtn_Click;
            editItemWindow.Closed += EditItemWindow_Closed;

            // Show the window
            UIFunctionsClass.CreateModalWindow(editItemWindow, App.MainWindow);
        }

        private void EditItemWindow_Closed(object sender, WindowEventArgs args)
        {
            editItemWindow = null;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            // Update props of this GridViewTile
            this.DisplayText = editItemWindow.EditDisplayTextTextBox.Text;
            this.ImageSource = editItemWindow.EditDialogImage.Source as BitmapImage;
            this.CustomImagePath = TempCustomImagePath;

            // Update launch args only if file
            // Show the launch args section only if this is a file
            if (!this.ExecutingPath.StartsWith("https://") && !this.ExecutingPath.StartsWith("http://") && IsPathDirectory(this.ExecutingPath) == false)
            {
                this.ExecutingArguments = editItemWindow.EditLaunchArgsTextBox.Text;
            }

            // If this GridViewTile belongs to a GridViewTileGroup, update its preview
            if (this.GroupParent != null)
            {
                this.GroupParent.UpdatePreview();
            }

            // Close the editItemWindow
            editItemWindow.Close();
        }

        private void EditItemWindow_EditIconBtn_Click(object sender, RoutedEventArgs e)
        {
            // Let the user select an image file
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.DereferenceLinks = false;
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.tif;*.tiff;*.bmp;*.ico";

            // When the user select the image
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // Get the file's image
                string filePath = openFileDialog.FileName;

                // Set the image source of the EditDialogImage
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.UriSource = new Uri(filePath, UriKind.Absolute);
                editItemWindow.EditDialogImage.Source = bitmapImage;
                TempCustomImagePath = filePath;
            }

        }

        private async void EditItemWindow_ResetIconBtn_Click(object sender, RoutedEventArgs e)
        {
            // Depending on the executable path, reset the icon in different ways
            if (this.ExecutingPath.StartsWith("http"))
            {
                // Website
                // Get the icon of the website
                BitmapImage websiteIcon = IconHelpers.GetWebsiteIcon(this.ExecutingPath);
                editItemWindow.EditDialogImage.Source = websiteIcon;
            }
            else if (IsPathDirectory(this.ExecutingPath))
            {
                // Folder
                BitmapImage folderIcon = await IconHelpers.GetFolderIcon(this.ExecutingPath);
                editItemWindow.EditDialogImage.Source = folderIcon;
            }
            else if (IsPathDirectory(this.ExecutingPath) == false)
            {
                // File
                editItemWindow.EditDialogImage.Source = await IconHelpers.GetFileIcon(this.ExecutingPath);
            }

            TempCustomImagePath = "";
        }
    }
}
