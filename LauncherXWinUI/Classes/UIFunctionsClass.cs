using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

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
    }
}
