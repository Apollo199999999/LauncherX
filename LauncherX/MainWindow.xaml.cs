using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Windows.Storage;
using Microsoft.Toolkit.Wpf;
using Microsoft.Win32;
using System.IO;
using System.DirectoryServices;
using System.Drawing;
using Shell32;
using System.Diagnostics;
using Windows.ApplicationModel.DataTransfer;
using System.Runtime.InteropServices;
using static LauncherX.PublicVariables;
using System.Windows.Interop;
using System.Windows.Threading;
using Windows.UI.Xaml.Controls;
using System.Windows.Forms;
using System.Net;
using System.Web;

namespace LauncherX
{ 

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
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
        
//------------------------------------------------------------------------------------------------------------------
        //size variable to control size of icons
        public double size;

        //inprogress window, used to tell users that it is adding items
        InProgress progress = new InProgress();

        //AddItemsErrorDialog, in case when loading the files after application startup, the file does not exist
        AddItemsErrorDialog AddItemsErrorDialog = new AddItemsErrorDialog();

        //create a new dispatcher timer
        DispatcherTimer themeupdater = new DispatcherTimer();

        //init directory strings
        public string loadDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\LauncherX\\Files\\";
        public string appIconDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\LauncherX\\Temp\\AppIcons\\";
        public string folderIconDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\LauncherX\\Temp\\FolderIcons\\";
        public string websiteIconDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\LauncherX\\Temp\\WebsiteIcons\\";


        public MainWindow()
        {
            InitializeComponent();

            //set the button reveal style
            var buttonstyle = (Style)App.Current.Resources["ButtonRevealStyle"];
            SettingsButton.Style = buttonstyle;
           
            //upgrade and reload settings
            Properties.Settings.Default.Upgrade();
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();

            //set size value
            size = Properties.Settings.Default.scale;

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

        private void Themeupdater_Tick(object sender, EventArgs e)
        {
            //check and update theme
            CheckAndUpdateTheme();
        }

        private void CheckAndUpdateTheme()
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

            //and then, create to seperate solid color brushes for the theme color accordingly
            SolidColorBrush lightTheme = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 243, 243, 243));
            SolidColorBrush darkTheme = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 32, 32, 32));

            //check light/dark mode, change colors accordingly
            if (is_light_mode == true)
            {
                grid.Background = lightTheme;
            }
            else if (is_light_mode == false)
            {
                grid.Background = darkTheme;
            }
        }

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

            //check if there are files in the loading directory
            if (IsDirectoryEmpty(loadDir) == false)
            {
                //show the progress window
                progress.label.Text = "Loading your items, please wait...";
                progress.Show();

                //wait a while for the progress window to open
                await Task.Delay(500);
            }

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
                        AddWebsite(s);

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
                                AddItem(s);
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

            progress.Hide();

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

        }
        

        //these variables are used for duplicate naming
        public int foldercount = 0;
        public int count = 0;
        public int websitecount = 0;

        //this variable is used in case the user wants to remove the item(**file**)
        public object filetoremove;

        //this variable is used to look for thr **file **to open
        public string filetolook;

        //this variable is used to look for the **folder** to open
        public string foldertolook;

        //this variable is used in case the user wants to remove the item(**folder**)
        public object foldertoremove;

        //this variable is used to look for the **website** to open
        public string websitetolook;

        //this variable is used in case the user wants to remove the item (**website**)
        public object websitetoremove;

        //this variable is used to save the items in the gridview, based on order
        public int savename = 1;

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

        public void AddItem(string myfile)
        {
            //init a gridview
            var gridView = gridviewhost.Child as Windows.UI.Xaml.Controls.GridView;

            //show gridView
            gridviewhost.Visibility = Visibility.Visible;

            //init filename
            var filename = System.IO.Path.GetFileNameWithoutExtension(myfile);
            filename = filename + ".tiff";

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
                if (File.Exists(Path.Combine(appIconDir, filename)))
                {
                    filename = count.ToString() + filename;
                    FileStream stream = new FileStream(System.IO.Path.Combine(appIconDir, filename), FileMode.Create);
                    System.Drawing.Image image1 = System.Drawing.Image.FromFile(myfile);
                    System.Drawing.Image icon = image1.GetThumbnailImage(image1.Width, image1.Height, () => false, IntPtr.Zero);
                    icon.Save(stream, System.Drawing.Imaging.ImageFormat.Tiff);
                    count += 1;

                }
                else
                {
                    FileStream stream = new FileStream(System.IO.Path.Combine(appIconDir, filename), FileMode.Create);
                    System.Drawing.Image image1 = System.Drawing.Image.FromFile(myfile);
                    System.Drawing.Image icon = image1.GetThumbnailImage(image1.Width, image1.Height, () => false, IntPtr.Zero);
                    icon.Save(stream, System.Drawing.Imaging.ImageFormat.Tiff);
                }
            }
            else
            {
                if (File.Exists(Path.Combine(appIconDir, filename)))
                {
                    filename = count.ToString() + filename;
                    FileStream stream = new FileStream(System.IO.Path.Combine(appIconDir, filename), FileMode.Create);
                    Bitmap icon = new Bitmap(System.Drawing.Icon.ExtractAssociatedIcon(myfile).ToBitmap());
                    icon.Save(stream, System.Drawing.Imaging.ImageFormat.Tiff);
                    count += 1;

                }
                else
                {
                    FileStream stream = new FileStream(System.IO.Path.Combine(appIconDir, filename), FileMode.Create);
                    Bitmap icon = new Bitmap(System.Drawing.Icon.ExtractAssociatedIcon(myfile).ToBitmap());
                    icon.Save(stream, System.Drawing.Imaging.ImageFormat.Tiff);
                }
            }

            

            //create a stackpanel
            Windows.UI.Xaml.Controls.StackPanel stackpanel = new Windows.UI.Xaml.Controls.StackPanel();
            stackpanel.Width = size * 105;
            stackpanel.Height = size * 90;
            //for some reason, it needs to have a background in order for right tap to work??
            stackpanel.Background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(0,0,0,0));

            //load file icon into uwp image control
            Windows.UI.Xaml.Controls.Image image = new Windows.UI.Xaml.Controls.Image();
            string path = Path.Combine(appIconDir + filename);
            Uri fileuri = new Uri(path);
            image.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(fileuri);
            image.Stretch = Windows.UI.Xaml.Media.Stretch.Uniform;
            image.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center;
            image.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center;
            image.Margin = new Windows.UI.Xaml.Thickness(size*22.5, 5, size*22.5, 0);
            image.Height = stackpanel.Width - size * 22.5 - size * 22.5;
            image.Width = stackpanel.Width - size * 22.5 - size * 22.5;

            //create a textblock
            Windows.UI.Xaml.Controls.TextBlock textblock = new Windows.UI.Xaml.Controls.TextBlock();
            textblock.TextAlignment = Windows.UI.Xaml.TextAlignment.Center;
            textblock.Text = Path.GetFileName(myfile);
            textblock.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center;
            textblock.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Bottom;
            textblock.FontSize = size * 11;
            textblock.Margin = new Windows.UI.Xaml.Thickness(5);
            textblock.TextTrimming = Windows.UI.Xaml.TextTrimming.CharacterEllipsis;

            //save the path in stackpanel tag
            stackpanel.Tag = myfile;

            //init a tooltip
            Windows.UI.Xaml.Controls.ToolTip toolTip = new Windows.UI.Xaml.Controls.ToolTip();
            toolTip.Content = Path.GetFileName(myfile);

            //set the tooltipowner to the stackpanel using tooltip service
            Windows.UI.Xaml.Controls.ToolTipService.SetToolTip(stackpanel, toolTip);

            //righttapped event handler for menu flyout
            stackpanel.RightTapped += Stackpanel_RightTapped;
            stackpanel.PointerPressed += Stackpanel_PointerPressed;

            //add the controls
            stackpanel.Children.Add(image);
            stackpanel.Children.Add(textblock);
            gridView.Items.Add(stackpanel);

        }

        private void Stackpanel_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            //init a stackpanel from sender
            Windows.UI.Xaml.Controls.StackPanel stackPanel = sender as Windows.UI.Xaml.Controls.StackPanel;

            //disable the righttapped event
            stackPanel.RightTapped -= Stackpanel_RightTapped;

            // Check for input device
            if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse)
            {
                var properties = e.GetCurrentPoint(stackPanel).Properties;
                
                if (properties.IsRightButtonPressed)
                {
                    //assign stackpanel tag to texttolook
                    filetolook = stackPanel.Tag.ToString();

                    //assign stackpanel to filetoremove
                    filetoremove = stackPanel;

                    //init a new menu flyout
                    Windows.UI.Xaml.Controls.MenuFlyout menu = new Windows.UI.Xaml.Controls.MenuFlyout();

                    //init menu flyout items
                    Windows.UI.Xaml.Controls.MenuFlyoutItem open = new Windows.UI.Xaml.Controls.MenuFlyoutItem();
                    Windows.UI.Xaml.Controls.MenuFlyoutItem openfilelocation = new Windows.UI.Xaml.Controls.MenuFlyoutItem();
                    Windows.UI.Xaml.Controls.MenuFlyoutItem remove = new Windows.UI.Xaml.Controls.MenuFlyoutItem();

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

                    //event handlers for the items
                    open.Click += Open_Click;
                    openfilelocation.Click += Openfilelocation_Click;
                    remove.Click += Remove_Click;

                    //add them to the menuflyout
                    menu.Items.Add(open);
                    menu.Items.Add(openfilelocation);
                    menu.Items.Add(remove);

                    //show the menyflyout
                    menu.ShowAt(sender as Windows.UI.Xaml.UIElement, e.GetCurrentPoint(stackPanel).Position);
                }
            }
            else
            {
                //if the input device is not a mouse, enable the righttapped event
                stackPanel.RightTapped += Stackpanel_RightTapped;
            }
        }

        

        private void Stackpanel_RightTapped(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            //assign stackpanel tag to filetolook
            Windows.UI.Xaml.Controls.StackPanel stackPanel = sender as Windows.UI.Xaml.Controls.StackPanel;
            filetolook = stackPanel.Tag.ToString();

            //assign stackpanel to filetoremove
            filetoremove = stackPanel;

            //init a new menu flyout
            Windows.UI.Xaml.Controls.MenuFlyout menu = new Windows.UI.Xaml.Controls.MenuFlyout();

            //init menu flyout items
            Windows.UI.Xaml.Controls.MenuFlyoutItem open = new Windows.UI.Xaml.Controls.MenuFlyoutItem();
            Windows.UI.Xaml.Controls.MenuFlyoutItem openfilelocation = new Windows.UI.Xaml.Controls.MenuFlyoutItem();
            Windows.UI.Xaml.Controls.MenuFlyoutItem remove = new Windows.UI.Xaml.Controls.MenuFlyoutItem();

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

            //event handlers for the items
            open.Click += Open_Click;
            openfilelocation.Click += Openfilelocation_Click;
            remove.Click += Remove_Click;

            //add them to the menuflyout
            menu.Items.Add(open);
            menu.Items.Add(openfilelocation);
            menu.Items.Add(remove);

            //show the menyflyout
            menu.ShowAt(sender as Windows.UI.Xaml.UIElement, e.GetPosition(sender as Windows.UI.Xaml.UIElement));

        }

        private void Remove_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //init gridView
            var gridView = gridviewhost.Child as Windows.UI.Xaml.Controls.GridView;

            //remove the item
            gridView.Items.Remove(filetoremove);

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

            //clear the variable
            filetoremove = null;

        }

        private void Openfilelocation_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //open file location
            Process.Start("explorer.exe", "/select, " + filetolook);

            //clear the variable
            filetolook = null;
        }

        private void Open_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                //start the process
                Process.Start(filetolook);
            }
            catch { }
            //clear the variable
            filetolook = null;
        }



        //add folder
        public void AddFolder(string directory)
        {
            //so, basic idea, copy the add_item and remove non essential functions

            //init a gridview
            var gridView = gridviewhost.Child as Windows.UI.Xaml.Controls.GridView;

            //show gridView
            gridviewhost.Visibility = Visibility.Visible;

            //init foldername
            string foldername = new DirectoryInfo(directory).Name;
            foldername = foldername + ".tiff";

            //now check if the icon of the folder exists in directory
            if (File.Exists(System.IO.Path.Combine(folderIconDir, foldername)))
            {
                foldername = foldercount.ToString() + foldername;

                //get the icon of the folder
                SHFILEINFO shinfo = new SHFILEINFO();
                SHGetFileInfo(directory, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), SHGFI_ICON | SHGFI_LARGEICON);
                Icon i = System.Drawing.Icon.FromHandle(shinfo.hIcon);

                //init filestream
                FileStream stream = new FileStream(System.IO.Path.Combine(folderIconDir, foldername), FileMode.Create);
                Bitmap icon = i.ToBitmap();
                icon.Save(stream, System.Drawing.Imaging.ImageFormat.Tiff);

                foldercount += 1;
            }
            else
            {
                //get the icon of the folder
                SHFILEINFO shinfo = new SHFILEINFO();
                SHGetFileInfo(directory, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), SHGFI_ICON | SHGFI_LARGEICON);
                Icon i = System.Drawing.Icon.FromHandle(shinfo.hIcon);

                //init filestream
                FileStream stream = new FileStream(System.IO.Path.Combine(folderIconDir, foldername), FileMode.Create);
                Bitmap icon = i.ToBitmap();
                icon.Save(stream, System.Drawing.Imaging.ImageFormat.Tiff);
            }

            //then do the controls and add it to the gridView
            //create a stackpanel
            Windows.UI.Xaml.Controls.StackPanel stackpanel = new Windows.UI.Xaml.Controls.StackPanel();
            stackpanel.Width = size * 105;
            stackpanel.Height = size * 90;

            //for some reason, it needs to have a background in order for right tap to work??
            stackpanel.Background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(0, 0, 0, 0));

            //load file icon into uwp image control
            Windows.UI.Xaml.Controls.Image image = new Windows.UI.Xaml.Controls.Image();
            string path = Path.Combine(folderIconDir + foldername);
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
            textblock.Text = new DirectoryInfo(directory).Name;
            textblock.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center;
            textblock.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Bottom;
            textblock.FontSize = size * 11;
            textblock.Margin = new Windows.UI.Xaml.Thickness(5);
            textblock.TextTrimming = Windows.UI.Xaml.TextTrimming.CharacterEllipsis;


            //save the path in stackpanel tag
            stackpanel.Tag = directory;

            //init a tooltip
            Windows.UI.Xaml.Controls.ToolTip toolTip = new Windows.UI.Xaml.Controls.ToolTip();
            toolTip.Content = new DirectoryInfo(directory).Name;

            //set the tooltipowner to the stackpanel using tooltip service
            Windows.UI.Xaml.Controls.ToolTipService.SetToolTip(stackpanel, toolTip);

            //stackpanel righttapped event handler to show menu flyout
            stackpanel.RightTapped += Stackpanel_RightTapped1;
            stackpanel.PointerPressed += Stackpanel_PointerPressed1;

            //add the controls
            stackpanel.Children.Add(image);
            stackpanel.Children.Add(textblock);
            gridView.Items.Add(stackpanel);

        }

        private void Stackpanel_PointerPressed1(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            //init a stackpanel from sender
            Windows.UI.Xaml.Controls.StackPanel stackPanel = sender as Windows.UI.Xaml.Controls.StackPanel;

            //disable the righttapped event
            stackPanel.RightTapped -= Stackpanel_RightTapped1;

            // Check for input device
            if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse)
            {
                var properties = e.GetCurrentPoint(stackPanel).Properties;

                if (properties.IsRightButtonPressed)
                {
                    // assign stackpanel tag to foldertolook
                    foldertolook = stackPanel.Tag.ToString();

                    //assign stackpanel to foldertoremove
                    foldertoremove = stackPanel;

                    //init a new menu flyout
                    Windows.UI.Xaml.Controls.MenuFlyout menu = new Windows.UI.Xaml.Controls.MenuFlyout();

                    //init menu flyout items
                    Windows.UI.Xaml.Controls.MenuFlyoutItem open = new Windows.UI.Xaml.Controls.MenuFlyoutItem();
                    Windows.UI.Xaml.Controls.MenuFlyoutItem openfilelocation = new Windows.UI.Xaml.Controls.MenuFlyoutItem();
                    Windows.UI.Xaml.Controls.MenuFlyoutItem remove = new Windows.UI.Xaml.Controls.MenuFlyoutItem();

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

                    //event handlers
                    open.Click += Open_Click1;
                    remove.Click += Remove_Click1;
                    openfilelocation.Click += Openfilelocation_Click1;

                    //add them to the menuflyout
                    menu.Items.Add(open);
                    menu.Items.Add(openfilelocation);
                    menu.Items.Add(remove);

                    //show the menyflyout
                    menu.ShowAt(sender as Windows.UI.Xaml.UIElement, e.GetCurrentPoint(stackPanel).Position);
                }
            }
            else
            {
                //if the input device is not a mouse, enable the righttapped event
                stackPanel.RightTapped += Stackpanel_RightTapped1;
            }
        }

        private void Stackpanel_RightTapped1(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            //assign stackpanel textblock to texttolook
            Windows.UI.Xaml.Controls.StackPanel stackPanel = sender as Windows.UI.Xaml.Controls.StackPanel;
            foldertolook = stackPanel.Tag.ToString();

            //assign stackpanel to foldertoremove
            foldertoremove = stackPanel;

            //init a new menu flyout
            Windows.UI.Xaml.Controls.MenuFlyout menu = new Windows.UI.Xaml.Controls.MenuFlyout();

            //init menu flyout items
            Windows.UI.Xaml.Controls.MenuFlyoutItem open = new Windows.UI.Xaml.Controls.MenuFlyoutItem();
            Windows.UI.Xaml.Controls.MenuFlyoutItem openfilelocation = new Windows.UI.Xaml.Controls.MenuFlyoutItem();
            Windows.UI.Xaml.Controls.MenuFlyoutItem remove = new Windows.UI.Xaml.Controls.MenuFlyoutItem();

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

            //event handlers
            open.Click += Open_Click1;
            remove.Click += Remove_Click1;
            openfilelocation.Click += Openfilelocation_Click1;

            //add them to the menuflyout
            menu.Items.Add(open);
            menu.Items.Add(openfilelocation);
            menu.Items.Add(remove);

            //show the menyflyout
            menu.ShowAt(sender as Windows.UI.Xaml.UIElement, e.GetPosition(sender as Windows.UI.Xaml.UIElement));

        }

        private void Openfilelocation_Click1(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //open folder location
            Process.Start("explorer.exe", "/select, " + foldertolook);

            //clear the variable
            foldertolook = null;
        }

        private void Remove_Click1(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //init a gridView
            var gridView = gridviewhost.Child as Windows.UI.Xaml.Controls.GridView;

            //remove the item
            gridView.Items.Remove(foldertoremove);

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

            //clear the variable
            foldertoremove = null;
        }

        private void Open_Click1(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                //start the process
                Process.Start(foldertolook);
            }
            catch { }
            //clear the variable
            foldertolook = null;
        }

        private void AddWebsite(string url)
        {
            //init a gridview
            var gridView = gridviewhost.Child as Windows.UI.Xaml.Controls.GridView;

            //show gridView
            gridviewhost.Visibility = Visibility.Visible;

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
            string downloadaddress = "https://www.google.com/s2/favicons?sz=64&domain_url=https://" + url;

            //init a new webclient
            WebClient webClient = new WebClient();
           

            if (File.Exists(Path.Combine(websiteIconDir, filename)))
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

        private void Stackpanel_PointerPressed2(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            //init a stackpanel from sender
            Windows.UI.Xaml.Controls.StackPanel stackPanel = sender as Windows.UI.Xaml.Controls.StackPanel;

            //disable the righttapped event
            stackPanel.RightTapped -= Stackpanel_RightTapped2;

            // Check for input device
            if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse)
            {
                var properties = e.GetCurrentPoint(stackPanel).Properties;

                if (properties.IsRightButtonPressed)
                {
                    //assign stackpanel tag to websitetolook
                    websitetolook = stackPanel.Tag.ToString();

                    //assign stackpanel to websitetoremove
                    websitetoremove = stackPanel;

                    //init a new menu flyout
                    Windows.UI.Xaml.Controls.MenuFlyout menu = new Windows.UI.Xaml.Controls.MenuFlyout();

                    //init menu flyout items
                    Windows.UI.Xaml.Controls.MenuFlyoutItem open = new Windows.UI.Xaml.Controls.MenuFlyoutItem();
                    Windows.UI.Xaml.Controls.MenuFlyoutItem remove = new Windows.UI.Xaml.Controls.MenuFlyoutItem();

                  
                    //set properties and icons for the menuitems
                    open.Icon = new SymbolIcon(Symbol.OpenFile);
                    open.Text = "Open website";

                    remove.Icon = new SymbolIcon(Symbol.Delete);
                    remove.Text = "Remove item from LauncherX";

                    //event handlers for the items
                    open.Click += Open_Click2;
                    remove.Click += Remove_Click2;

                    //add them to the menuflyout
                    menu.Items.Add(open);
                    menu.Items.Add(remove);

                    //show the menyflyout
                    menu.ShowAt(sender as Windows.UI.Xaml.UIElement, e.GetCurrentPoint(stackPanel).Position);
                }
            }
            else
            {
                //if the input device is not a mouse, enable the righttapped event
                stackPanel.RightTapped += Stackpanel_RightTapped2;
            }
        }

        private void Remove_Click2(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //init a gridView
            var gridView = gridviewhost.Child as Windows.UI.Xaml.Controls.GridView;

            //remove the item
            gridView.Items.Remove(websitetoremove);

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

            //clear the variable
            websitetoremove = null;
        }

        private void Open_Click2(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                //start the process
                Process.Start(websitetolook);
            }
            catch
            { }
            //clear the variable
            websitetolook = null;
        }

        private void Stackpanel_RightTapped2(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            //init a stackpanel
            Windows.UI.Xaml.Controls.StackPanel stackPanel = sender as Windows.UI.Xaml.Controls.StackPanel;

            //assign stackpanel tag to websitetolook
            websitetolook = stackPanel.Tag.ToString();

            //assign stackpanel to websitetoremove
            websitetoremove = stackPanel;

            //init a new menu flyout
            Windows.UI.Xaml.Controls.MenuFlyout menu = new Windows.UI.Xaml.Controls.MenuFlyout();

            //init menu flyout items
            Windows.UI.Xaml.Controls.MenuFlyoutItem open = new Windows.UI.Xaml.Controls.MenuFlyoutItem();
            Windows.UI.Xaml.Controls.MenuFlyoutItem remove = new Windows.UI.Xaml.Controls.MenuFlyoutItem();


            //set properties and icons for the menuitems
            open.Icon = new SymbolIcon(Symbol.OpenFile);
            open.Text = "Open website";

            remove.Icon = new SymbolIcon(Symbol.Delete);
            remove.Text = "Remove item from LauncherX";

            //event handlers for the items
            open.Click += Open_Click2;
            remove.Click += Remove_Click2;

            //add them to the menuflyout
            menu.Items.Add(open);
            menu.Items.Add(remove);

            //show the menyflyout
            menu.ShowAt(sender as Windows.UI.Xaml.UIElement, e.GetPosition(sender as Windows.UI.Xaml.UIElement));
        }

        private void OpenFileHost_ChildChanged(object sender, EventArgs e)
        {
            //init button and set properties
            var button = OpenFileHost.Child as Windows.UI.Xaml.Controls.Button;
            button.Content = "Add a file";
            button.Click += OpenFile_Click;
            OpenFileHost.ChildChanged -= OpenFileHost_ChildChanged;
        }
      
        
        private async void OpenFile_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //init open file dialog
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog() { DereferenceLinks = false };
            openFileDialog.Multiselect = true;

            //check if openfile dialog is ok
            if (openFileDialog.ShowDialog() == true)
            {
                progress.Show();
                progress.label.Text = "Adding your items, please wait...";

                //this delay is to allow some time for the progress dialog to open
                await Task.Delay(500);

                //and then get the icon and YEET in into the grid view
                foreach (string myfile in openFileDialog.FileNames)
                {
                    AddItem(myfile);
                }

                progress.Hide();
            }
        }

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

        private void OpenFolderHost_ChildChanged(object sender, EventArgs e)
        {
            //init a button and declare properties
            var folderbutton = OpenFolderHost.Child as Windows.UI.Xaml.Controls.Button;
            folderbutton.Content = "Add a folder";

            //event handlers
            folderbutton.Click += Folderbutton_Click;
            OpenFolderHost.ChildChanged -= OpenFolderHost_ChildChanged;

        }

        private async void Folderbutton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //init folderbrowsedialog
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog =
                new System.Windows.Forms.FolderBrowserDialog();

            //Add the folder
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                progress.Show();
                progress.label.Text = "Adding your items, please wait...";

                //this delay is to allow some time for the progress dialog to open
                await Task.Delay(500);

                AddFolder(folderBrowserDialog.SelectedPath);
                progress.Hide();
            }

        }

        private void SearchHost_ChildChanged(object sender, EventArgs e)
        {
            //Make the auto suggest box. Demos and guides in Xaml controls gallery and Microsoft docs
            var AutoSuggestBox = SearchHost.Child as Windows.UI.Xaml.Controls.AutoSuggestBox;

            //declare properties
            AutoSuggestBox.PlaceholderText = "Search through everything in LauncherX";
            AutoSuggestBox.QueryIcon = new SymbolIcon(Symbol.Find);

            //event handlers
            //3 autosuggest event handlers need to handle
            AutoSuggestBox.TextChanged += AutoSuggestBox_TextChanged;
            AutoSuggestBox.SuggestionChosen += AutoSuggestBox_SuggestionChosen;
            AutoSuggestBox.QuerySubmitted += AutoSuggestBox_QuerySubmitted;

            SearchHost.ChildChanged -= SearchHost_ChildChanged;
        }

        //create a new list to store the itemssource
        List<string> Query = new List<string>();

        private void AutoSuggestBox_QuerySubmitted(Windows.UI.Xaml.Controls.AutoSuggestBox sender, Windows.UI.Xaml.Controls.AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            try
            {
                //get the first item of the list, and then assign it to text
                string text = Query[0];

                //init a gridview and autosuggestbox
                var gridView = gridviewhost.Child as Windows.UI.Xaml.Controls.GridView;

                //check every item in grid, and find the stackpanel with the textblock with the correct text, and then launch the app
                foreach (Windows.UI.Xaml.Controls.StackPanel stackpanel in gridView.Items)
                {
                    //textblock
                    Windows.UI.Xaml.Controls.TextBlock textblock = (Windows.UI.Xaml.Controls.TextBlock)stackpanel.Children[1];

                    //now check if it is the same
                    if (textblock.Text.ToLower() == text.ToLower())
                    {
                        try
                        {
                            //start the process
                            Process.Start(stackpanel.Tag.ToString());
                        }
                        catch { }
                    }
                }
                //init a list
                List<string> AutoItems = new List<string>();
                sender.ItemsSource = AutoItems;
            }
            catch { }

        }

        private void AutoSuggestBox_SuggestionChosen(Windows.UI.Xaml.Controls.AutoSuggestBox sender, Windows.UI.Xaml.Controls.AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            var AutoSuggestBox = SearchHost.Child as Windows.UI.Xaml.Controls.AutoSuggestBox;

            //disable the event handler
            AutoSuggestBox.QuerySubmitted -= AutoSuggestBox_QuerySubmitted;

            //init a gridview and autosuggestbox
            var gridView = gridviewhost.Child as Windows.UI.Xaml.Controls.GridView;

            //check every item in grid, and find the stackpanel with the textblock with the correct text, and then launch the app
            foreach (Windows.UI.Xaml.Controls.StackPanel stackpanel in gridView.Items)
            {
                //textblock
                Windows.UI.Xaml.Controls.TextBlock textblock = (Windows.UI.Xaml.Controls.TextBlock)stackpanel.Children[1];

                //now check if it is the same
                if (textblock.Text.ToLower() == args.SelectedItem.ToString().ToLower())
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
            sender.ItemsSource = AutoItems;

            //enable the event handler
            AutoSuggestBox.QuerySubmitted += AutoSuggestBox_QuerySubmitted;

        }



        private void AutoSuggestBox_TextChanged(Windows.UI.Xaml.Controls.AutoSuggestBox sender, Windows.UI.Xaml.Controls.AutoSuggestBoxTextChangedEventArgs args)
        {
            //init a list
            List<string> AutoItems = new List<string>();

            //init a gridview and autosuggestbox
            var gridView = gridviewhost.Child as Windows.UI.Xaml.Controls.GridView;

            var AutoSuggestBox = SearchHost.Child as Windows.UI.Xaml.Controls.AutoSuggestBox;
            

            foreach (Windows.UI.Xaml.Controls.StackPanel stackpanel in gridView.Items)
            {
                Windows.UI.Xaml.Controls.TextBlock textBlock = (Windows.UI.Xaml.Controls.TextBlock)stackpanel.Children[1];

                //check if the textblock contains characters from the search. Make everthing lower case as strings are case sensitive.
                if (textBlock.Text.ToLower().Contains(AutoSuggestBox.Text.ToLower()) == true)
                {
                    AutoItems.Add(textBlock.Text);
                }
                
            }
            //and then make the autosuggestbox.itemssource the array
            AutoSuggestBox.ItemsSource = AutoItems;
            AutoSuggestBox.MaxSuggestionListHeight = 200;

            //assign to query, this list is used for query_submitted
            Query = AutoItems;

        }

        private void OpenWebsiteHost_ChildChanged(object sender, EventArgs e)
        {
            //init button and set properties
            var openwebsite = OpenWebsiteHost.Child as Windows.UI.Xaml.Controls.Button;

            openwebsite.Content = "Add a website";

            //event handlers
            openwebsite.Click += Openwebsite_Click;
            OpenWebsiteHost.ChildChanged -= OpenWebsiteHost_ChildChanged;
        }

        private void Openwebsite_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //show add website dialog
            WebsiteDialog wbd = new WebsiteDialog();
            wbd.Show();

            //event handler to add the website
            wbd.Closed += Wbd_Closed;
        }

        private async void Wbd_Closed(object sender, EventArgs e)
        {
            
            if (websiteok == true)
            {
                progress.Show();
                progress.label.Text = "Adding your items, please wait...";

                //this delay is to allow some time for the progress dialog to open
                await Task.Delay(500);

                //add the website
                AddWebsite(url);

                websiteok = false;
                progress.Hide();
            }
        }

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

       
    }
}
