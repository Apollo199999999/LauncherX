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

namespace LauncherX
{
    /// <summary>
    /// Interaction logic for WebsiteDialog.xaml
    /// </summary>
    public partial class WebsiteDialog 
    {
        public WebsiteDialog()
        {
            InitializeComponent();
        }

        private void urlBoxHost_ChildChanged(object sender, EventArgs e)
        {
            var urlBox = urlBoxHost.Child as Windows.UI.Xaml.Controls.TextBox;
            urlBox.PlaceholderText = "Enter URL here";

            urlBoxHost.ChildChanged -= urlBoxHost_ChildChanged;
        }

        private void OkBtn_ChildChanged(object sender, EventArgs e)
        {
            var OkBtn = OkBtnHost.Child as Windows.UI.Xaml.Controls.Button;
            OkBtn.Content = "OK";
            OkBtn.Click += OkBtn_Click;

            OkBtnHost.ChildChanged -= OkBtn_ChildChanged;
        }

        private void OkBtn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var urlBox = urlBoxHost.Child as Windows.UI.Xaml.Controls.TextBox;

            if (urlBox.Text.StartsWith("https://"))
            {
                //remove https:// and set websiteok to true to activate the method later
                original_url = urlBox.Text;
                url = urlBox.Text.Remove(0, 8);
                websiteok = true;
                Close();
            }
            else if (urlBox.Text.StartsWith("http://"))
            {
                //remove http:// and set websiteok to true to activate the method later
                original_url = urlBox.Text;
                url = urlBox.Text.Remove(0, 7);
                websiteok = true;
                Close();
            }
            else
            {
                //assign to the url variable and set website ok to true
                original_url = urlBox.Text;
                url = urlBox.Text;
                websiteok = true;
                Close();
            }
        }
    }
}
