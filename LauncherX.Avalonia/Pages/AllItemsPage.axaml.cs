using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using FluentAvalonia.UI.Controls;

namespace LauncherX.Avalonia.Pages
{
    public partial class AllItemsPage : UserControl
    {
        //init controls from xaml
        //public GridView AllItemsGridView = new GridView();

        public AllItemsPage()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            //locate controls
            //AllItemsGridView = this.FindControl<GridView>("AllItemsGridView");
        }
    }
}
