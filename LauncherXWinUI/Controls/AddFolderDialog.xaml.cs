using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage.FileProperties;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LauncherXWinUI.Controls
{
    public sealed partial class AddFolderDialog : ContentDialog
    {
        public AddFolderDialog()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Folders added to the AddFoldersDialog
        /// </summary>
        public List<AddFolderDialogListViewItem> AddedFolders
        {
            get => (List<AddFolderDialogListViewItem>)GetValue(AddedFoldersProperty);
            set => SetValue(AddedFoldersProperty, value);
        }

        DependencyProperty AddedFoldersProperty = DependencyProperty.Register(
            nameof(AddedFolders),
            typeof(string),
            typeof(AddFolderDialog),
            new PropertyMetadata(new List<AddFolderDialogListViewItem>()));

        // Event Handlers
        private async void PickAFolderButton_Click(object sender, RoutedEventArgs e)
        {
            // Configure UI
            this.IsPrimaryButtonEnabled = false;
            OpenFolderProgressRing.IsActive = true;

            // Create a folder picker
            FolderPicker openPicker = new Windows.Storage.Pickers.FolderPicker();

            // See the sample code below for how to make the window accessible from the App class.
            var window = App.MainWindow;

            // Retrieve the window handle (HWND) of the current WinUI 3 window.
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

            // Initialize the folder picker with the window handle (HWND).
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

            // Set options for your folder picker
            openPicker.FileTypeFilter.Add("*");

            // Open the picker for the user to pick a folder
            StorageFolder folder = await openPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                SelectedFoldersListView.Items.Remove(PlaceholderListViewItem);

                // Display the folder in the ListView
                AddFolderDialogListViewItem addFolderDialogListViewItem = new AddFolderDialogListViewItem();
                addFolderDialogListViewItem.ExecutingPath = folder.Path;
                addFolderDialogListViewItem.DisplayText = folder.Name;
                SelectedFoldersListView.Items.Add(addFolderDialogListViewItem);

                // Get the thumbnail of the folder
                StorageItemThumbnail thumbnail = await folder.GetThumbnailAsync(ThumbnailMode.SingleItem);
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.SetSource(thumbnail);
                addFolderDialogListViewItem.FolderIcon = bitmapImage;
            }

            // Configure UI
            this.IsPrimaryButtonEnabled = true;
            OpenFolderProgressRing.IsActive = false;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Update the AddedFiles property
            AddedFolders.Clear();
            foreach (var item in SelectedFoldersListView.Items)
            {
                AddFolderDialogListViewItem folderItem = item as AddFolderDialogListViewItem;
                if (folderItem != null)
                {
                    AddedFolders.Add(folderItem);
                }
            }
        }
    }
}
