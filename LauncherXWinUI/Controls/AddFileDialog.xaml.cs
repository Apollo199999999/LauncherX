using LauncherXWinUI.Classes;
using Microsoft.UI;
using Microsoft.UI.Windowing;
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

using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
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

        // Helper functions
        // THANK GOD FOR STACKOVERFLOW: https://stackoverflow.com/questions/28525925/get-icon-128128-file-type-c-sharp/28530403#28530403
        // Get high-resolution icon of a file using Shell32/Win32 API
        int GetIconIndex(string pszFile)
        {
            SHFILEINFO sfi = new SHFILEINFO();
            Shell32.SHGetFileInfo(pszFile
                , 0
                , ref sfi
                , (uint)System.Runtime.InteropServices.Marshal.SizeOf(sfi)
                , (uint)(SHGFI.SysIconIndex | SHGFI.LargeIcon | SHGFI.UseFileAttributes));
            return sfi.iIcon;
        }

        // 256*256
        IntPtr GetJumboIcon(int iImage)
        {
            const string IID_IImageList = "46EB5926-582E-4017-9FDF-E8998DAA0950";
            const string IID_IImageList2 = "192B9D83-50FC-457B-90A0-2B82A8B5DAE1";

            IImageList spiml = null;
            Guid guil = new Guid(IID_IImageList2);//or IID_IImageList

            Shell32.SHGetImageList(Shell32.SHIL_JUMBO, ref guil, ref spiml);
            IntPtr hIcon = IntPtr.Zero;
            spiml.GetIcon(iImage, Shell32.ILD_TRANSPARENT | Shell32.ILD_IMAGE, ref hIcon); //

            return hIcon;
        }

        // THANK GOD FOR STACKOVERFLOW: https://stackoverflow.com/questions/76640972/convert-system-drawing-icon-to-microsoft-ui-xaml-imagesource
        /// <summary>
        /// Converts System.Drawing.Icon to SoftwareBitmapSource
        /// </summary>
        /// <param name="icon">Icon to convert</param>
        /// <returns>SoftwareBitmapSource for Image Control</returns>
        public static async Task<SoftwareBitmapSource> GetWinUI3BitmapSourceFromIcon(System.Drawing.Icon icon)
        {
            if (icon == null)
                return null;

            // convert to bitmap
            using var bmp = icon.ToBitmap();
            return await GetWinUI3BitmapSourceFromGdiBitmap(bmp);
        }

        /// <summary>
        /// Converts System.Drawing.Bitmap to SoftwareBitmapSource
        /// </summary>
        /// <param name="bmp">Bitmap to convert</param>
        /// <returns>SoftwareBitmapSource for Image Control</returns>
        public static async Task<SoftwareBitmapSource> GetWinUI3BitmapSourceFromGdiBitmap(System.Drawing.Bitmap bmp)
        {
            if (bmp == null)
                return null;

            // get pixels as an array of bytes
            var data = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);
            var bytes = new byte[data.Stride * data.Height];
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);
            bmp.UnlockBits(data);

            // get WinRT SoftwareBitmap
            var softwareBitmap = new Windows.Graphics.Imaging.SoftwareBitmap(
                Windows.Graphics.Imaging.BitmapPixelFormat.Bgra8,
                bmp.Width,
                bmp.Height,
                Windows.Graphics.Imaging.BitmapAlphaMode.Premultiplied);
            softwareBitmap.CopyFromBuffer(bytes.AsBuffer());

            // build WinUI3 SoftwareBitmapSource
            var source = new Microsoft.UI.Xaml.Media.Imaging.SoftwareBitmapSource();
            await source.SetBitmapAsync(softwareBitmap);
            return source;
        }

        // Event handlers
        private async void PickAFileButton_Click(object sender, RoutedEventArgs e)
        {
            // Configure UI
            this.IsPrimaryButtonEnabled = false;
            OpenFilesProgressRing.IsActive = true;

            // Unfortuantely, the FileOpenPicker that WASDK provides does not support opening .lnk, .url, and .wsh files,
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

                    string ext = Path.GetExtension(filePath);
                    if (ext == ".lnk" || ext == ".url" || ext == ".wsh")
                    {
                        // Manually configure the AddFileDialogListViewItem without using StorageFile
                        AddFileDialogListViewItem addFileDialogListViewItem = new AddFileDialogListViewItem();
                        addFileDialogListViewItem.ExecutingPath = filePath;
                        addFileDialogListViewItem.DisplayText = Path.GetFileName(filePath);
                        SelectedFilesListView.Items.Add(addFileDialogListViewItem);

                        // Get the thumbnail of the file using Win32 APIs
                        IntPtr hIcon = GetJumboIcon(GetIconIndex(filePath));
                        System.Drawing.Icon ico = (System.Drawing.Icon)System.Drawing.Icon.FromHandle(hIcon).Clone();
                        SoftwareBitmapSource fileIcon = await GetWinUI3BitmapSourceFromIcon(ico);
                        addFileDialogListViewItem.FileIcon = fileIcon;

                        // Clean up
                        Shell32.DestroyIcon(hIcon);
                    }
                    else
                    {
                        // We can use WASDK APIs to continue
                        // Create a StorageFile
                        StorageFile file = await StorageFile.GetFileFromPathAsync(filePath);

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
