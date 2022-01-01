using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace LauncherX.Avalonia.Pages
{
    public partial class WebsiteDialogContentPage : UserControl
    {
        public WebsiteDialogContentPage()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
