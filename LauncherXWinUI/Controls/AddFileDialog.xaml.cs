using LauncherXWinUI.Classes;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;

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

            // Unfortuantely, the FileOpenPicker that WASDK provides does not support opening .lnk, .url, and .wsh files (because StorageFile doesn't support it),
            // which is unacceptable, hence we must use the OpenFilePicker from WinForms to pick files
            // The System.Windows.Forms namespace is accessed via a project reference to the "WinFormsClassLibrary" project
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.DereferenceLinks = false;
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                foreach (string filePath in openFileDialog.FileNames)
                {
                    SelectedFilesListView.Items.Remove(PlaceholderListViewItem);

                    // Manually configure the AddFileDialogListViewItem without using StorageFile
                    AddFileDialogListViewItem addFileDialogListViewItem = new AddFileDialogListViewItem();
                    addFileDialogListViewItem.ExecutingPath = filePath;
                    addFileDialogListViewItem.DisplayText = Path.GetFileName(filePath);
                    SelectedFilesListView.Items.Add(addFileDialogListViewItem);

                    // Get the thumbnail of the file using IconHelpers, which will automatically select the most optimal method
                    addFileDialogListViewItem.FileIcon = await IconHelpers.GetFileIcon(filePath);
                }
            }

            // Clean up
            openFileDialog.Dispose();

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
