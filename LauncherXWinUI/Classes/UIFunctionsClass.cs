using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;
using System.Runtime.InteropServices;
using WinUIEx;

namespace LauncherXWinUI.Classes
{
    /// <summary>
    /// Class that handles all methods and variables relating to controlling the user interface of LauncherX
    /// </summary>
    public static class UIFunctionsClass
    {
        

        /// <summary>
        /// Method that sets the window background material (Mica, Acrylic, or Solid Color) of a window
        /// </summary>
        /// <param name="window">Window to apply background to</param>
        /// <param name="gridFallbackBrush">Fallback SolidColorBrush in the XAML of the window</param>

        public static void SetWindowBackground(Window window, SolidColorBrush gridFallbackBrush)
        {
            // Configure either Mica or Acrylic background, depending on which is available
            if (Microsoft.UI.Composition.SystemBackdrops.MicaController.IsSupported())
            {
                MicaBackdrop micaBackdrop = new MicaBackdrop();
                micaBackdrop.Kind = Microsoft.UI.Composition.SystemBackdrops.MicaKind.Base;
                window.SystemBackdrop = micaBackdrop;

            }
            else if (Microsoft.UI.Composition.SystemBackdrops.DesktopAcrylicController.IsSupported())
            {
                DesktopAcrylicBackdrop DesktopAcrylicBackdrop = new DesktopAcrylicBackdrop();
                window.SystemBackdrop = DesktopAcrylicBackdrop;
                gridFallbackBrush.Opacity = 0.2;
            }
            else
            {
                // Load fallback background colour
                gridFallbackBrush.Opacity = 1.0;
            }
        }

        /// <summary>
        /// Sets the icon of a window to use the LauncherX ico file
        /// </summary>
        /// <param name="window">Window to set the icon of</param>
        public static void SetWindowLauncherXIcon(Window window)
        {
            AppWindow appWindow = window.AppWindow;
            appWindow.SetIcon(@"Resources\icon.ico");
        }

        /// <summary>
        /// Method that shows a window as a modal window, similar to ShowDialog() in WPF
        /// </summary>
        /// <param name="modalWindow">Window to show as the modal window</param>
        /// <param name="parentWindow">Parent window of the modal window</param>
        public static void CreateModalWindow(Window modalWindow, Window parentWindow)
        {
            // Disable parent window using Win32 API
            Shell32.EnableWindow(WinRT.Interop.WindowNative.GetWindowHandle(parentWindow), false);
            // Enable parent window when the modalWindow is closed
            modalWindow.Closed += (s, e) => Shell32.EnableWindow(WinRT.Interop.WindowNative.GetWindowHandle(parentWindow), true);
            modalWindow.Activate();
        }

    }
}
