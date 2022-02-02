using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace LauncherX.Avalonia.Pages
{
    public partial class WebsiteDialogContentPage : UserControl
    {
        //init controls from xaml
        public TextBox UrlTextBox = new TextBox();

        public WebsiteDialogContentPage()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            //locate controls
            UrlTextBox = this.FindControl<TextBox>("UrlTextBox");
        }
    }
}