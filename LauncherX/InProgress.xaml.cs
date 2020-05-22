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
    /// Interaction logic for InProgress.xaml
    /// </summary>
    public partial class InProgress
    {
        public InProgress()
        {
            InitializeComponent();

            //check and updtate theme
            CheckAndUpdateTheme();

            //create a new dispatcher timer
            DispatcherTimer themeupdater = new DispatcherTimer();

            //create a dispatcher timer to check for theme        
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

            //check light/dark mode, change colors accordingly
            if (is_light_mode == true)
            {
                //change window tint color
                this.TintColor = System.Windows.Media.Color.FromArgb(255, 255, 255, 255);
            }
            else if (is_light_mode == false)
            {
                //change window tint color
                this.TintColor = System.Windows.Media.Color.FromArgb(255, 0, 0, 0);
            }
        }
    }
}
