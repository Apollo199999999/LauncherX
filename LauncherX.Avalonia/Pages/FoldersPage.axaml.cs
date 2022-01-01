using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace LauncherX.Avalonia.Pages
{
    public partial class FoldersPage : UserControl
    {
        public FoldersPage()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
