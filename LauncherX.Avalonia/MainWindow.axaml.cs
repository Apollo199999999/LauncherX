using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using FluentAvalonia.UI.Controls;
using LauncherX.Avalonia.Pages;

namespace LauncherX.Avalonia
{
    public partial class MainWindow : CoreWindow
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public async void AddWebsiteBtn_Click(object sender, RoutedEventArgs e)
        {
            //show a contentdialog with a textbox to enter the website URL
            
            //init contentdialog
            ContentDialog WebsiteDialog = new ContentDialog();
            WebsiteDialog.Title = "Add a website";
            WebsiteDialog.PrimaryButtonText = "OK";
            WebsiteDialog.CloseButtonText = "Cancel";
            WebsiteDialog.DefaultButton = ContentDialogButton.Primary;
            WebsiteDialog.Content = new WebsiteDialogContentPage();

            var result = await WebsiteDialog.ShowAsync();

        }
    }
}
