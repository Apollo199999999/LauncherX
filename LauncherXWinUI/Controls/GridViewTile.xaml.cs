using LauncherXWinUI.Classes;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LauncherXWinUI.Controls
{
    public sealed partial class GridViewTile : UserControl
    {
        public GridViewTile()
        {
            this.InitializeComponent();

            // For some reason, StackPanel needs a background for right tap to work
            TilePanel.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

            // Set the unique id to some guid
            this.UniqueId = System.Guid.NewGuid().ToString();
        }

        // Declare properties that this control will have

        /// <summary>
        /// A unique GUID to identify each item in the ItemsGridView
        /// </summary>
        public string UniqueId
        {
            get => (string)GetValue(UniqueIdProperty);
            set => SetValue(UniqueIdProperty, value);
        }

        DependencyProperty UniqueIdProperty = DependencyProperty.Register(
            nameof(UniqueId),
            typeof(string),
            typeof(GridViewTile),
            new PropertyMetadata(default(string)));


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
                gridViewTile.TileImage.Margin = new Thickness(newSize * 22.5, newSize * 5, newSize * 22.5, 0);
                gridViewTile.TileImage.Height = newWidth - newSize * 22.5 - newSize * 22.5;
                //gridViewTile.TileImage.Width = newWidth - newSize * 22.5 - newSize * 22.5;
                gridViewTile.TileImage.Stretch = Stretch.Uniform;

                // Update the font size of the textblock
                gridViewTile.TileText.FontSize = newSize * 12;
            }
        }


        /// <summary>
        /// Path to the image file to be rendered in the control
        /// </summary>
        public ImageSource ImageSource
        {
            get => (ImageSource)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        DependencyProperty ImageSourceProperty = DependencyProperty.Register(
            nameof(ImageSource),
            typeof(ImageSource),
            typeof(GridViewTile),
            new PropertyMetadata(default(ImageSource), new PropertyChangedCallback(OnImageSourceChanged)));

        private static void OnImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GridViewTile gridViewTile = d as GridViewTile;
            ImageSource newImageSource = e.NewValue as ImageSource;

            if (newImageSource != null)
            {
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
                ToolTipService.SetToolTip(gridViewTile.TilePanel, newExecutingPath);
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
        /// Method that shows the "Remove from group" option in the right click menu
        /// </summary>
        public void ShowRemoveFromGroupOption()
        {
            MenuRemoveGroupOption.Visibility = Visibility.Visible;
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
                // Second try-catch as the error ContentDialog will not show if this GridViewTile is in a GridViewTileGroup
                // Show a ContentDialog to tell the user something went wrong
                try
                {
                    ContentDialog procErrorDialog = new ContentDialog()
                    {
                        XamlRoot = this.XamlRoot,
                        Title = "Error running item",
                        Content = "If you are attempting to run this item as an administrator, check that it is possible to do so in the first place. " +
                                  "Finally, check that the file/folder has not been moved or deleted.",
                        CloseButtonText = "OK",
                        Width = 300,
                        DefaultButton = ContentDialogButton.Close
                    };

                    await procErrorDialog.ShowAsync();
                }
                catch { }
            }

            // Unselect this item
            await Task.Delay(500);
            UnhighlightControl();
            GridView parentGridView = this.Parent as GridView;
            if (parentGridView != null)
            {
                parentGridView.SelectedItem = null;
            }
        }

        // Event handlers
        private void GridViewTileControl_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            HighlightControl();
        }

        private void GridViewTileControl_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            UnhighlightControl();
        }

        private async void GridViewTileControl_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await StartAssociatedProcess();
        }

        private void GridViewTileControl_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            // Implement right click menu
            // Rename options based on the ExecutingPath
            if (ExecutingPath.StartsWith("http://") || ExecutingPath.StartsWith("https://"))
            {
                // This item belongs to a website
                MenuOpenOptionIcon.Glyph = "\uE774";
                MenuOpenOption.Text = "Open website";
                MenuOpenLocOption.Visibility = Visibility.Collapsed;
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
            }
            else
            {
                MenuOpenOptionIcon.Glyph = "\uE8E5";
                MenuOpenOption.Text = "Open file";
                MenuOpenLocOption.Text = "Open file location";
                MenuRemoveGroupOption.Text = "Remove file from group";
                MenuRemoveOption.Text = "Remove file from LauncherX";
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
                ContentDialog procErrorDialog = new ContentDialog()
                {
                    XamlRoot = this.XamlRoot,
                    Title = "Error opening location",
                    Content = "Unable to open location on disk. Check that the file/folder has not been moved or deleted, or try again later.",
                    CloseButtonText = "OK",
                    DefaultButton = ContentDialogButton.Close
                };

                await procErrorDialog.ShowAsync();
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
            MenuRemoveGroupOption.Visibility = Visibility.Collapsed;
            mainWindowGridView.Items.Add(this);

        }

        // This section handles events for the EditItemDialog and its associated functions
        private string TempCustomImagePath = "";
        private async void MenuEditOption_Click(object sender, RoutedEventArgs e)
        {
            // Show the EditItemDialog
            EditDialogImage.Source = this.ImageSource;
            EditDisplayTextTextBox.Text = this.DisplayText;
            TempCustomImagePath = this.CustomImagePath;

            // Show the launch args section only if this is a file
            if (this.ExecutingPath.StartsWith("http") == false && IsPathDirectory(this.ExecutingPath) == false)
            {
                EditLaunchArgsTextBox.Visibility = Visibility.Visible;
                LaunchArgsTextBlock.Visibility = Visibility.Visible;
                EditLaunchArgsTextBox.Text = this.ExecutingArguments;
            }

            ContentDialogResult result = await EditItemDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                // Update props of this GridViewTile
                this.DisplayText = EditDisplayTextTextBox.Text;
                this.ImageSource = EditDialogImage.Source;
                this.CustomImagePath = TempCustomImagePath;

                // Update launch args only if file
                // Show the launch args section only if this is a file
                if (this.ExecutingPath.StartsWith("http") == false && IsPathDirectory(this.ExecutingPath) == false)
                {
                    this.ExecutingArguments = EditLaunchArgsTextBox.Text;
                }
            }
        }

        private void EditIconBtn_Click(object sender, RoutedEventArgs e)
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
                EditDialogImage.Source = bitmapImage;
                TempCustomImagePath = filePath;
            }

        }

        private async void ResetIconBtn_Click(object sender, RoutedEventArgs e)
        {
            // Depending on the executable path, reset the icon in different ways
            if (this.ExecutingPath.StartsWith("http"))
            {
                // Website
                // Get the icon of the website
                BitmapImage websiteIcon = IconHelpers.GetWebsiteIcon(this.ExecutingPath);
                EditDialogImage.Source = websiteIcon;
            }
            else if (IsPathDirectory(this.ExecutingPath))
            {
                // Folder
                StorageFolder storageFolder = await StorageFolder.GetFolderFromPathAsync(this.ExecutingPath);
                BitmapImage folderIcon = await IconHelpers.GetFolderIcon(storageFolder);
                EditDialogImage.Source = folderIcon;
            }
            else if (IsPathDirectory(this.ExecutingPath) == false)
            {
                // File
                try
                {
                    StorageFile storageFile = await StorageFile.GetFileFromPathAsync(this.ExecutingPath);
                    BitmapImage fileIcon = await IconHelpers.GetFileIcon(storageFile);
                    EditDialogImage.Source = fileIcon;
                }
                catch
                {
                    EditDialogImage.Source = await IconHelpers.GetFileIconWin32(this.ExecutingPath);
                }
            }

            TempCustomImagePath = "";
        }
    }
}
