using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using SettingsUI;
using SettingsUI.Helpers;
using LauncherX.WinUI3.Frames;
using Windows.UI.ViewManagement;
using Microsoft.UI;
using Microsoft.Windows.ApplicationModel;
using Microsoft.UI.Windowing;
using LauncherX.WinUI3.Win32;

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
        public void TryEnableMica()
        {
            //Enable Mica if supported
            if (Microsoft.UI.Composition.SystemBackdrops.MicaController.IsSupported() == true)
            {
                //Set background Mica
                SystemBackdropsHelper systemBackdropsHelper = new SystemBackdropsHelper(this);
                systemBackdropsHelper.SetBackdrop(BackdropType.Mica);

                //Set titlebar Mica
                DesktopWindowManager.EnableMicaIfSupported(hWnd);
            }
            else
            {
                //Set the ParentControlsGrid background
                ParentControlsGrid.Background = (Brush)Application.Current.Resources["ApplicationPageBackgroundThemeBrush"];
            }
           
            //Try set titlebar color
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

        private void MainWindow_ThemeChanged(FrameworkElement sender, object args)
        {
            //Try Enable Mica
            TryEnableMica();
        }

        private void ParentControlsGrid_Loaded(object sender, RoutedEventArgs e)
        {
            //CONFIGURE WINDOW
            //Set Window icon
            appWindow.SetIcon("Assets\\icon.ico");

            //Set Window titlebar text
            this.Title = "LauncherX - Create and organise collections of files, folders, and websites";

            //Enable Mica
            TryEnableMica();

            //Set window size and minimum window size
            WindowHelper.SetWindowSize(this, 900, 600);
            WindowHelper.RegisterWindowMinMax(this);
            WindowHelper.MinWindowWidth = 600;
            WindowHelper.MinWindowHeight = 400;
        }

        private async void AddCategoryBtn_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog();
            dialog.XamlRoot = this.Content.XamlRoot;
            dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            dialog.Width = 350;
            dialog.Title = "Create new collection";
            dialog.PrimaryButtonText = "Create Collection";
            dialog.CloseButtonText = "Cancel";
            dialog.DefaultButton = ContentDialogButton.Primary;
            dialog.Content = new CreateCollectionDialogContent();

            var result = await dialog.ShowAsync();

        }
    }
}
