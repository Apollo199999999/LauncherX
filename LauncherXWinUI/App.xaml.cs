using LauncherXWinUI.Classes;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json.Nodes;
using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LauncherXWinUI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Tray icon for LauncherX to run in background
        /// </summary>
        private static TrayIcon AppTrayIcon;

        /// <summary>
        /// MainWindow instance
        /// </summary>
        public static MainWindow MainWindow;

        /// <summary>
        /// To register system hot keys to activate LauncherX
        /// </summary>
        public static HotKeyHook ActivationHotKeyHook;

        /// <summary>
        /// Initializes the singleton application object. This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();

            // Configure FilePersistence for WinUIEx to save Window position in Unpackaged apps https://github.com/dotMorten/WinUIEx/issues/61
            WinUIEx.WindowManager.PersistenceStorage = new FilePersistence(Path.Combine(UserSettingsClass.SettingsDir, "windowPlace.json"));

            // Create an instance of HotKeyHook to watch out for activation shortcuts,
            // and create a new event handler for when the activation shortcut (hotkey) is triggered
            ActivationHotKeyHook = new HotKeyHook();
            ActivationHotKeyHook.KeyPressed += ActivationHotKeyHook_KeyPressed;
        }
       
        /// <summary>
        /// Creates a new MainWindow (if applicable) and activates the MainWindow
        /// </summary>
        public void GetMainWindow()
        {
            if (MainWindow == null)
            {
                MainWindow = new MainWindow();
                MainWindow.Activate();
                return;
            }

            // Try to just activate the window, if it fails, create a new instance
            try
            {
                MainWindow.Activate();
            }
            catch
            {
                MainWindow = new MainWindow();
                MainWindow.Activate();
            }
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            // Register a tray icon
            AppTrayIcon = new TrayIcon(1, "Resources\\icon.ico", "LauncherX");
            AppTrayIcon.IsVisible = true;
            AppTrayIcon.Selected += (s, e) => GetMainWindow();
            // A bit messy, but its just UI stuff, so who cares?
            AppTrayIcon.ContextMenu += (w, e) =>
            {
                var flyout = new MenuFlyout();
                flyout.Items.Add(new MenuFlyoutItem() 
                { 
                    Text = "Open LauncherX",
                    Height = 36,
                    Icon = new FontIcon() 
                    { 
                        Glyph="\uE8A7"
                    } 
                });
                ((MenuFlyoutItem)flyout.Items[0]).Click += (s, e) => GetMainWindow();

                flyout.Items.Add(new MenuFlyoutItem() 
                { 
                    Text = "Quit LauncherX", 
                    Height = 36,
                    Icon = new FontIcon()
                    {
                        Glyph = "\uE711"
                    }
                });
                ((MenuFlyoutItem)flyout.Items[1]).Click += (s, e) =>
                {
                    ExitApplication();
                };
                e.Flyout = flyout;
            };

            // Launch MainWindow
            GetMainWindow();
        }

        // Activate LauncherX when hot key is triggered
        private void ActivationHotKeyHook_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            GetMainWindow();
        }

        /// <summary>
        /// Exits the application and cleans up the necessary objects
        /// </summary>
        public static void ExitApplication()
        {
            AppTrayIcon.Dispose();
            ActivationHotKeyHook.Dispose();
            Application.Current.Exit();
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int GetCurrentPackageFullName(ref int packageFullNameLength, System.Text.StringBuilder packageFullName);

        private class FilePersistence : IDictionary<string, object>
        {
            private readonly Dictionary<string, object> _data = new Dictionary<string, object>();
            private readonly string _file;

            public FilePersistence(string filename)
            {
                _file = filename;
                try
                {
                    if (File.Exists(filename))
                    {
                        var jo = System.Text.Json.Nodes.JsonObject.Parse(File.ReadAllText(filename)) as JsonObject;
                        foreach (var node in jo)
                        {
                            if (node.Value is JsonValue jvalue && jvalue.TryGetValue<string>(out string value))
                                _data[node.Key] = value;
                        }
                    }
                }
                catch { }
            }
            private void Save()
            {
                JsonObject jo = new JsonObject();
                foreach (var item in _data)
                {
                    if (item.Value is string s) // In this case we only need string support. TODO: Support other types
                        jo.Add(item.Key, s);
                }
                File.WriteAllText(_file, jo.ToJsonString());
            }
            public object this[string key] { get => _data[key]; set { _data[key] = value; Save(); } }

            public ICollection<string> Keys => _data.Keys;

            public ICollection<object> Values => _data.Values;

            public int Count => _data.Count;

            public bool IsReadOnly => false;

            public void Add(string key, object value)
            {
                _data.Add(key, value); Save();
            }

            public void Add(KeyValuePair<string, object> item)
            {
                _data.Add(item.Key, item.Value); Save();
            }

            public void Clear()
            {
                _data.Clear(); Save();
            }

            public bool Contains(KeyValuePair<string, object> item) => _data.Contains(item);

            public bool ContainsKey(string key) => _data.ContainsKey(key);

            public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) => throw new NotImplementedException(); // TODO

            public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => throw new NotImplementedException(); // TODO

            public bool Remove(string key) => throw new NotImplementedException(); // TODO

            public bool Remove(KeyValuePair<string, object> item) => throw new NotImplementedException(); // TODO

            public bool TryGetValue(string key, [MaybeNullWhen(false)] out object value) => throw new NotImplementedException(); // TODO

            IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException(); // TODO
        }
    }
}
