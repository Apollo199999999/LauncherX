using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Threading.Tasks;

namespace LauncherX.Avalonia.Pages
{
    public partial class FoldersPage : UserControl
    {
        //init controls from xaml
        //public static GridView FoldersGridView = new GridView();
        public ListBox FoldersGridView = new ListBox();

        public FoldersPage()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            //locate controls
            //FoldersGridView = this.FindControl<GridView>("FoldersGridView");
            FoldersGridView = this.FindControl<ListBox>("FoldersGridView");

            //configure controls
            FoldersGridView.SelectionChanged += FoldersGridView_SelectionChanged;
        }

        private async void FoldersGridView_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            //unselect the selected item
            await Task.Delay(500);
            FoldersGridView.SelectedItem = null;
        }
    }
}
