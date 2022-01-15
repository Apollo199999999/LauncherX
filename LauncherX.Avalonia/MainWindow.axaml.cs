using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using FluentAvalonia.Styling;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Navigation;
using LauncherX.Avalonia.Pages;
using static LauncherX.Avalonia.PublicVariables;
using Button = Avalonia.Controls.Button;

namespace LauncherX.Avalonia
{
    public partial class MainWindow : Window
    {
        //init controls from xaml
        public ExperimentalAcrylicBorder MainWindowMicaMaterial = new ExperimentalAcrylicBorder();
        public TextBlock HeaderText = new TextBlock();
        public Button SettingsBtn = new Button();
        public Button AddWebsiteBtn = new Button();
        public Button AddFolderBtn = new Button();
        public Button AddFileBtn = new Button();
        public Frame ContentFrame = new Frame();
        public NavigationView NavView = new NavigationView();
        public NavigationViewItem AllItemsItem = new NavigationViewItem();
        public NavigationViewItem FilesItem = new NavigationViewItem();
        public NavigationViewItem FoldersItem = new NavigationViewItem();
        public NavigationViewItem WebsitesItem = new NavigationViewItem();


        //FUNCTIONS:

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
            MainWindowMicaMaterial = this.FindControl<ExperimentalAcrylicBorder>("MainWindowMicaMaterial");
            HeaderText = this.FindControl<TextBlock>("HeaderText");
            SettingsBtn = this.FindControl<Button>("SettingsBtn");
            AddWebsiteBtn = this.FindControl<Button>("AddWebsiteBtn");
            AddFolderBtn = this.FindControl<Button>("AddFolderBtn");
            AddFileBtn = this.FindControl<Button>("AddFileBtn");
            ContentFrame = this.FindControl<Frame>("ContentFrame");
            NavView = this.FindControl<NavigationView>("NavView");
            AllItemsItem = this.FindControl<NavigationViewItem>("AllItemsItem");
            FilesItem = this.FindControl<NavigationViewItem>("FilesItem");
            FoldersItem = this.FindControl<NavigationViewItem>("FoldersItem");
            WebsitesItem = this.FindControl<NavigationViewItem>("WebsitesItem");

            //configure controls

            //set the NavView SelectedItem manually and navigate to that page
            NavView.SelectedItem = AllItemsItem;
            ContentFrame.Navigate(typeof(AllItemsPage));

            //all event handlers go here
            SettingsBtn.Click += SettingsBtn_Click;
            AddWebsiteBtn.Click += AddWebsiteBtn_Click;
            NavView.SelectionChanged += NavView_SelectionChanged;

            //get the system theme (string SysTheme is from PublicVariables)
            var thm = AvaloniaLocator.Current.GetService<FluentAvaloniaTheme>();
            SysTheme = thm.RequestedTheme;


        }

        private async void AddWebsiteBtn_Click(object? sender, RoutedEventArgs e)
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

        private void SettingsBtn_Click(object? sender, RoutedEventArgs e)
        {
            //when the settings button is clicked, configure and open the settings window
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.HeaderTextBox.Text = HeaderText.Text;
            settingsWindow.VersionText.Text = "Current version: " + CurrentVersion;
            settingsWindow.ShowDialog(this);
        }

        private void NavView_SelectionChanged(object? sender, NavigationViewSelectionChangedEventArgs e)
        {
            //this part controls which page to navigate to, upon user selection of a new NavViewItem

            if (NavView.SelectedItem == AllItemsItem)
            {
                ContentFrame.Navigate(typeof(AllItemsPage));
            }
            else if (NavView.SelectedItem == FilesItem)
            {
                ContentFrame.Navigate(typeof(FilesPage));
            }
            else if (NavView.SelectedItem == FoldersItem)
            {
                ContentFrame.Navigate(typeof(FoldersPage));
            }
            else if (NavView.SelectedItem == WebsitesItem)
            {
                ContentFrame.Navigate(typeof(WebsitesPage));
            }

        }
     
    }
}
