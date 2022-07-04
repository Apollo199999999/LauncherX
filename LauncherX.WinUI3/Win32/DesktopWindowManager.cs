using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Security;

namespace LauncherX.WinUI3.Win32
{
    public static class DesktopWindowManager
    {
        [SecurityCritical]
        [DllImport("dwmapi.dll", SetLastError = false, ExactSpelling = true)]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, uint dwAttribute, [In] IntPtr pvAttribute, int cbAttribute);

        [ComVisible(true)]
        public enum BackdropType
        {
            Default = 0,
            None = 1,
            Mica = 2,
            Acrylic = 3,
            Tabbed = 4,
        }

        /// <summary>
        /// Type of system backdrop to be rendered by DWM
        /// </summary>
        public enum DWM_SYSTEMBACKDROP_TYPE : uint
        {
            DWMSBT_AUTO = 0,

            /// <summary>
            /// no backdrop
            /// </summary>
            DWMSBT_NONE = 1,

            /// <summary>
            /// Use tinted blurred wallpaper backdrop (Mica)
            /// </summary>
            DWMSBT_MAINWINDOW = 2,

            /// <summary>
            /// Use Acrylic backdrop
            /// </summary>
            DWMSBT_TRANSIENTWINDOW = 3,

            /// <summary>
            /// Use blurred wallpaper backdrop
            /// </summary>
            DWMSBT_TABBEDWINDOW = 4
        }

        private const uint DWMWA_MICA = 1029;
        private const uint DWMWA_IMMERSIVE_DARK_MODE = 20;
        private const uint DWMWA_SYSTEMBACKDROP_TYPE = 38;
        private const uint DWMWA_CAPTION_COLOR = 35;
        private const uint DWMWA_TEXT_COLOR = 36;

        /// <summary>
        /// Check whether Windows build is 19041 or higher, that supports <see cref="SetImmersiveDarkMode(IntPtr, bool)"/>.
        /// </summary>
        public static bool IsImmersiveDarkModeSupported { get; } =
            Environment.OSVersion.Version.Build >= 19041;

        /// <summary>
        /// Check whether Windows build is 22000 or higher, that supports <see cref="SetMica(IntPtr, bool)"/>.
        /// </summary>
        public static bool IsUndocumentedMicaSupported { get; } =
            Environment.OSVersion.Version.Build >= 22000;

        /// <summary>
        /// Check wether Windows Windows build is 22523 or higher, that supports <see cref="SetBackdropType(IntPtr, DWM_SYSTEMBACKDROP_TYPE)"/>
        /// </summary>
        public static bool IsBackdropTypeSupported { get; } =
            Environment.OSVersion.Version.Build >= 22523;

        /// <summary>
        /// Enable Mica on target window with <see cref="SetMica(IntPtr, bool)"/> or <see cref="SetBackdropType(IntPtr, DWM_SYSTEMBACKDROP_TYPE)"/> if supported.
        /// </summary>
        public static void EnableMicaIfSupported(IntPtr hWnd)
        {
            if (IsBackdropTypeSupported)
            {
                SetBackdropType(hWnd, BackdropType.Mica);
            }
            else if (IsUndocumentedMicaSupported)
            {
                SetMica(hWnd, true);
            }
        }

        /// <summary>
        /// Enable or Disable Mica on target window
        /// Supported on Windows builds from 22000 to 22523. It doesn't work on 22523, use <see cref="SetBackdropType(IntPtr, DWM_SYSTEMBACKDROP_TYPE)"/> instead.
        /// </summary>
        public static void SetMica(IntPtr hWnd, bool state)
        {
            var value = GCHandle.Alloc(state ? 1 : 0, GCHandleType.Pinned);
            var result = DwmSetWindowAttribute(hWnd, DWMWA_MICA, value.AddrOfPinnedObject(), sizeof(int));
            value.Free();
            if (result != 0)
            {
                throw Marshal.GetExceptionForHR(result);
            }
        }

        /// <summary>
        /// Set backdrop type on target window
        /// Requires Windows build 22523 or higher.
        /// </summary>
        public static void SetBackdropType(IntPtr hWnd, DWM_SYSTEMBACKDROP_TYPE backdropType)
        {
            var value = GCHandle.Alloc((uint)backdropType, GCHandleType.Pinned);
            var result = DwmSetWindowAttribute(hWnd, DWMWA_SYSTEMBACKDROP_TYPE, value.AddrOfPinnedObject(), sizeof(uint));
            value.Free();
            if (result != 0)
            {
                throw Marshal.GetExceptionForHR(result);
            }

        }

        public static void SetBackdropType(IntPtr target, BackdropType backdropType)
        {
            SetBackdropType(target, (DWM_SYSTEMBACKDROP_TYPE)backdropType);
        }

        /// <summary>
        /// Enable or disable immersive dark mode.
        /// Requires Windows build 19041 or higher.
        /// </summary>
        public static void SetImmersiveDarkMode(IntPtr target, bool state)
        {
            var value = GCHandle.Alloc(state ? 1 : 0, GCHandleType.Pinned);
            var result = DwmSetWindowAttribute(target, DWMWA_IMMERSIVE_DARK_MODE, value.AddrOfPinnedObject(), sizeof(int));
            value.Free();
            if (result != 0)
            {
                throw Marshal.GetExceptionForHR(result);
            }
        }
    }
}
