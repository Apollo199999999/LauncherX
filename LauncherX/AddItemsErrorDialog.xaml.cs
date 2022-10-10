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
using System.Windows.Threading;

namespace LauncherX
{
    /// <summary>
    /// Interaction logic for AddItemsErrorDialog.xaml
    /// </summary>
    public partial class AddItemsErrorDialog
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

        public AddItemsErrorDialog()
        {
            InitializeComponent();

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

        private void button_Click(object sender, RoutedEventArgs e)
        {
            //close the window
            this.Close();
        }
    }
}
