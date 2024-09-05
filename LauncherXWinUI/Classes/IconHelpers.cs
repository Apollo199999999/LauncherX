using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Microsoft.UI.Xaml.Media;
using Windows.UI.Popups;
using System.Drawing;
using System.Drawing.Imaging;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace LauncherXWinUI.Classes
{
    /// <summary>
    /// Class to get the icon of a file, folder, or website
    /// </summary>
    public static class IconHelpers
    {
        // THANK GOD FOR STACKOVERFLOW: https://stackoverflow.com/questions/76640972/convert-system-drawing-icon-to-microsoft-ui-xaml-imagesource
        /// <summary>
        /// Converts System.Drawing.Icon to SoftwareBitmapSource
        /// </summary>
        /// <param name="icon">Icon to convert</param>
        /// <returns>BitmapImage for Image Control</returns>
        private static async Task<BitmapImage> GetWinUI3BitmapSourceFromIcon(System.Drawing.Icon icon)
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
        /// <returns>BitmapImage for Image Control</returns>
        private static async Task<BitmapImage> GetWinUI3BitmapSourceFromGdiBitmap(System.Drawing.Bitmap bmp)
        {
            if (bmp == null)
                return null;

            // get pixels as an array of bytes
            var data = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);
            var bytes = new byte[data.Stride * data.Height];
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);
            bmp.UnlockBits(data);

            // Create WriteableBitmap from byte array
            WriteableBitmap writableBitmap = new WriteableBitmap(bmp.Width, bmp.Height);
            await writableBitmap.PixelBuffer.AsStream().WriteAsync(bytes, 0, bytes.Length);

            // Convert WriteableBitmap to BitmapImage
            InMemoryRandomAccessStream inMemoryRandomAccessStream = new InMemoryRandomAccessStream();
            BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, inMemoryRandomAccessStream);
            Stream pixelStream = writableBitmap.PixelBuffer.AsStream();
            byte[] pixels = new byte[pixelStream.Length];
            await pixelStream.ReadAsync(pixels, 0, pixels.Length);
            encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight, (uint)writableBitmap.PixelWidth, (uint)writableBitmap.PixelHeight, 96.0, 96.0, pixels);
            await encoder.FlushAsync();
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.SetSource(inMemoryRandomAccessStream);

            return bitmapImage;
        }

        /// <summary>
        /// Method to get the icon of a website
        /// </summary>
        /// <param name="websiteUrl">URL of the website</param>
        /// <returns></returns>
        public static BitmapImage GetWebsiteIcon(string websiteUrl)
        {
            // Get the icon of the website, using Google's favicon service
            BitmapImage websiteIcon = new BitmapImage();

            // Fallback icon
            Uri defaultImageUri = new Uri(Path.GetFullPath(@"Resources\websitePlaceholder.png"), UriKind.Absolute);
            websiteIcon.ImageFailed += (s, e) =>
            {
                websiteIcon.UriSource = defaultImageUri;
            };
            // Try getting website icon
            Uri iconUri = new Uri("https://www.google.com/s2/favicons?sz=128&domain_url=" + websiteUrl, UriKind.Absolute);
            websiteIcon.UriSource = iconUri;

            return websiteIcon;
        }

        /// <summary>
        /// Method to get the icon of a folder
        /// </summary>
        /// <param name="storageFolder">StorageFolder object storing the folder</param>
        /// <returns></returns>
        public async static Task<BitmapImage> GetFolderIcon(StorageFolder storageFolder)
        {
            // Get the thumbnail of the folder
            StorageItemThumbnail thumbnail = await storageFolder.GetThumbnailAsync(ThumbnailMode.SingleItem, 256);
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.SetSource(thumbnail);

            return bitmapImage;
        }


        /// <summary>
        /// Method to get the icon of a file, using WinRT/WASDK APIs
        /// </summary>
        /// <param name="storageFile">StorageFile object storing the file</param>
        /// <returns></returns>
        private async static Task<BitmapImage> GetFileIconWinRT(StorageFile storageFile)
        {
            // Get the thumbnail of the folder
            StorageItemThumbnail thumbnail = await storageFile.GetThumbnailAsync(ThumbnailMode.SingleItem, 256);
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.SetSource(thumbnail);

            return bitmapImage;
        }

        /// <summary>
        /// Method to get the icon of a file, using Win32 methods
        /// </summary>
        /// <param name="filePath">Path to file</param>
        private async static Task<BitmapImage> GetFileIconWin32(string filePath)
        {
            IntPtr hIcon = Shell32.GetJumboIcon(Shell32.GetIconIndex(filePath));
            System.Drawing.Icon ico = (System.Drawing.Icon)System.Drawing.Icon.FromHandle(hIcon).Clone();
            BitmapImage fileIcon = await GetWinUI3BitmapSourceFromIcon(ico);

            // Clean up
            Shell32.DestroyIcon(hIcon);

            return fileIcon;
        }

        /// <summary>
        /// One unified method that works with all file types to get the icon of a file
        /// </summary>
        /// <returns>BitmapImage that can be used directly with an image control</returns>
        public async static Task<BitmapImage> GetFileIcon(string filePath)
        {
            // Get the extension of the file
            string ext = Path.GetExtension(filePath);

            // StorageFile is not compatible with files of extension .lnk, .wsh, or .url, thus if our file has those extensions, we must use Win32 methods to retrieve the file icon
            if (ext == ".lnk" || ext == ".url" || ext == ".wsh")
            {
                BitmapImage fileIcon = await GetFileIconWin32(filePath);
                return fileIcon;
            }
            else
            {
                // Try to use WinRT methods, which might still fail if the item is hidden, so wrap in try-catch
                try
                {
                    StorageFile storageFile = await StorageFile.GetFileFromPathAsync(filePath);
                    BitmapImage fileIcon = await GetFileIconWinRT(storageFile);
                    return fileIcon;
                }
                catch
                {
                    BitmapImage fileIcon = await GetFileIconWin32(filePath);
                    return fileIcon;
                }
            }
        }
    }
}
