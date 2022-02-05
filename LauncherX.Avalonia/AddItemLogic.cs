using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using FluentAvalonia.Styling;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Navigation;
using LauncherX.Avalonia.Pages;
using Button = Avalonia.Controls.Button;
using static LauncherX.Avalonia.PublicVariables;
using Avalonia.Controls.ApplicationLifetimes;
using System.Runtime.InteropServices;
using System.Net;
using System.IO;
using Avalonia.Media.Imaging;
using Avalonia.Layout;
using System.Collections.Generic;

namespace LauncherX.Avalonia
{
    public static class AddItemLogic
    {
        //these variables are used for duplicate naming
        public static int foldercount = 0;
        public static int count = 0;
        public static int WebsiteCount = 0;

        public async static void AddWebsite(string url, double size)
        {
            //init filename and remove everything after .com
            var filename = url;

            //remove all illegal characters from filename
            filename = filename.Replace("/", "");
            filename = filename.Replace(@"\", "");
            filename = filename.Replace(":", "");
            filename = filename.Replace("*", "");
            filename = filename.Replace("?", "");
            filename = filename.Replace("\"", "");
            filename = filename.Replace("<", "");
            filename = filename.Replace(">", "");
            filename = filename.Replace("|", "");
            filename = filename.Replace(".", "");

            //add the .tiff extension
            filename = filename + ".tiff";


            //download address. The link is used to grab the favicon
            string downloadaddress = "https://www.google.com/s2/favicons?sz=64&domain_url=" + url;

            //init a new webclient
            WebClient webClient = new WebClient();

            if (File.Exists(Path.Combine(PV_WebsiteIconDir, filename)))
            {
                try
                {
                    //try to download file
                    filename = WebsiteCount.ToString() + filename;
                    //download the website's favicon
                    webClient.DownloadFile(new Uri(downloadaddress), System.IO.Path.Combine(PV_WebsiteIconDir, filename));
                    WebsiteCount += 1;
                }
                catch
                {
                    //show error message (contentdialog)
                    ContentDialog dialog = new ContentDialog();
                    dialog.Title = "Unable to get website icon";
                    dialog.CloseButtonText = " OK ";
                    dialog.DefaultButton = ContentDialogButton.Close;
                    dialog.Content = "Please check that the website is valid and that you are connected to the internet. LauncherX will still add the website, just without the icon.";

                    var result = await dialog.ShowAsync();
                }


            }
            else
            {
                try
                {
                    //try to download file
                    //download the website's favicon
                    webClient.DownloadFile(new Uri(downloadaddress), System.IO.Path.Combine(PV_WebsiteIconDir, filename));
                }
                catch
                {
                    //show error message (contentdialog)
                    ContentDialog dialog = new ContentDialog();
                    dialog.Title = "Unable to get website icon";
                    dialog.CloseButtonText = " OK ";
                    dialog.DefaultButton = ContentDialogButton.Close;
                    dialog.Content = "Please check that the website is valid and that you are connected to the internet. LauncherX will still add the website, just without the icon.";

                    var result = await dialog.ShowAsync();
                }
            }

            //create a stackpanel
            StackPanel stackpanel = new StackPanel();
            stackpanel.Width = size * 105;
            stackpanel.Height = size * 90;

            //load file icon into uwp image control
            Image image = new Image();
            string path = Path.Combine(PV_WebsiteIconDir, filename);
            try
            {
                image.Source = new Bitmap(path);
            }
            catch { }
            image.Stretch = Stretch.Uniform;
            image.VerticalAlignment = VerticalAlignment.Center;
            image.HorizontalAlignment = HorizontalAlignment.Center;
            image.Margin = new Thickness(size * 22.5, 5, size * 22.5, 0);
            image.Height = stackpanel.Width - size * 22.5 - size * 22.5;
            image.Width = stackpanel.Width - size * 22.5 - size * 22.5;

            //create a textblock
            TextBlock textblock = new TextBlock();
            textblock.TextAlignment = TextAlignment.Center;
            textblock.Text = url;
            textblock.HorizontalAlignment = HorizontalAlignment.Center;
            textblock.VerticalAlignment = VerticalAlignment.Bottom;
            textblock.FontSize = size * 11;
            textblock.Margin = new Thickness(5);
            textblock.TextTrimming = TextTrimming.CharacterEllipsis;

            //save the path in stackpanel tag
            if (url.StartsWith("http://") == false && url.StartsWith("https://") == false)
            {
                stackpanel.Tag = "https://" + url;
            }

            //add the controls
            stackpanel.Children.Add(image);
            stackpanel.Children.Add(textblock);

            List<StackPanel> WebsitesGridViewItems = new List<StackPanel>();
            foreach (StackPanel WebsitesStack in PV_MainWindow.WebsitesGridView.Items)
            {
                WebsitesGridViewItems.Add(WebsitesStack);
            }
            WebsitesGridViewItems.Add(stackpanel);
            PV_MainWindow.WebsitesGridView.Items = WebsitesGridViewItems;

            List<StackPanel> AllItemsGridViewItems = new List<StackPanel>();
            foreach (StackPanel AllItemsStack in PV_MainWindow.AllItemsGridView.Items)
            {
                AllItemsGridViewItems.Add(AllItemsStack);
            }
            AllItemsGridViewItems.Add(stackpanel);
            PV_MainWindow.AllItemsGridView.Items = AllItemsGridViewItems;
        }
    }
}
