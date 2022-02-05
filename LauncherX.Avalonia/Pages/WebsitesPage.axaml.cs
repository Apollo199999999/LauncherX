using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using FluentAvalonia.UI.Controls;

namespace LauncherX.Avalonia.Pages
{
    public partial class WebsitesPage : UserControl
    {
        //init controls from xaml
        //public GridView WebsitesGridView = new GridView();
        public static ListBox WebsitesGridView = new ListBox();

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
        }
    }
}
