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
using static LauncherX.PublicVariables;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows.Controls;
using System.Windows.Media;
using Image = System.Windows.Controls.Image;
using System.Windows.Media.Imaging;
using Color = System.Windows.Media.Color;

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
        public string fileIconDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\LauncherX\\Temp\\AppIcons\\";
        public string folderIconDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\LauncherX\\Temp\\FolderIcons\\";
        public string websiteIconDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\LauncherX\\Temp\\WebsiteIcons\\";

        #endregion

        public MainWindow()
        {
            InitializeComponent();

            //save Window Pos
            ((App)Application.Current).WindowPlace.Register(this);

            //upgrade settings if needed
            if (Properties.Settings.Default.UpgradeRequired)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.UpgradeRequired = false;
                Properties.Settings.Default.Save();
            }

            //set size value
            scale = Properties.Settings.Default.scale;
            size = Math.Sqrt(scale);

            //set header text
            header.Text = Properties.Settings.Default.headerText;

            //create directories
            Directory.CreateDirectory(loadDir);
            Directory.CreateDirectory(fileIconDir);
            Directory.CreateDirectory(folderIconDir);
            Directory.CreateDirectory(websiteIconDir);

            //delete files in directory (appicons)
            System.IO.DirectoryInfo di = new DirectoryInfo(fileIconDir);

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

            //create a dispatcher timer to check for theme        
            themeupdater.Interval = TimeSpan.FromMilliseconds(100);
            themeupdater.Tick += Themeupdater_Tick;
            themeupdater.Start();

            //event handler for when window loads to check for updates
            this.Loaded += MainWindow_Loaded;

        }

        #region Checking for updates and loading items when LauncherX loads
        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //wait a while for controls and window to load
            LoadingDialog.Visibility = Visibility.Visible;
            this.IsEnabled = false;

            await Task.Delay(100);

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
            if (WPFGridView.Items.Count == 0)
            {
                WPFGridView.Visibility = Visibility.Hidden;
                EmptyNotice.Visibility = Visibility.Visible;
            }
            else
            {
                WPFGridView.Visibility = Visibility.Visible;
                EmptyNotice.Visibility = Visibility.Hidden;
            }

            if (ErrorAddingItems == true)
            {
                //show the additemserrordialog
                AddItemsErrorDialog.ShowDialog();
            }

            //activate and focus this window
            this.Activate();
            this.Focus();

            LoadingDialog.Visibility = Visibility.Collapsed;
            this.IsEnabled = true;

            /*check if internet connection exists, and if so, check for updates, and if there are updates,
            show a snackbar to ask if the user wants to update*/

            if (CheckForUpdates() == true)
            {
                //show the messagebox
                UpdateSnackBar.Show();
                UpdateBtn.Click += (s, args) =>
                {
                    //navigate to github releases page
                    Process.Start("https://github.com/Apollo199999999/LauncherX/releases");

                    //close this app
                    themeupdater.Stop();
                    System.Windows.Application.Current.Shutdown();
                };

            }

        }

        #endregion

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
            }
            else if (is_light_mode == false)
            {
                Wpf.Ui.Appearance.Theme.Apply(
                  Wpf.Ui.Appearance.ThemeType.Dark,      // Theme type
                  Wpf.Ui.Appearance.BackgroundType.Mica, // Background type
                  true                                   // Whether to change accents automatically
                );
            }

        }

        private void Themeupdater_Tick(object sender, EventArgs e)
        {
            //check and update theme
            CheckAndUpdateTheme();
        }

        #endregion

        #region Methods relating to GridView Items

        #region Methods relating to creation and handling of GridView Items
        public StackPanel CreateGridViewItem(string iconPath, string DisplayText, string StackPanelTag)
        {
            //create a stackpanel
            StackPanel stackpanel = new StackPanel();
            stackpanel.Width = size * 105;
            stackpanel.Height = size * 95;
            //for some reason, it needs to have a background in order for right tap to work??
            stackpanel.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

            //load file icon into image control
            Image image = new Image();
            string path = iconPath;
            Uri fileuri = new Uri(path);
            image.Source = new BitmapImage(fileuri);
            image.Stretch = Stretch.Uniform;
            image.VerticalAlignment = VerticalAlignment.Center;
            image.HorizontalAlignment = HorizontalAlignment.Center;
            image.Margin = new Thickness(size * 22.5, 5, size * 22.5, 0);
            image.Height = stackpanel.Width - size * 22.5 - size * 22.5;
            image.Width = stackpanel.Width - size * 22.5 - size * 22.5;
            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.LowQuality);

            //create a textblock
            TextBlock textblock = new TextBlock();
            textblock.TextAlignment = TextAlignment.Center;
            textblock.Text = DisplayText;
            textblock.HorizontalAlignment = HorizontalAlignment.Center;
            textblock.VerticalAlignment = VerticalAlignment.Bottom;
            textblock.FontSize = size * 11;
            textblock.Margin = new Thickness(5);
            textblock.TextTrimming = TextTrimming.CharacterEllipsis;

            //save the path in stackpanel tag
            stackpanel.Tag = StackPanelTag;

            //init a tooltip
            ToolTip toolTip = new ToolTip();
            toolTip.Content = DisplayText;

            //set the tooltipowner to the stackpanel using tooltip service
            ToolTipService.SetToolTip(stackpanel, toolTip);

            //left click event handler for starting the process
            stackpanel.MouseLeftButtonUp += GridViewItem_LeftClicked;
            //right click event handler for menu flyout
            stackpanel.MouseRightButtonUp += GridViewItem_RightClicked;
            //mouseenter and mouseleave events for stackpanel mouse highlight effect
            stackpanel.MouseEnter += GridViewItem_MouseEnter;
            stackpanel.MouseLeave += GridViewItem_MouseLeave;

            //add the controls
            stackpanel.Children.Add(image);
            stackpanel.Children.Add(textblock);

            return stackpanel;

        }

        private async void GridViewItem_LeftClicked(object sender, MouseButtonEventArgs e)
        {
            //init stackpanel from sender
            StackPanel stackPanel = sender as StackPanel;
            try
            {
                //start the process that is related to the tag
                Process.Start(stackPanel.Tag.ToString());
            }
            catch { }

            //unselect the selected item
            await Task.Delay(500);
            WPFGridView.SelectedItem = null;
        }

        private void GridViewItem_RightClicked(object sender, MouseButtonEventArgs e)
        {
            //init stackpanel from sender
            StackPanel stackPanel = sender as StackPanel;

            //create context menu for stackpanel
            ContextMenu menu = CreateGridViewItemContextMenu(stackPanel);
            stackPanel.ContextMenu = menu;
        }

        private void GridViewItem_MouseEnter(object sender, MouseEventArgs e)
        {
            //set the background of the stackpanel
            StackPanel stackPanel = sender as StackPanel;
            stackPanel.Background = Application.Current.Resources["ControlFillColorSecondaryBrush"] as SolidColorBrush;
        }

        private void GridViewItem_MouseLeave(object sender, MouseEventArgs e)
        {
            //restore the background of the stackpanel
            StackPanel stackPanel = sender as StackPanel;
            stackPanel.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        }

        //add file
        public void AddFile(string myfile)
        {
            //show gridView
            //gridviewhost.Visibility = Visibility.Visible;
            WPFGridView.Visibility = Visibility.Visible;
            EmptyNotice.Visibility = Visibility.Hidden;  

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
                if (File.Exists(Path.Combine(fileIconDir, FileIconName)))
                {
                    FileIconName = FileNameCount.ToString() + FileIconName;
                    FileStream stream = new FileStream(System.IO.Path.Combine(fileIconDir, FileIconName), FileMode.Create);
                    System.Drawing.Image image1 = System.Drawing.Image.FromFile(myfile);
                    System.Drawing.Image icon = image1.GetThumbnailImage(image1.Width, image1.Height, () => false, IntPtr.Zero);
                    icon.Save(stream, System.Drawing.Imaging.ImageFormat.Tiff);
                    FileNameCount += 1;
                    stream.Dispose();
                }
                else
                {
                    FileStream stream = new FileStream(System.IO.Path.Combine(fileIconDir, FileIconName), FileMode.Create);
                    System.Drawing.Image image1 = System.Drawing.Image.FromFile(myfile);
                    System.Drawing.Image icon = image1.GetThumbnailImage(image1.Width, image1.Height, () => false, IntPtr.Zero);
                    icon.Save(stream, System.Drawing.Imaging.ImageFormat.Tiff);
                    stream.Dispose();
                }
            }
            else
            {
                if (File.Exists(Path.Combine(fileIconDir, FileIconName)))
                {
                    FileIconName = FileNameCount.ToString() + FileIconName;
                    FileStream stream = new FileStream(System.IO.Path.Combine(fileIconDir, FileIconName), FileMode.Create);
                    Bitmap icon = new Bitmap(System.Drawing.Icon.ExtractAssociatedIcon(myfile).ToBitmap());
                    icon.Save(stream, System.Drawing.Imaging.ImageFormat.Tiff);
                    FileNameCount += 1;
                    stream.Dispose();
                }
                else
                {
                    FileStream stream = new FileStream(System.IO.Path.Combine(fileIconDir, FileIconName), FileMode.Create);
                    Bitmap icon = new Bitmap(System.Drawing.Icon.ExtractAssociatedIcon(myfile).ToBitmap());
                    icon.Save(stream, System.Drawing.Imaging.ImageFormat.Tiff);
                    stream.Dispose();
                }
            }

            WPFGridView.Items.Add(CreateGridViewItem(Path.Combine(fileIconDir + FileIconName), Path.GetFileName(myfile), myfile));


        }

        //add folder
        public void AddFolder(string directory)
        {
            //show gridView
            //gridviewhost.Visibility = Visibility.Visible;
            WPFGridView.Visibility = Visibility.Visible;
            EmptyNotice.Visibility = Visibility.Hidden;

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

                stream.Dispose();
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

                stream.Dispose();
            }

            //then do the controls and add it to the gridView
            WPFGridView.Items.Add(CreateGridViewItem(Path.Combine(folderIconDir + FolderIconName), new DirectoryInfo(directory).Name, directory));

        }

        private void AddWebsite(string url, string DisplayName)
        {
            //show gridView
            //gridviewhost.Visibility = Visibility.Visible;
            WPFGridView.Visibility = Visibility.Visible;
            EmptyNotice.Visibility = Visibility.Hidden;

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

            WPFGridView.Items.Add(CreateGridViewItem(Path.Combine(websiteIconDir + WebsiteIconFileName), DisplayName, "https://" + url));
        }

        #endregion

        #region Methods relating to ContextMenu of GridViewItems and its event handlers
        public ContextMenu CreateGridViewItemContextMenu(StackPanel stackPanel)
        {
            //init a new contextmenu
            ContextMenu menu = new ContextMenu();
            menu.Closed += Menu_Closed;

            //init menu flyout items
            MenuItem open = new MenuItem();
            MenuItem openfilelocation = new MenuItem();
            MenuItem remove = new MenuItem();

            //event handlers for the items
            open.Click += (s, args) => MenuItemOpen_Click(s, args, stackPanel.Tag.ToString());
            remove.Click += (s, args) => MenuItemRemove_Click(s, args, stackPanel);
            openfilelocation.Click += (s, args) => MenuItemOpenLocation_Click(s, args, stackPanel.Tag.ToString());

            //create symbolicons
            Wpf.Ui.Controls.SymbolIcon OpenItemIcon = new Wpf.Ui.Controls.SymbolIcon();
            OpenItemIcon.Symbol = Wpf.Ui.Common.SymbolRegular.OpenFolder24;
            Wpf.Ui.Controls.SymbolIcon OpenLocationIcon = new Wpf.Ui.Controls.SymbolIcon();
            OpenLocationIcon.Symbol = Wpf.Ui.Common.SymbolRegular.FolderOpen24;
            Wpf.Ui.Controls.SymbolIcon RemoveItemIcon = new Wpf.Ui.Controls.SymbolIcon();
            RemoveItemIcon.Symbol = Wpf.Ui.Common.SymbolRegular.Delete24;

            //next, we need to determine if the stackpanel is a file, folder, or website item.
            if (stackPanel.Tag.ToString().StartsWith("https://"))
            {
                //set properties and icons for the menuitems
                open.Icon = OpenItemIcon;
                open.Header = "Open website";

                remove.Icon = RemoveItemIcon;
                remove.Header = "Remove item from LauncherX";

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
                    //set properties and icons for the menuitems
                    open.Icon = OpenItemIcon;
                    open.Header = "Open";

                    openfilelocation.Icon = OpenLocationIcon;
                    openfilelocation.Header = "Open folder location";

                    remove.Icon = RemoveItemIcon;
                    remove.Header = "Remove item from LauncherX";

                    //add them to the menuflyout
                    menu.Items.Add(open);
                    menu.Items.Add(openfilelocation);
                    menu.Items.Add(remove);

                }
                else if (!attr.HasFlag(System.IO.FileAttributes.Directory))
                {
                    //this is a file
                    //set properties and icons for the menuitems
                    open.Icon = OpenItemIcon;
                    open.Header = "Open";

                    openfilelocation.Icon = OpenLocationIcon;
                    openfilelocation.Header = "Open file location";

                    remove.Icon = RemoveItemIcon;
                    remove.Header = "Remove item from LauncherX";

                    //add them to the menuflyout
                    menu.Items.Add(open);
                    menu.Items.Add(openfilelocation);
                    menu.Items.Add(remove);

                }
            }

            return menu;
        }

        private void Menu_Closed(object sender, RoutedEventArgs e)
        {
            //unselect the selected GridView Item
            WPFGridView.SelectedItem = null;
        }

        private void MenuItemOpen_Click(object sender, RoutedEventArgs e, string ItemToOpen)
        {
            try
            {
                //start the process
                Process.Start(ItemToOpen);
            }
            catch { }
        }

        private void MenuItemRemove_Click(object sender, RoutedEventArgs e, StackPanel stackPanel)
        {
            //remove the item
            WPFGridView.Items.Remove(stackPanel);

            if (WPFGridView.Items.Count == 0)
            {
                WPFGridView.Visibility = Visibility.Hidden;
                EmptyNotice.Visibility = Visibility.Visible;
            }
            else
            {
                WPFGridView.Visibility = Visibility.Visible;
                EmptyNotice.Visibility = Visibility.Hidden;
            }

        }

        private void MenuItemOpenLocation_Click(object sender, RoutedEventArgs e, string ItemToLook)
        {
            //open file location
            Process.Start("explorer.exe", "/select, " + ItemToLook);
        }

        #endregion

        #endregion

        #region Event handlers for the Open file, folder, and website buttons
        private async void OpenFileBtn_Click(object sender, RoutedEventArgs e)
        {
            //init open file dialog
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog() { DereferenceLinks = false };
            openFileDialog.Multiselect = true;

            //check if openfile dialog is ok
            if (openFileDialog.ShowDialog() == true)
            {
                LoadingDialog.Visibility = Visibility.Visible;

                await Task.Delay(100);

                //and then get the filename and YEET in into the grid view
                foreach (string myfile in openFileDialog.FileNames)
                {
                    AddFile(myfile);
                }

            }

            LoadingDialog.Visibility = Visibility.Hidden;
        }

        private async void OpenFolderBtn_Click(object sender, RoutedEventArgs e)
        {
            //init folderbrowsedialog
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.Multiselect = true;
            CommonFileDialogResult result = dialog.ShowDialog();

            //Add the folder
            if (result == CommonFileDialogResult.Ok)
            {
                LoadingDialog.Visibility = Visibility.Visible;

                await Task.Delay(100);

                //and then get the filename and YEET in into the grid view
                foreach (string myfile in dialog.FileNames)
                {
                    AddFolder(myfile);
                }
            }

            LoadingDialog.Visibility = Visibility.Hidden;
        }

        private void OpenWebsiteBtn_Click(object sender, RoutedEventArgs e)
        {
            //show add website dialog
            WebsiteDialog wbd = new WebsiteDialog();
            //event handler to add the website
            wbd.Closed += Wbd_Closed;
            wbd.ShowDialog();
        }

        private async void Wbd_Closed(object sender, EventArgs e)
        {
            if (websiteok == true)
            {
                LoadingDialog.Visibility = Visibility.Visible;

                await Task.Delay(100);

                //add the website
                AddWebsite(url, original_url);

                websiteok = false;
            }

            LoadingDialog.Visibility = Visibility.Hidden;
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
            size = Math.Sqrt(scale);

            try
            {
                //PUT UPDATE SETTINGS CODE HERE

                //---------------------------------------------------------------change scale
                //UPDATE SIZES

                //foreach loop
                foreach (StackPanel stackpanel in WPFGridView.Items)
                {
                    //update sizes
                    stackpanel.Width = size * 105;
                    stackpanel.Height = size * 95;

                    TextBlock textBlock = (TextBlock)stackpanel.Children[1];
                    Image image = (Image)stackpanel.Children[0];
                    textBlock.FontSize = size * 11;
                    image.Margin = new Thickness(size * 22.5, 5, size * 22.5, 0);
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

            //check every item in grid, and find the stackpanel with the textblock with the correct text, and then launch the app
            foreach (StackPanel stackpanel in WPFGridView.Items)
            {
                //textblock
                TextBlock textblock = (TextBlock)stackpanel.Children[1];

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

            Wpf.Ui.Controls.AutoSuggestBox autoSuggestBox = sender as Wpf.Ui.Controls.AutoSuggestBox;


            foreach (StackPanel stackpanel in WPFGridView.Items)
            {
                TextBlock textBlock = (TextBlock)stackpanel.Children[1];

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

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            //hide the suggestion list of the search box
            SearchBox.IsSuggestionListOpen = false;
        }

        #endregion

        #region Saving of items
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //stop timers
            themeupdater.Stop();

            //save the items by creating text documents----------------------------------------

            //delete files in directory(loading items directory)
            System.IO.DirectoryInfo di4 = new DirectoryInfo(loadDir);

            foreach (FileInfo file3 in di4.GetFiles())
            {
                file3.Delete();
            }

            foreach (StackPanel stackpanel in WPFGridView.Items)
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
                        TextBlock textBlock = (TextBlock)stackpanel.Children[1];
                        sw.WriteLine(textBlock.Text);
                    }

                }

                savename += 1;
            }

            //close application manually
            System.Windows.Application.Current.Shutdown();
        }
        #endregion

        #region Misc. Event Handlers
        private void MainWindow_Deactivated(object sender, EventArgs e)
        {
            //hide the suggestion list of the search box
            SearchBox.IsSuggestionListOpen = false;
        }

        private void Container_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //hide the suggestion list of the search box
            SearchBox.IsSuggestionListOpen = false;
        }

        #endregion
    }
}
