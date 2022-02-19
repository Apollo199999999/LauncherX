using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Threading.Tasks;

namespace LauncherX.Avalonia.Pages
{
    public partial class WebsitesPage : UserControl
    {
        //init controls from xaml
        //public static GridView WebsitesGridView = new GridView();
        public ListBox WebsitesGridView = new ListBox();


        public WebsitesPage()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            //locate controls
            //WebsitesGridView = this.FindControl<GridView>("WebsitesGridView");
            WebsitesGridView = this.FindControl<ListBox>("WebsitesGridView");

            //configure controls
            WebsitesGridView.SelectionChanged += WebsitesGridView_SelectionChanged;
        }

        private async void WebsitesGridView_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            //unselect the selected item
            await Task.Delay(500);
            WebsitesGridView.SelectedItem = null;

        }
    }
}
