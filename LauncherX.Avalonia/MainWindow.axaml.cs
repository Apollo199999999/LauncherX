using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using FluentAvalonia.UI.Controls;
using LauncherX.Avalonia.Pages;
using Button = Avalonia.Controls.Button;

namespace LauncherX.Avalonia
{
    public partial class MainWindow : CoreWindow
    {
        //init controls from xaml
        Button AddWebsiteBtn = new Button();
        Button AddFolderBtn = new Button();
        Button AddFileBtn = new Button();

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

            //locate controls
            AddWebsiteBtn = this.FindControl<Button>("AddWebsiteBtn");
            AddFolderBtn = this.FindControl<Button>("AddFolderBtn");
            AddFileBtn = this.FindControl<Button>("AddFileBtn");

            //configure controls

            //set the margin for the three buttons
            //AddWebsiteBtn.Margin = new Thickness(0, 0, 0, 0);
            //AddFolderBtn.Margin = new Thickness(0, (AddWebsiteBtn.Width + 10), 0, 0);
            //AddFileBtn.Margin = new Thickness(0, (AddWebsiteBtn.Width + 10 + AddFolderBtn.Width + 10), 0, 0);

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
