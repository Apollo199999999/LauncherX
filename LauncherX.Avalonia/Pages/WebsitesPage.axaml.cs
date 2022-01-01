using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace LauncherX.Avalonia.Pages
{
    public partial class WebsitesPage : UserControl
    {
        public WebsitesPage()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
