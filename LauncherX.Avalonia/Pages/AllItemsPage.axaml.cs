using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace LauncherX.Avalonia.Pages
{
    public partial class AllItemsPage : UserControl
    {
        public AllItemsPage()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
