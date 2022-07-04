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
        public MainWindow()
        {
            this.InitializeComponent();
        }

        public void TryEnableMica()
        {
            //Try and enable Mica and set a Mica titlebar (may not be compatible with diff Windows versions)
            try
            {
                //Enable Mica
                SystemBackdropsHelper systemBackdropsHelper = new SystemBackdropsHelper(this);
                systemBackdropsHelper.SetBackdrop(BackdropType.Mica);

                //Try enable Mica titlebar
                try
                {
                    // Retrieve the window handle (HWND) of the current WinUI 3 window.
                    IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
                    DesktopWindowManager.EnableMicaIfSupported(hWnd);

                    //Set titlebar Light/Dark mode based on app theme
                    if (Application.Current.RequestedTheme == ApplicationTheme.Light)
                    {
                        DesktopWindowManager.SetImmersiveDarkMode(hWnd, false);
                    }
                    else
                    {
                        DesktopWindowManager.SetImmersiveDarkMode(hWnd, true);
                    }
                } catch { }
            }
            catch
            {
                //Set the ParentControlsGrid background
                ParentControlsGrid.Background = (Brush)Application.Current.Resources["SolidBackgroundFillColorBaseBrush"];
            }
        }
        private void ParentControlsGrid_Loaded(object sender, RoutedEventArgs e)
        {
            //CONFIGURE WINDOW
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
            dialog.Title = "Create new collection";
            dialog.PrimaryButtonText = "Create Collection";
            dialog.CloseButtonText = "Cancel";
            dialog.DefaultButton = ContentDialogButton.Primary;
            dialog.Content = new CreateCollectionDialogContent();

            var result = await dialog.ShowAsync();

        }
    }
}
