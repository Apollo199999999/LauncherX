using Microsoft.UI.Text;
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
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LauncherXWinUI.Controls
{
    public sealed partial class AboutSectionControl : UserControl
    {
        public AboutSectionControl()
        {
            this.InitializeComponent();
        }

        private void VersionText_Loaded(object sender, RoutedEventArgs e)
        {
            // Update the version string
            string versionString = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            // Do not display the last .0
            VersionText.Text = versionString.Substring(0, versionString.Length - 2);
        }
    }
}
