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
        

        public SettingsWindow()
        {
            InitializeComponent();

            //set the button reveal style
            var buttonstyle = (Style)App.Current.Resources["ButtonRevealStyle"];
            CloseButton.Style = buttonstyle;
            VisitGithub.Style = buttonstyle;

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
            SolidColorBrush lightTheme = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 255, 255));
            SolidColorBrush darkTheme = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 0, 0));

            //check light/dark mode, change colors accordingly
            if (is_light_mode == true)
            {
                changeHeaderTextTextBox.Background = lightTheme;
                changeHeaderTextTextBox.Foreground = darkTheme;

                //change window tint color
                this.TintColor = System.Windows.Media.Color.FromArgb(255, 255, 255, 255);
            }
            else if (is_light_mode == false)
            {
                changeHeaderTextTextBox.Background = darkTheme;
                changeHeaderTextTextBox.Foreground = lightTheme;

                //change window tint color
                this.TintColor = System.Windows.Media.Color.FromArgb(255, 0, 0, 0);
            }
        }

        private void scalescrollhost_ChildChanged(object sender, EventArgs e)
        {
            //init slider and properties
            var scrollscale = scalescrollhost.Child as Windows.UI.Xaml.Controls.Slider;

            scrollscale.Value = Properties.Settings.Default.scale;

            scrollscale.Maximum = 2.5;
            scrollscale.Minimum = 0.7;
            scrollscale.StepFrequency = 0.1;
            scrollscale.ValueChanged += Scrollscale_ValueChanged;

            //unsubscribe this event
            scalescrollhost.ChildChanged -= scalescrollhost_ChildChanged;

        }

        private void Scrollscale_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            //init slider
            var scrollscale = scalescrollhost.Child as Windows.UI.Xaml.Controls.Slider;

            //update scale value
            scale = scrollscale.Value;

            //update settings
            Properties.Settings.Default.scale = scale;
            Properties.Settings.Default.Save();
        }

        private void AcrylicWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
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

        private void VisitGithub_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //get the github respository link from the pastebin link
                //github respository link is a pastebin link
                var url = "https://pastebin.com/raw/kjLK23F0";

                //init a webclient
                WebClient client = new WebClient();

                //download all text from the pastebin raw link
                string reply = client.DownloadString(url);

                //start the process
                Process.Start(reply);
            }
            catch
            {
                //show an error message
                System.Windows.MessageBox.Show("Unable to open the github respository. Check that you are connected " +
                    "to the internet, and try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
