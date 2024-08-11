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
using System.Windows.Forms;

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

            // Unfortunately, WASDK has a 2 year old issue (at the time of writing) about FolderPickers breaking when the app is run as admin: https://github.com/microsoft/WindowsAppSDK/issues/2504
            // WHY THE FUCK IS MICROSHIT DIDDLING AROUND WITH COPILOT AND AI WHEN THEY CANT EVEN GET THIS SHIT RIGHT??!??!?
            // aNYWAYS, we must use the WinForms FolderBrowserDialog as a replacement for now
            // System.Windows.Forms is exposed via the "WinForms Class Library" project
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                string folderPath = folderBrowserDialog.SelectedPath;

                // We can use WASDK APIs from here on out
                SelectedFoldersListView.Items.Remove(PlaceholderListViewItem);

                // Initialize a new StorageFolder object to use WASDK APIs
                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(folderPath);

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
