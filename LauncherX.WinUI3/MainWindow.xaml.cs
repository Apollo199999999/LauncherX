using LauncherX.WinUI3.Dialogs;
using LauncherX.WinUI3.Win32;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using SettingsUI.Helpers;
using System;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LauncherX.WinUI3
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        //Initialise AppWindow
        public AppWindow appWindow;

        //Intialise Window Handle
        public IntPtr hWnd;

        //Initialise 

        public MainWindow()
        {
            this.InitializeComponent();

            // Retrieve the window handle (HWND) of the current WinUI 3 window.
            hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);

            //load AppWindow
            var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
            appWindow = AppWindow.GetFromWindowId(windowId);

            //Theme changed event handler
            ((FrameworkElement)this.Content).ActualThemeChanged += MainWindow_ThemeChanged;
        }

        //ALL FUNCTIONS GO HERE
        public void TrySetModernWindowStyles()
        {
            //Enable Mica if supported
            if (Microsoft.UI.Composition.SystemBackdrops.MicaController.IsSupported() == true)
            {
                //Set background Mica
                SystemBackdropsHelper systemBackdropsHelper = new SystemBackdropsHelper(this);
                systemBackdropsHelper.SetBackdrop(BackdropType.Mica);
            }
            else
            {
                //Set the ParentControlsGrid background
                ParentControlsGrid.Background = (Brush)Application.Current.Resources["ApplicationPageBackgroundThemeBrush"];
            }

            //Set custom titlebar if supported
            if (AppWindowTitleBar.IsCustomizationSupported() == true)
            {
                //Set modern titlebar style
                //Show the custom titlebar
                AppTitleBar.Visibility = Visibility.Visible;
                ParentControlsGrid.Margin = new Thickness(0, 40, 0, 0);

                //Customise AppWindow Titlebar
                appWindow.TitleBar.ExtendsContentIntoTitleBar = true;
                appWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
                appWindow.TitleBar.ButtonForegroundColor = (Color)Application.Current.Resources["TextFillColorPrimary"];
                appWindow.TitleBar.ButtonHoverBackgroundColor = (Color)Application.Current.Resources["ControlFillColorSecondary"];
                appWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                appWindow.TitleBar.ButtonPressedBackgroundColor = (Color)Application.Current.Resources["ControlFillColorDefault"];

            }
            else
            {
                //Try set titlebar color based on user theme
                try
                {
                    //Set titlebar Light/Dark mode based on app theme
                    if (Application.Current.RequestedTheme == ApplicationTheme.Light)
                    {
                        DesktopWindowManager.SetImmersiveDarkMode(hWnd, false);
                    }
                    else if (Application.Current.RequestedTheme == ApplicationTheme.Dark)
                    {
                        DesktopWindowManager.SetImmersiveDarkMode(hWnd, true);
                    }
                }
                catch { }
            }

        }

        private void MainWindow_ThemeChanged(FrameworkElement sender, object args)
        {
            //Try set modern window styles
            TrySetModernWindowStyles();
        }

        private void ParentControlsGrid_Loaded(object sender, RoutedEventArgs e)
        {
            //CONFIGURE WINDOW
            //Set Window icon
            appWindow.SetIcon("Assets\\icon.ico");

            //Set Window titlebar text
            this.Title = "LauncherX - Create and organise collections of files, folders, and websites";

            //Try set modern window styles
            TrySetModernWindowStyles();

            //Set window size and minimum window size
            WindowHelper.SetWindowSize(this, 900, 600);
            WindowHelper.RegisterWindowMinMax(this);
            WindowHelper.MinWindowWidth = 900;
            WindowHelper.MinWindowHeight = 600;
        }

        private async void AddCategoryBtn_Click(object sender, RoutedEventArgs e)
        {
            //Launch a ContentDialog that prompts the user to create a new collection
            ContentDialog createCollectionDialog = new ContentDialog();
            createCollectionDialog.XamlRoot = ParentControlsGrid.XamlRoot;
            createCollectionDialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            createCollectionDialog.Width = 350;
            createCollectionDialog.Title = "Create new collection";
            createCollectionDialog.PrimaryButtonText = "Create Collection";
            createCollectionDialog.CloseButtonText = "Cancel";
            createCollectionDialog.DefaultButton = ContentDialogButton.Primary;
            createCollectionDialog.Content = new CreateCollectionDialog();

            var result = await createCollectionDialog.ShowAsync();

        }

        private async void SettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            //Launch a ContentDialog that shows app settings
            SettingsDialog settingsDialog = new SettingsDialog();
            settingsDialog.XamlRoot = ParentControlsGrid.XamlRoot;
            var result = await settingsDialog.ShowAsync();
        }
    }
}
