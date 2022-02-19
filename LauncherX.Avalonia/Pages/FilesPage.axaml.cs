using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Threading.Tasks;

namespace LauncherX.Avalonia.Pages
{
    public partial class FilesPage : UserControl
    {
        //init controls from xaml
        //public static GridView FilesGridView = new GridView();
        public ListBox FilesGridView = new ListBox();

        public FilesPage()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            //locate controls
            //FilesGridView = this.FindControl<GridView>("FilesGridView");
            FilesGridView = this.FindControl<ListBox>("FilesGridView");

            //configure controls
            FilesGridView.SelectionChanged += FilesGridView_SelectionChanged;
        }

        private async void FilesGridView_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            //unselect the selected item
            await Task.Delay(500);
            FilesGridView.SelectedItem = null;
        }
    }
}
