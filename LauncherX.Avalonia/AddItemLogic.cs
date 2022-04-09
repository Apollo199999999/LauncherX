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
using Avalonia.Input;
using MenuFlyout = Avalonia.Controls.MenuFlyout;

namespace LauncherX.Avalonia
{
    public static class AddItemLogic
    {

        //these variables are used for duplicate naming
        public static int foldercount = 0;
        public static int count = 0;
        public static int WebsiteCount = 0;

        //functions for creating gridviewtiles
        public static StackPanel CreateWebsiteTile(string url, string filename, double size)
        {
            //create a stackpanel
            StackPanel stackPanel = new StackPanel();
            stackPanel.Width = size * 105;
            stackPanel.Height = size * 90;
            //for some reason, it needs to have a background in order for the stackpanel to be clickable when there's no favicon??
            stackPanel.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

            //load file icon into image control
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
            image.Height = stackPanel.Width - size * 22.5 - size * 22.5;
            image.Width = stackPanel.Width - size * 22.5 - size * 22.5;

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
                stackPanel.Tag = "https://" + url;
            }
            else
            {
                stackPanel.Tag = url;
            }

            //attach a tooltip to the stackpanel
            ToolTip.SetTip(stackPanel, url);

            //add the controls
            stackPanel.Children.Add(image);
            stackPanel.Children.Add(textblock);
            stackPanel.PointerReleased += Item_PointerReleased;

            return stackPanel;
        }

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
            //TODO: FIND AN OPTIMAL WEBSITE TO GET FAVICONS
            string downloadaddress = "";

            if (url.StartsWith("http://") == false && url.StartsWith("https://") == false)
            {
                downloadaddress = "https://standard-amber-wombat.faviconkit.com/" + url + "/256";
            }
            else if (url.StartsWith("https://"))
            {
                string faviconurl = url.Replace("https://", "");
                downloadaddress = "https://standard-amber-wombat.faviconkit.com/" + faviconurl + "/256";
            }
            else if (url.StartsWith("http://"))
            {
                string faviconurl = url.Replace("http://", "");
                downloadaddress = "https://standard-amber-wombat.faviconkit.com/" + faviconurl + "/256";
            }


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

            //create a websitetile and add it to the ItemsListBox
            List<StackPanel> ItemsListBoxItems = new List<StackPanel>();
            foreach (StackPanel AllItemsStack in PV_MainWindow.ItemsListBox.Items)
            {
                ItemsListBoxItems.Add(AllItemsStack);
            }
            ItemsListBoxItems.Add(CreateWebsiteTile(url, filename, size));
            PV_MainWindow.ItemsListBox.Items = ItemsListBoxItems;
        }



        //event handlers
        private static void Item_PointerReleased(object? sender, global::Avalonia.Input.PointerReleasedEventArgs e)
        {
            //cast the sender as a stackpanel
            StackPanel stackPanel = sender as StackPanel;

            //check if it is a left click or right click
            if (e.InitialPressMouseButton == MouseButton.Left)
            {
                //check if the item is a websiteitem, fileitem, or folderitem
                if (stackPanel.Tag.ToString().StartsWith("https://") || stackPanel.Tag.ToString().StartsWith("http://"))
                {
                    //the stackpanel is a website tile, start the website
                    PV_OpenBrowser(stackPanel.Tag.ToString());
                }
                else
                {
                    //the stackpanel is a file/folder tile
                }
            }

        }
    }
}