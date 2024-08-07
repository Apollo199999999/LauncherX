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
                gridViewTile.GridViewTileControl.Width = newWidth;
                gridViewTile.GridViewTileControl.Height = newHeight;
                gridViewTile.TilePanel.Width = newWidth;
                gridViewTile.TilePanel.Height = newHeight;

                // Update image margin and dimensions
                gridViewTile.TileImage.Margin = new Thickness(newSize * 22.5, newSize * 5, newSize * 22.5, 0);
                gridViewTile.TileImage.Height = newWidth - newSize * 22.5 - newSize * 22.5;
                gridViewTile.TileImage.Width = newWidth - newSize * 22.5 - newSize * 22.5;
                gridViewTile.TileImage.Stretch = Stretch.Uniform;

                // Update the font size of the textblock
                gridViewTile.TileText.FontSize = newSize * 12;
            }
        }


        /// <summary>
        /// Path to the image file to be rendered in the control
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
                gridViewTile.TileImage.Source = newImageSource;
            }
        }


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


        // Event handlers
        private async void GridViewTileControl_Tapped(object sender, TappedRoutedEventArgs e)
        {
            // Try to start the executing path
            try
            {
                Process.Start(new ProcessStartInfo { FileName = ExecutingPath, UseShellExecute = true, Arguments = ExecutingArguments });
            }
            catch
            {
                // Show a content dialog to tell the user something went wrong
                ContentDialog procErrorDialog = new ContentDialog()
                {
                    XamlRoot = this.XamlRoot,
                    Title = "Error running process",
                    Content = "Unable to run process. Check that the file/folder has not been moved or deleted, or try again later.",
                    CloseButtonText = "OK",
                    DefaultButton = ContentDialogButton.Close
                };

                await procErrorDialog.ShowAsync();
            }

            // Unselect this item
            await Task.Delay(500);
            GridView parentGridView = this.Parent as GridView;
            if (parentGridView != null)
            {
                parentGridView.SelectedItem = null;
            }
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
                MenuRemoveOption.Text = "Remove website from LauncherX";
            }
            else if (IsPathDirectory(ExecutingPath))
            {
                MenuOpenOptionIcon.Glyph = "\uE8DA";
                MenuOpenOption.Text = "Open folder";
                MenuOpenLocOption.Text = "Open folder location";
                MenuRemoveOption.Text = "Remove folder from LauncherX";
            }
            else
            {
                MenuOpenOptionIcon.Glyph = "\uE8E5";
                MenuOpenOption.Text = "Open file";
                MenuOpenLocOption.Text = "Open file location";
                MenuRemoveOption.Text = "Remove file from LauncherX";
            }

            MenuFlyout flyoutBase = (MenuFlyout)FlyoutBase.GetAttachedFlyout(TilePanel);
            flyoutBase.ShowAt(TilePanel, e.GetPosition(TilePanel));
        }

        // Event handlers for right click menu options
        private void MenuOpenOption_Click(object sender, RoutedEventArgs e)
        {
            // Start the process
            GridViewTileControl_Tapped(null, null);
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

        
    }
}
