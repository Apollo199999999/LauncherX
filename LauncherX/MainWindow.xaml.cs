using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Windows.UI.Xaml.Controls;
using static LauncherX.PublicVariables;
using Microsoft.WindowsAPICodePack.Dialogs;
using static System.Net.WebRequestMethods;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Controls;
using Windows.Devices.PointOfService;
using File = System.IO.File;

namespace LauncherX
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow
    {
        #region Code related to calling of Win32 APIs
        //Struct used by SHGetFileInfo function
        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };

        //Constants flags for SHGetFileInfo 
        public const uint SHGFI_ICON = 0x100;
        public const uint SHGFI_LARGEICON = 0x0; // 'Large icon

        //Import SHGetFileInfo function
        [DllImport("shell32.dll")]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

        #endregion

        #region Global variables

        //size variable to control size of icons
        public double size;

        //these variables are used for duplicate naming
        public int FolderNameCount = 0;
        public int FileNameCount = 0;
        public int WebsiteNameCount = 0;

        //this variable is used to save the items in the gridview, based on order
        public int savename = 1;

        //AddItemsErrorDialog, in case when loading the files after application startup, the file does not exist
        AddItemsErrorDialog AddItemsErrorDialog = new AddItemsErrorDialog();

        //create a new dispatcher timer
        DispatcherTimer themeupdater = new DispatcherTimer();

        //init directory strings
        public string loadDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\LauncherX\\Files\\";
        public string appIconDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\LauncherX\\Temp\\AppIcons\\";
        public string folderIconDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\LauncherX\\Temp\\FolderIcons\\";
        public string websiteIconDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\LauncherX\\Temp\\WebsiteIcons\\";

        #endregion

        public MainWindow()
        {
            InitializeComponent();

            //upgrade and reload settings
            Properties.Settings.Default.Upgrade();
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();

            //set size value
            size = Properties.Settings.Default.scale;
            scale = Properties.Settings.Default.scale;

            //set header text
            header.Text = Properties.Settings.Default.headerText;

            //create directories
            Directory.CreateDirectory(loadDir);
            Directory.CreateDirectory(appIconDir);
            Directory.CreateDirectory(folderIconDir);
            Directory.CreateDirectory(websiteIconDir);

            //delete files in directory (appicons)
            System.IO.DirectoryInfo di = new DirectoryInfo(appIconDir);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }

            //delete files in directory (foldericons)
            System.IO.DirectoryInfo di2 = new DirectoryInfo(folderIconDir);

            foreach (FileInfo file1 in di2.GetFiles())
            {
                file1.Delete();
            }

            //delete files in directory (websiteicons)
            System.IO.DirectoryInfo di3 = new DirectoryInfo(websiteIconDir);

            foreach (FileInfo file2 in di3.GetFiles())
            {
                file2.Delete();
            }

            //check and update theme
            CheckAndUpdateTheme();

            //event handlers
            window.Loaded += Window_Loaded;

            //set updaterequired to false
            updaterequired = false;

            //create a dispatcher timer to check for theme        
            themeupdater.Interval = TimeSpan.FromMilliseconds(100);
            themeupdater.Tick += Themeupdater_Tick;
            themeupdater.Start();

        }

        #region Methods relating to appearance of MainWindow
        //function to check and update theme
        public void CheckAndUpdateTheme()
        {
            //next, check if the system is in light or dark theme
            bool is_light_mode = true;
            try
            {
                var v = Microsoft.Win32.Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", "1");
                if (v != null && v.ToString() == "0")
                    is_light_mode = false;
            }
            catch { }


            if (is_light_mode == true)
            {
                Wpf.Ui.Appearance.Theme.Apply(
                    Wpf.Ui.Appearance.ThemeType.Light,     // Theme type
                    Wpf.Ui.Appearance.BackgroundType.Mica, // Background type
                    true                                   // Whether to change accents automatically
                );

                //change the background of the gridview section
                var gridView = gridviewhost.Child as Windows.UI.Xaml.Controls.GridView;
                if (gridView != null)
                {
                    Windows.UI.Xaml.Media.SolidColorBrush background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(255, 243, 243, 243));
                    gridView.Background = background;
                }
                //GridViewBackground.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 243, 243, 243));
            }
            else if (is_light_mode == false)
            {
                Wpf.Ui.Appearance.Theme.Apply(
                  Wpf.Ui.Appearance.ThemeType.Dark,      // Theme type
                  Wpf.Ui.Appearance.BackgroundType.Mica, // Background type
                  true                                   // Whether to change accents automatically
                );

                //change the background of the gridview section
                var gridView = gridviewhost.Child as Windows.UI.Xaml.Controls.GridView;
                if (gridView != null)
                {
                    Windows.UI.Xaml.Media.SolidColorBrush background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(255, 32, 32, 32));
                    gridView.Background = background;
                }
                ///GridViewBackground.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 32, 32, 32));

            }

        }

        private void Themeupdater_Tick(object sender, EventArgs e)
        {
            //check and update theme
            CheckAndUpdateTheme();
        }

        #endregion

        #region Loading settings and checking for updates when LauncherX starts
        public bool IsDirectoryEmpty(string path)
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //wait a while for the controls to load
            await Task.Delay(500);

            //init gridView
            var gridView = gridviewhost.Child as Windows.UI.Xaml.Controls.GridView;

            //create a variable to check if there are items that LauncherX cannot add
            bool ErrorAddingItems = false;

            //create a new list and sort the files
            var list = Directory.GetFiles(loadDir);
            Array.Sort(list, new AlphanumComparatorFast());

            //foreach loop to iterate through files and call the appropriate functions
            foreach (string file in list)
            {
                //load the text files (read)

                // Open the file to read from.
                using (StreamReader sr = File.OpenText(file))
                {
                    string s = "";

                    //read the first line of the text document
                    s = sr.ReadLine();

                    if (s.StartsWith("https://"))
                    {
                        //this is a website.
                        //remove https://
                        s = s.Replace("https://", "");
                        original_url = sr.ReadLine();
                        AddWebsite(s, original_url);

                    }
                    else
                    {
                        /*this part, it is either a file or folder. hence, if it doesn't exist, 
                         * we need to add it to a listview in a dialog window, to tell the user that the file/folder 
                         * cannot be added*/

                        //wrap everything in a try catch loop.
                        try
                        {
                            //create a new fileatttributes from s. This will be used to check if the text in the text file is a directory or file.
                            System.IO.FileAttributes attr = File.GetAttributes(s);

                            if (attr.HasFlag(System.IO.FileAttributes.Directory))
                            {
                                //this is a directory (folder)
                                AddFolder(s);
                            }
                            else if (!attr.HasFlag(System.IO.FileAttributes.Directory))
                            {
                                //this is a file
                                AddFile(s);
                            }
                        }
                        catch
                        {
                            //add the problematic directory ti the listview in the error dialog, then set erroraddingitems to true
                            AddItemsErrorDialog.ErrorFiles.Items.Add(s);
                            ErrorAddingItems = true;
                        }

                    }

                }

            }

            //check if gridView is empty
            if (gridView.Items.Count == 0)
            {
                gridviewhost.Visibility = Visibility.Hidden;
                empty.BringIntoView();
            }
            else
            {
                gridviewhost.Visibility = Visibility.Visible;
            }

            if (ErrorAddingItems == true)
            {
                //show the additemserrordialog
                AddItemsErrorDialog.ShowDialog();
            }

            /*check if internet connection exists, and if so, check for updates, and if there are updates,
           show a messagebox to ask if the user wants to update*/

            if (CheckForInternetConnection() == true)
            {
                CheckForUpdates();

                if (updaterequired == true)
                {
                    //show the messagebox
                    var result = System.Windows.MessageBox.Show("An update is available, would you like to download it?", "Update available",
                        System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Information);

                    //check for the messagebox reply
                    if (result == MessageBoxResult.Yes)
                    {
                        //get the download update link from the pastebin link
                        //update link is a pastebin link
                        var url = "https://pastebin.com/raw/M0zxFc29";

                        //init a webclient
                        WebClient client = new WebClient();

                        //download all text from the pastebin raw link
                        string reply = client.DownloadString(url);

                        //set the updateLink
                        updateLink = reply;
                        //start the update link, stop all timers, and shutdown application
                        Process.Start(updateLink);

                        themeupdater.Stop();
                        System.Windows.Application.Current.Shutdown();
                    }
                }

            }

            //activate and focus this window
            this.Activate();
            this.Focus();
        }

        #endregion

        #region Methods relating to auto-update system

        //Check Internet Connection Function
        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var CheckInternet = new WebClient())
                using (CheckInternet.OpenRead("http://clients3.google.com/generate_204"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }


        public void CheckForUpdates()
        {
            //Check for updates

            //updtae link is a pastebin link
            var url = "https://pastebin.com/raw/PGcXcxnT";

            //init a webclient
            WebClient client = new WebClient();

            //download all text from the pastebin raw link
            string reply = client.DownloadString(url);

            //check if version matches pastebin link
            if (reply != currentversion)
            {
                //this is the output when an update is available. Modify it if you wish

                //update the "updaterequired" variable from publicvariables to true, this will be used later in the about section
                updaterequired = true;
            }
            else
            {
                updaterequired = false;
            }
        }

        #endregion

        #region Methods relating to GridView Items

        #region Methods relating to creation of GridView Items
        public Windows.UI.Xaml.Controls.StackPanel CreateGridViewItem(string iconPath, string DisplayText, string StackPanelTag)
        {
            //create a stackpanel
            Windows.UI.Xaml.Controls.StackPanel stackpanel = new Windows.UI.Xaml.Controls.StackPanel();
            stackpanel.Width = size * 105;
            stackpanel.Height = size * 90;
            //for some reason, it needs to have a background in order for right tap to work??
            stackpanel.Background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(0, 0, 0, 0));

            //load file icon into uwp image control
            Windows.UI.Xaml.Controls.Image image = new Windows.UI.Xaml.Controls.Image();
            string path = iconPath;
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
            textblock.Text = DisplayText;
            textblock.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center;
            textblock.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Bottom;
            textblock.FontSize = size * 11;
            textblock.Margin = new Windows.UI.Xaml.Thickness(5);
            textblock.TextTrimming = Windows.UI.Xaml.TextTrimming.CharacterEllipsis;

            //save the path in stackpanel tag
            stackpanel.Tag = StackPanelTag;

            //init a tooltip
            Windows.UI.Xaml.Controls.ToolTip toolTip = new Windows.UI.Xaml.Controls.ToolTip();
            toolTip.Content = DisplayText;

            //set the tooltipowner to the stackpanel using tooltip service
            Windows.UI.Xaml.Controls.ToolTipService.SetToolTip(stackpanel, toolTip);

            //righttapped event handler for menu flyou
            stackpanel.PointerPressed += GridViewItem_PointerPressed;

            //add the controls
            stackpanel.Children.Add(image);
            stackpanel.Children.Add(textblock);

            return stackpanel;

        }


        public void AddFile(string myfile)
        {
            //init a gridview
            var gridView = gridviewhost.Child as Windows.UI.Xaml.Controls.GridView;

            //show gridView
            gridviewhost.Visibility = Visibility.Visible;

            //init FileIconName
            var FileIconName = System.IO.Path.GetFileNameWithoutExtension(myfile);
            FileIconName = FileIconName + ".tiff";

            bool isImageFile;
            try
            {
                System.Drawing.Image.FromFile(myfile).Dispose();
                isImageFile = true;
            }
            catch (OutOfMemoryException)
            {
                isImageFile = false;
            }

            //save file icon
            if (isImageFile == true)
            {
                if (File.Exists(Path.Combine(appIconDir, FileIconName)))
                {
                    FileIconName = FileNameCount.ToString() + FileIconName;
                    FileStream stream = new FileStream(System.IO.Path.Combine(appIconDir, FileIconName), FileMode.Create);
                    System.Drawing.Image image1 = System.Drawing.Image.FromFile(myfile);
                    System.Drawing.Image icon = image1.GetThumbnailImage(image1.Width, image1.Height, () => false, IntPtr.Zero);
                    icon.Save(stream, System.Drawing.Imaging.ImageFormat.Tiff);
                    FileNameCount += 1;

                }
                else
                {
                    FileStream stream = new FileStream(System.IO.Path.Combine(appIconDir, FileIconName), FileMode.Create);
                    System.Drawing.Image image1 = System.Drawing.Image.FromFile(myfile);
                    System.Drawing.Image icon = image1.GetThumbnailImage(image1.Width, image1.Height, () => false, IntPtr.Zero);
                    icon.Save(stream, System.Drawing.Imaging.ImageFormat.Tiff);
                }
            }
            else
            {
                if (File.Exists(Path.Combine(appIconDir, FileIconName)))
                {
                    FileIconName = FileNameCount.ToString() + FileIconName;
                    FileStream stream = new FileStream(System.IO.Path.Combine(appIconDir, FileIconName), FileMode.Create);
                    Bitmap icon = new Bitmap(System.Drawing.Icon.ExtractAssociatedIcon(myfile).ToBitmap());
                    icon.Save(stream, System.Drawing.Imaging.ImageFormat.Tiff);
                    FileNameCount += 1;

                }
                else
                {
                    FileStream stream = new FileStream(System.IO.Path.Combine(appIconDir, FileIconName), FileMode.Create);
                    Bitmap icon = new Bitmap(System.Drawing.Icon.ExtractAssociatedIcon(myfile).ToBitmap());
                    icon.Save(stream, System.Drawing.Imaging.ImageFormat.Tiff);
                }
            }

            gridView.Items.Add(CreateGridViewItem(Path.Combine(appIconDir + FileIconName), Path.GetFileName(myfile), myfile));


        }

        //add folder
        public void AddFolder(string directory)
        {
            //so, basic idea, copy the add_item and remove non essential functions

            //init a gridview
            var gridView = gridviewhost.Child as Windows.UI.Xaml.Controls.GridView;

            //show gridView
            gridviewhost.Visibility = Visibility.Visible;

            //init FolderIconName
            string FolderIconName = new DirectoryInfo(directory).Name;
            FolderIconName = FolderIconName + ".tiff";

            //now check if the icon of the folder exists in directory
            if (File.Exists(System.IO.Path.Combine(folderIconDir, FolderIconName)))
            {
                FolderIconName = FolderNameCount.ToString() + FolderIconName;

                //get the icon of the folder
                SHFILEINFO shinfo = new SHFILEINFO();
                SHGetFileInfo(directory, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), SHGFI_ICON | SHGFI_LARGEICON);
                Icon i = System.Drawing.Icon.FromHandle(shinfo.hIcon);

                //init filestream
                FileStream stream = new FileStream(System.IO.Path.Combine(folderIconDir, FolderIconName), FileMode.Create);
                Bitmap icon = i.ToBitmap();
                icon.Save(stream, System.Drawing.Imaging.ImageFormat.Tiff);

                FolderNameCount += 1;
            }
            else
            {
                //get the icon of the folder
                SHFILEINFO shinfo = new SHFILEINFO();
                SHGetFileInfo(directory, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), SHGFI_ICON | SHGFI_LARGEICON);
                Icon i = System.Drawing.Icon.FromHandle(shinfo.hIcon);

                //init filestream
                FileStream stream = new FileStream(System.IO.Path.Combine(folderIconDir, FolderIconName), FileMode.Create);
                Bitmap icon = i.ToBitmap();
                icon.Save(stream, System.Drawing.Imaging.ImageFormat.Tiff);
            }

            //then do the controls and add it to the gridView
            gridView.Items.Add(CreateGridViewItem(Path.Combine(folderIconDir + FolderIconName), new DirectoryInfo(directory).Name, directory));

        }

        private void AddWebsite(string url, string DisplayName)
        {
            //init a gridview
            var gridView = gridviewhost.Child as Windows.UI.Xaml.Controls.GridView;

            //show gridView
            gridviewhost.Visibility = Visibility.Visible;


            //init WebsiteIconFileName and remove everything after .com
            var WebsiteIconFileName = url;

            //remove all illegal characters from WebsiteIconFileName
            WebsiteIconFileName = WebsiteIconFileName.Replace("/", "");
            WebsiteIconFileName = WebsiteIconFileName.Replace(@"\", "");
            WebsiteIconFileName = WebsiteIconFileName.Replace(":", "");
            WebsiteIconFileName = WebsiteIconFileName.Replace("*", "");
            WebsiteIconFileName = WebsiteIconFileName.Replace("?", "");
            WebsiteIconFileName = WebsiteIconFileName.Replace("\"", "");
            WebsiteIconFileName = WebsiteIconFileName.Replace("<", "");
            WebsiteIconFileName = WebsiteIconFileName.Replace(">", "");
            WebsiteIconFileName = WebsiteIconFileName.Replace("|", "");
            WebsiteIconFileName = WebsiteIconFileName.Replace(".", "");

            //add the .tiff extension
            WebsiteIconFileName = WebsiteIconFileName + ".tiff";


            //download address. The link is used to grab the favicon
            string downloadaddress = "https://www.google.com/s2/favicons?sz=64&domain_url=https://" + url;

            //init a new webclient
            WebClient webClient = new WebClient();


            if (File.Exists(Path.Combine(websiteIconDir, WebsiteIconFileName)))
            {
                try
                {
                    //try to download file
                    WebsiteIconFileName = WebsiteNameCount.ToString() + WebsiteIconFileName;
                    //download the website's favicon
                    webClient.DownloadFile(new Uri(downloadaddress), System.IO.Path.Combine(websiteIconDir, WebsiteIconFileName));
                    WebsiteNameCount += 1;
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
                    webClient.DownloadFile(new Uri(downloadaddress), System.IO.Path.Combine(websiteIconDir, WebsiteIconFileName));
                }
                catch
                {
                    //show error message
                    System.Windows.MessageBox.Show("Unable to get website icon. Please check that the website is valid and that you are connected to the internet. LauncherX will still add the website, just without the icon." +
                        "If you want to have the icon, remove the website from LauncherX and re-add the website or restart Launcher X when you have internet connection.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            gridView.Items.Add(CreateGridViewItem(Path.Combine(websiteIconDir + WebsiteIconFileName), DisplayName, "https://" + url));
        }

        #endregion

        #region Methods relating to MenuFlyout and its event handlers
        public Windows.UI.Xaml.Controls.MenuFlyout CreateGridViewItemContextMenu(Windows.UI.Xaml.Controls.StackPanel stackPanel)
        {
            //init a new menu flyout
            Windows.UI.Xaml.Controls.MenuFlyout menu = new Windows.UI.Xaml.Controls.MenuFlyout();

            //init menu flyout items
            Windows.UI.Xaml.Controls.MenuFlyoutItem open = new Windows.UI.Xaml.Controls.MenuFlyoutItem();
            Windows.UI.Xaml.Controls.MenuFlyoutItem openfilelocation = new Windows.UI.Xaml.Controls.MenuFlyoutItem();
            Windows.UI.Xaml.Controls.MenuFlyoutItem remove = new Windows.UI.Xaml.Controls.MenuFlyoutItem();

            //event handlers for the items
            open.Click += (s, args) => MenuItemOpen_Click(s, args, stackPanel.Tag.ToString());
            remove.Click += (s, args) => MenuItemRemove_Click(s, args, stackPanel);
            openfilelocation.Click += (s, args) => MenuItemOpenLocation_Click(s, args, stackPanel.Tag.ToString());

            //next, we need to determine if the stackpanel is a file, folder, or website item.
            if (stackPanel.Tag.ToString().StartsWith("https://"))
            {

                //set properties and icons for the menuitems
                open.Icon = new SymbolIcon(Symbol.OpenFile);
                open.Text = "Open website";

                remove.Icon = new SymbolIcon(Symbol.Delete);
                remove.Text = "Remove item from LauncherX";

                //this is a website
                //add them to the menuflyout
                menu.Items.Add(open);
                menu.Items.Add(remove);
            }
            else
            {
                //either file or folder
                System.IO.FileAttributes attr = File.GetAttributes(stackPanel.Tag.ToString());


                if (attr.HasFlag(System.IO.FileAttributes.Directory))
                {
                    //this is a directory (folder)
                    //create a new FontIcon
                    Windows.UI.Xaml.Controls.FontIcon fontIcon = new Windows.UI.Xaml.Controls.FontIcon();
                    fontIcon.FontFamily = new Windows.UI.Xaml.Media.FontFamily("Segoe MDL2 Assets");
                    fontIcon.Glyph = "\xE838";

                    //set properties and icons for the menuitems
                    open.Icon = new SymbolIcon(Symbol.OpenFile);
                    open.Text = "Open";

                    openfilelocation.Icon = fontIcon;
                    openfilelocation.Text = "Open folder location";

                    remove.Icon = new SymbolIcon(Symbol.Delete);
                    remove.Text = "Remove item from LauncherX";

                    //add them to the menuflyout
                    menu.Items.Add(open);
                    menu.Items.Add(openfilelocation);
                    menu.Items.Add(remove);

                }
                else if (!attr.HasFlag(System.IO.FileAttributes.Directory))
                {
                    //this is a file
                    //create a new FontIcon
                    Windows.UI.Xaml.Controls.FontIcon fontIcon = new Windows.UI.Xaml.Controls.FontIcon();
                    fontIcon.FontFamily = new Windows.UI.Xaml.Media.FontFamily("Segoe MDL2 Assets");
                    fontIcon.Glyph = "\xE838";

                    //set properties and icons for the menuitems
                    open.Icon = new SymbolIcon(Symbol.OpenFile);
                    open.Text = "Open";

                    openfilelocation.Icon = fontIcon;
                    openfilelocation.Text = "Open file location";

                    remove.Icon = new SymbolIcon(Symbol.Delete);
                    remove.Text = "Remove item from LauncherX";

                    //add them to the menuflyout
                    menu.Items.Add(open);
                    menu.Items.Add(openfilelocation);
                    menu.Items.Add(remove);

                }
            }

            return menu;
        }
        private void GridViewItem_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            //init a stackpanel from sender
            Windows.UI.Xaml.Controls.StackPanel stackPanel = sender as Windows.UI.Xaml.Controls.StackPanel;

            // Check for input device
            if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse)
            {
                var properties = e.GetCurrentPoint(stackPanel).Properties;

                if (properties.IsRightButtonPressed)
                {
                    MenuFlyout menu = CreateGridViewItemContextMenu(stackPanel);

                    //show the menuflyout
                    menu.ShowAt(sender as Windows.UI.Xaml.UIElement, e.GetCurrentPoint(stackPanel).Position);
                }
            }
        }

        private void MenuItemOpen_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e, string ItemToOpen)
        {
            try
            {
                //start the process
                Process.Start(ItemToOpen);
            }
            catch { }
        }

        private void MenuItemRemove_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e, Windows.UI.Xaml.Controls.StackPanel stackPanel)
        {
            //init gridView
            var gridView = gridviewhost.Child as Windows.UI.Xaml.Controls.GridView;

            //remove the item
            gridView.Items.Remove(stackPanel);

            //check if gridView is empty
            if (gridView.Items.Count == 0)
            {
                gridviewhost.Visibility = Visibility.Hidden;
                empty.BringIntoView();
            }
            else
            {
                gridviewhost.Visibility = Visibility.Visible;
            }

        }

        private void MenuItemOpenLocation_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e, string ItemToLook)
        {
            //open file location
            Process.Start("explorer.exe", "/select, " + ItemToLook);
        }

        #endregion

        #endregion

        #region Methods relating to XAML Islands GridView
        private void gridviewhost_ChildChanged(object sender, EventArgs e)
        {
            //init a gridview
            var gridView = gridviewhost.Child as Windows.UI.Xaml.Controls.GridView;

            //declare properties
            gridView.CanDragItems = true;
            gridView.CanReorderItems = true;
            gridView.IsItemClickEnabled = true;
            gridView.AllowDrop = true;
            gridView.SelectionMode = Windows.UI.Xaml.Controls.ListViewSelectionMode.Single;

            //event handlers
            gridView.ItemClick += GridView_ItemClick;
            gridviewhost.ChildChanged -= gridviewhost_ChildChanged;
        }

        private async void GridView_ItemClick(object sender, Windows.UI.Xaml.Controls.ItemClickEventArgs e)
        {
            //init a gridView
            var gridView = gridviewhost.Child as Windows.UI.Xaml.Controls.GridView;

            //init a stackpanel from the clicked item
            Windows.UI.Xaml.Controls.StackPanel stackPanel = e.ClickedItem as Windows.UI.Xaml.Controls.StackPanel;

            try
            {
                //start the process that us related to the tag
                Process.Start(stackPanel.Tag.ToString());
            }
            catch { }

            //unselect the selected item
            await Task.Delay(500);
            gridView.SelectedItem = null;

        }

        #endregion

        #region Event handlers for the Open file, folder, and website buttons
        private void OpenFileBtn_Click(object sender, RoutedEventArgs e)
        {
            //init open file dialog
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog() { DereferenceLinks = false };
            openFileDialog.Multiselect = true;

            //check if openfile dialog is ok
            if (openFileDialog.ShowDialog() == true)
            {
                //and then get the filename and YEET in into the grid view
                foreach (string myfile in openFileDialog.FileNames)
                {
                    AddFile(myfile);
                }

            }
        }

        private void OpenFolderBtn_Click(object sender, RoutedEventArgs e)
        {
            //init folderbrowsedialog
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.Multiselect = true;
            CommonFileDialogResult result = dialog.ShowDialog();

            //Add the folder
            if (result == CommonFileDialogResult.Ok)
            {
                //and then get the filename and YEET in into the grid view
                foreach (string myfile in dialog.FileNames)
                {
                    AddFolder(myfile);
                }
            }
        }

        private void OpenWebsiteBtn_Click(object sender, RoutedEventArgs e)
        {
            //show add website dialog
            WebsiteDialog wbd = new WebsiteDialog();
            //event handler to add the website
            wbd.Closed += Wbd_Closed;
            wbd.ShowDialog();
        }

        private void Wbd_Closed(object sender, EventArgs e)
        {

            if (websiteok == true)
            {
                //add the website
                AddWebsite(url, original_url);

                websiteok = false;
            }
        }

        #endregion

        #region Event handlers for application of settings
        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            //show settings window
            SettingsWindow sw = new SettingsWindow();
            sw.Closing += Sw_Closing;
            sw.ShowDialog();
        }

        private void Sw_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //update the size variable
            size = scale;

            try
            {
                //PUT UPDATE SETTINGS CODE HERE

                //---------------------------------------------------------------change scale
                //UPDATE SIZES

                //init a gridview
                var gridView = gridviewhost.Child as Windows.UI.Xaml.Controls.GridView;
                //foreach loop
                foreach (Windows.UI.Xaml.Controls.StackPanel stackpanel in gridView.Items)
                {
                    //update sizes
                    stackpanel.Width = size * 105;
                    stackpanel.Height = size * 90;

                    Windows.UI.Xaml.Controls.TextBlock textBlock = (Windows.UI.Xaml.Controls.TextBlock)stackpanel.Children[1];
                    Windows.UI.Xaml.Controls.Image image = (Windows.UI.Xaml.Controls.Image)stackpanel.Children[0];
                    textBlock.FontSize = size * 11;
                    image.Margin = new Windows.UI.Xaml.Thickness(size * 22.5, 5, size * 22.5, 0);
                    image.Height = stackpanel.Width - size * 22.5 - size * 22.5;
                    image.Width = stackpanel.Width - size * 22.5 - size * 22.5;
                }

                //Update headerText
                header.Text = headerText;
            }
            catch
            {
                //show a error message
                System.Windows.MessageBox.Show("Unable to update one or more settings. Please try again later, " +
                    "or contact the developer.", "Error while updating settings", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region Event handlers for AutoSuggestBox
        private void SearchBox_SuggestionChosen(object sender, RoutedEventArgs e)
        {
            Wpf.Ui.Controls.AutoSuggestBox autoSuggestBox = sender as Wpf.Ui.Controls.AutoSuggestBox;

            //init a gridview and autosuggestbox
            var gridView = gridviewhost.Child as Windows.UI.Xaml.Controls.GridView;

            //check every item in grid, and find the stackpanel with the textblock with the correct text, and then launch the app
            foreach (Windows.UI.Xaml.Controls.StackPanel stackpanel in gridView.Items)
            {
                //textblock
                Windows.UI.Xaml.Controls.TextBlock textblock = (Windows.UI.Xaml.Controls.TextBlock)stackpanel.Children[1];

                //now check if it is the same
                if (textblock.Text.ToLower().ToString() == SearchBox.Text.ToLower().ToString())
                {
                    try
                    { //start the process
                        Process.Start(stackpanel.Tag.ToString());
                    }
                    catch { }

                    break;

                }
            }
            //init a list
            List<string> AutoItems = new List<string>();
            autoSuggestBox.ItemsSource = AutoItems;
        }

        private void SearchBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            //init a list
            List<string> AutoItems = new List<string>();

            //init a gridview and autosuggestbox
            var gridView = gridviewhost.Child as Windows.UI.Xaml.Controls.GridView;

            Wpf.Ui.Controls.AutoSuggestBox autoSuggestBox = sender as Wpf.Ui.Controls.AutoSuggestBox;


            foreach (Windows.UI.Xaml.Controls.StackPanel stackpanel in gridView.Items)
            {
                Windows.UI.Xaml.Controls.TextBlock textBlock = (Windows.UI.Xaml.Controls.TextBlock)stackpanel.Children[1];

                //check if the textblock contains characters from the search. Make everthing lower case as strings are case sensitive.
                if (textBlock.Text.ToLower().Contains(autoSuggestBox.Text.ToLower()) == true)
                {
                    AutoItems.Add(textBlock.Text);
                }

            }
            //and then make the autosuggestbox.itemssource the array
            autoSuggestBox.ItemsSource = AutoItems;
            autoSuggestBox.MaxDropDownHeight = 200;
        }

        #endregion

        #region Saving of settings
        private void window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //stop timers
            themeupdater.Stop();

            //save the items by creating text documents----------------------------------------

            //foreach stackpanel in gridView.items
            var gridView = gridviewhost.Child as Windows.UI.Xaml.Controls.GridView;

            //delete files in directory(loading items directory)
            System.IO.DirectoryInfo di4 = new DirectoryInfo(loadDir);

            foreach (FileInfo file3 in di4.GetFiles())
            {
                file3.Delete();
            }

            foreach (Windows.UI.Xaml.Controls.StackPanel stackpanel in gridView.Items)
            {
                //create a filepath variable
                string filepath = Path.Combine(loadDir, savename.ToString() + ".txt");

                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(stackpanel.Tag.ToString());

                    if (stackpanel.Tag.ToString().StartsWith("https://"))
                    {
                        //it is a website, needs an extra line
                        Windows.UI.Xaml.Controls.TextBlock textBlock = (Windows.UI.Xaml.Controls.TextBlock)stackpanel.Children[1];
                        sw.WriteLine(textBlock.Text);
                    }

                }

                savename += 1;
            }

            //close application manually
            System.Windows.Application.Current.Shutdown();
        }
        #endregion

        #region Event Handlers for focusing window
        private void grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //focus the window
            Focus();
        }

        private void window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //focus the window
            Focus();
        }
        #endregion
    }
}
