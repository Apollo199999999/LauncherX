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
using System.Windows.Shapes;
using static LauncherX.PublicVariables;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using Windows.UI.Xaml.Controls;
using System.Windows.Threading;
using System.Diagnostics;
using System.Net;

namespace LauncherX
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow
    {
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
                   Wpf.Ui.Appearance.ThemeType.Dark,     // Theme type
                   Wpf.Ui.Appearance.BackgroundType.Mica, // Background type
                   true                                   // Whether to change accents automatically
               );
            }

        }

        public SettingsWindow()
        {
            InitializeComponent();

            //set changeHeaderTextTextBox inital text to properties.settings.default.headertext
            changeHeaderTextTextBox.Text = Properties.Settings.Default.headerText;

            //make sure the textbox is not readonly
            changeHeaderTextTextBox.IsReadOnly = false;

            //check and update theme
            CheckAndUpdateTheme();

            //create a dispatcher timer to check for theme    
            //init a new dispatcher timer
            DispatcherTimer themeupdater = new DispatcherTimer();

            themeupdater.Interval = TimeSpan.FromMilliseconds(100);
            themeupdater.Tick += Themeupdater_Tick;
            themeupdater.Start();
        }

        private void Themeupdater_Tick(object sender, EventArgs e)
        {
            //check and update theme
            CheckAndUpdateTheme();
        }


        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void changeHeaderTextTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            //update headerText value from **publicvariables**
            headerText = changeHeaderTextTextBox.Text;

            //update the headerText from **Properties.Settings.Default**
            Properties.Settings.Default.headerText = changeHeaderTextTextBox.Text;

            //save settings
            Properties.Settings.Default.Save();
        }

        private void ScaleSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //update scale value
            scale = ScaleSlider.Value;

            //update settings
            Properties.Settings.Default.scale = ScaleSlider.Value;
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();
        }

        private void ScaleSlider_Loaded(object sender, RoutedEventArgs e)
        {
            ScaleSlider.Value = Properties.Settings.Default.scale;
            //subscribe to valuechanged event handler
            ScaleSlider.ValueChanged += ScaleSlider_ValueChanged;
        }

        private void ScaleSlider_Unloaded(object sender, RoutedEventArgs e)
        {
            //unsubscribe to valuechanged event handler
            ScaleSlider.ValueChanged -= ScaleSlider_ValueChanged;
        }
    }
}
