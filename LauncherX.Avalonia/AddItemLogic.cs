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

namespace LauncherX.Avalonia
{
    public static class AddItemLogic
    {
        public static void AddWebsite(string url, GridView gridView)
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
                    filename = websitecount.ToString() + filename;
                    //download the website's favicon
                    webClient.DownloadFile(new Uri(downloadaddress), System.IO.Path.Combine(websiteIconDir, filename));
                    websitecount += 1;
                }
                catch
                {
                    //show error message
                    System.Windows.MessageBox.Show("Unable to get website icon. Please check that the website is valid and that you are connected to the internet. LauncherX will still add the website, just without the icon." +
                        "If you want to have the icon, remove the website from LauncherX and re-add the website or restart Launcher X when you have internet connection.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }


            }
            else
            {
                try
                {
                    //try to download file
                    //download the website's favicon
                    webClient.DownloadFile(new Uri(downloadaddress), System.IO.Path.Combine(websiteIconDir, filename));
                }
                catch
                {
                    //show error message
                    System.Windows.MessageBox.Show("Unable to get website icon. Please check that the website is valid and that you are connected to the internet. LauncherX will still add the website, just without the icon." +
                        "If you want to have the icon, remove the website from LauncherX and re-add the website or restart Launcher X when you have internet connection.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }




            //create a stackpanel
            Windows.UI.Xaml.Controls.StackPanel stackpanel = new Windows.UI.Xaml.Controls.StackPanel();
            stackpanel.Width = size * 105;
            stackpanel.Height = size * 90;
            //for some reason, it needs to have a background in order for right tap to work??
            stackpanel.Background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(0, 0, 0, 0));

            //load file icon into uwp image control
            Windows.UI.Xaml.Controls.Image image = new Windows.UI.Xaml.Controls.Image();
            string path = Path.Combine(websiteIconDir + filename);
            Uri fileuri = new Uri(path);
            image.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(fileuri);
            image.Stretch = Windows.UI.Xaml.Media.Stretch.Uniform;
            image.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center;
            image.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center;
            image.Margin = new Windows.UI.Xaml.Thickness(size * 22.5, 5, size * 22.5, 0);
            image.Height = stackpanel.Width - size * 22.5 - size * 22.5;
            image.Width = stackpanel.Width - size * 22.5 - size * 22.5;

            //create a textblock
            Windows.UI.Xaml.Controls.TextBlock textblock = new Windows.UI.Xaml.Controls.TextBlock();
            textblock.TextAlignment = Windows.UI.Xaml.TextAlignment.Center;
            textblock.Text = original_url;
            textblock.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center;
            textblock.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Bottom;
            textblock.FontSize = size * 11;
            textblock.Margin = new Windows.UI.Xaml.Thickness(5);
            textblock.TextTrimming = Windows.UI.Xaml.TextTrimming.CharacterEllipsis;

            //save the path in stackpanel tag
            stackpanel.Tag = "https://" + url;

            //init a tooltip
            Windows.UI.Xaml.Controls.ToolTip toolTip = new Windows.UI.Xaml.Controls.ToolTip();
            toolTip.Content = original_url;

            //set the tooltipowner to the stackpanel using tooltip service
            Windows.UI.Xaml.Controls.ToolTipService.SetToolTip(stackpanel, toolTip);

            //righttapped event handler for menu flyout
            stackpanel.RightTapped += Stackpanel_RightTapped2;
            stackpanel.PointerPressed += Stackpanel_PointerPressed2;

            //add the controls
            stackpanel.Children.Add(image);
            stackpanel.Children.Add(textblock);
            gridView.Items.Add(stackpanel);


        }
    }
