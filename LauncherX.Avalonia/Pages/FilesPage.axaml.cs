using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace LauncherX.Avalonia.Pages
{
    public partial class FilesPage : UserControl
    {
        public FilesPage()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
