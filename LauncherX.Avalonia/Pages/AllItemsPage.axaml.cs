using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Threading.Tasks;
using static LauncherX.Avalonia.PublicVariables;

namespace LauncherX.Avalonia.Pages
{
    public partial class AllItemsPage : UserControl
    {
        //init controls from xaml
        //public static GridView AllItemsGridView = new GridView();
        public ListBox AllItemsGridView = new ListBox();

        public AllItemsPage()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            //locate controls
            //AllItemsGridView = this.FindControl<GridView>("AllItemsGridView");
            AllItemsGridView = this.FindControl<ListBox>("AllItemsGridView");

            //configure controls
            AllItemsGridView.SelectionChanged += AllItemsGridView_SelectionChanged;

        }

        private async void AllItemsGridView_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            //unselect the selected item
            await Task.Delay(500);
            AllItemsGridView.SelectedItem = null;
        }
    }
}
