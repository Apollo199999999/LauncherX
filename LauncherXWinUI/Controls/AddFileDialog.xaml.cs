using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Management.Deployment;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LauncherXWinUI.Controls
{
    public sealed partial class AddFileDialog : ContentDialog
    {
        public AddFileDialog()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Files added to the AddFileDialog
        /// </summary>
        public List<AddFileDialogListViewItem> AddedFiles
        {
            get => (List<AddFileDialogListViewItem>)GetValue(AddedFilesProperty);
            set => SetValue(AddedFilesProperty, value);
        }

        DependencyProperty AddedFilesProperty = DependencyProperty.Register(
            nameof(AddedFiles),
            typeof(string),
            typeof(AddFileDialog),
            new PropertyMetadata(new List<AddFileDialogListViewItem>()));

        // Event handlers
        private async void PickAFileButton_Click(object sender, RoutedEventArgs e)
        {
            // Configure UI
            this.IsPrimaryButtonEnabled = false;
            OpenFilesProgressRing.IsActive = true;

            // Create a file picker
            var openPicker = new Windows.Storage.Pickers.FileOpenPicker();

            // See the sample code below for how to make the window accessible from the App class.
            var window = App.MainWindow;

            // Retrieve the window handle (HWND) of the current WinUI 3 window.
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

            // Initialize the file picker with the window handle (HWND).
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

            // Set options for your file picker
            openPicker.ViewMode = PickerViewMode.List;
            openPicker.FileTypeFilter.Add("*");

            // TODO: Deal with shorttcuts
            // Open the picker for the user to pick a file
            IReadOnlyList<StorageFile> files = await openPicker.PickMultipleFilesAsync();
            if (files.Count > 0)
            {
                SelectedFilesListView.Items.Remove(PlaceholderListViewItem);
                foreach (StorageFile file in files)
                {
                    // Display the files in the ListView
                    AddFileDialogListViewItem addFileDialogListViewItem = new AddFileDialogListViewItem();
                    addFileDialogListViewItem.ExecutingPath = file.Path;
                    addFileDialogListViewItem.DisplayText = file.Name;
                    SelectedFilesListView.Items.Add(addFileDialogListViewItem);

                    // Get the thumbnail of the file
                    StorageItemThumbnail thumbnail = await file.GetThumbnailAsync(ThumbnailMode.SingleItem);
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.SetSource(thumbnail);
                    addFileDialogListViewItem.FileIcon = bitmapImage;
                }
            }

            // Configure UI
            this.IsPrimaryButtonEnabled = true;
            OpenFilesProgressRing.IsActive = false;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Update the AddedFiles property
            AddedFiles.Clear();
            foreach (var item in SelectedFilesListView.Items)
            {
                AddFileDialogListViewItem fileItem = item as AddFileDialogListViewItem;
                if (fileItem != null)
                {
                    AddedFiles.Add(fileItem);
                }
            }
        }
    }
}
