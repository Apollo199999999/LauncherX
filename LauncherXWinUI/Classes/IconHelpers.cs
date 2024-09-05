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
using Microsoft.Graphics.Display;
using WinUIEx;
using System.Runtime.CompilerServices;

namespace LauncherXWinUI.Classes
{
    /// <summary>
    /// Class to get the icon of a file, folder, or website
    /// </summary>
    public static class IconHelpers
    {
        // Get a list of all image file extensions
        public static List<string> ImageFileExtensions = ImageCodecInfo.GetImageEncoders()
                                                            .Select(c => c.FilenameExtension)
                                                            .SelectMany(e => e.Split(';'))
                                                            .Select(e => e.Replace("*", "").ToLower())
                                                            .ToList();

        // The following 2 functions are modified from WinLaunch
        // The problem is this - when a file does not have a 256x256 icon,
        // WinAPI just retrieves the 48x48 icon and draws it in the top left corner, leading to this bug: https://github.com/Apollo199999999/LauncherX/issues/14

        /// <summary>
        /// Resuzes a Jumbo (256x256) icon with a 48x48 icon on the top left to the specified size, for files that do not have a 256x256 icon
        /// </summary>
        /// <param name="imgToResize">Jumbo icon</param>
        /// <param name="size">Final size to resize it to</param>
        /// <returns>System.Drawing.Bitmap</returns>
        private static System.Drawing.Bitmap ResizeJumbo(System.Drawing.Bitmap imgToResize, System.Drawing.Size size)
        {
            System.Drawing.Bitmap b = new System.Drawing.Bitmap(size.Width, size.Height);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage((System.Drawing.Image)b);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

            int sourceWidth = (int)(48 * Shell32.GetDPI(App.MainWindow) / 96);
            int sourceHeight = (int)(48 * Shell32.GetDPI(App.MainWindow) / 96);

            double WidthScale = ((double)size.Width / (double)sourceWidth);
            double HeightScale = ((double)size.Height / (double)sourceHeight);

            g.DrawImage(imgToResize, 0, 0, (int)(256 * WidthScale), (int)(256 * HeightScale));
            g.Dispose();

            return b;
        }

        /// <summary>
        /// Check whether a Jumbo icon is just a 48x48 icon at the top left 
        /// </summary>
        /// <param name="bitmap">Jumbo icon</param>
        /// <returns>Boolean telling you whether a Jumbo icon just contains a 48x48 icon at the top left</returns>
        private static bool IsScaledDown(System.Drawing.Bitmap bitmap)
        {
            System.Drawing.Color empty = System.Drawing.Color.FromArgb(0, 0, 0, 0);

            if (bitmap != null)
            {
                if (bitmap.Width <= 48)
                    return false;

                int checks = 5;
                double SmallImageSize = 48.0 * Shell32.GetDPI(App.MainWindow) / 96 + 1;
                double CheckDistance = (bitmap.Width - SmallImageSize) / (double)(checks + 1);

                for (int x = 0; x < checks + 1; x++)
                {
                    for (int y = 0; y < checks + 1; y++)
                    {
                        int xpos = (int)(SmallImageSize + (double)x * CheckDistance);
                        int ypos = (int)(SmallImageSize + (double)y * CheckDistance);
                        try
                        {
                            if (bitmap.GetPixel(xpos, ypos) != empty)
                            {
                                //not an empty pixel
                                return false;
                            }
                        }
                        catch { }
                    }
                }
            }

            return true;
        }

        // THANK GOD FOR STACKOVERFLOW: https://stackoverflow.com/questions/76640972/convert-system-drawing-icon-to-microsoft-ui-xaml-imagesource
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
            System.Drawing.Bitmap bitmapIcon = ico.ToBitmap();
            ico.Dispose();

            // For some files, the 256x256 icon may not be available, and we need to account for that to prevent this bug: https://github.com/Apollo199999999/LauncherX/issues/14
            if (IsScaledDown(bitmapIcon))
            {
                bitmapIcon = ResizeJumbo(bitmapIcon, new System.Drawing.Size(200, 200));
            }

            // Convert Bitmap to BitmapImage
            BitmapImage fileIcon = await GetWinUI3BitmapSourceFromGdiBitmap(bitmapIcon);

            // Clean up
            Shell32.DestroyIcon(hIcon);
            bitmapIcon.Dispose();

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
            else if (ImageFileExtensions.Contains(ext))
            {
                // Use WinRT methods to get the thumbnail of the image
                StorageFile storageFile = await StorageFile.GetFileFromPathAsync(filePath);
                BitmapImage fileIcon = await GetFileIconWinRT(storageFile);
                return fileIcon;
            }
            else
            {
                // Use Win32 methods
                BitmapImage fileIcon = await GetFileIconWin32(filePath);
                return fileIcon;
            }
        }
    }
}
