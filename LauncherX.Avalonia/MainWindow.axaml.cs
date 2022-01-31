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
using Button = Avalonia.Controls.Button;
using static LauncherX.Avalonia.PublicVariables;
using Avalonia.Controls.ApplicationLifetimes;
using System.Runtime.InteropServices;
using static LauncherX.Avalonia.AddItemLogic;

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

        //init SettingsWindow
        SettingsWindow settingsWindow = new SettingsWindow();

        //init item pages for navview
        AllItemsPage allItemsPage = new AllItemsPage();
        FilesPage filesPage = new FilesPage();
        FoldersPage foldersPage = new FoldersPage();
        WebsitesPage websitesPage = new WebsitesPage();

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            //MainWindow event handlers go here
            this.Opened += MainWindow_Opened;
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

            //set the application shutdownmode to onmainwindowclose

            if (Application.Current != null)
            {
                if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    desktop.ShutdownMode = ShutdownMode.OnMainWindowClose;
                }
            }

            //set the NavView SelectedItem manually and navigate to that page
            NavView.SelectedItem = AllItemsItem;
            ContentFrame.Navigate(typeof(AllItemsPage));

            //all event handlers go here
            SettingsBtn.Click += SettingsBtn_Click;
            AddWebsiteBtn.Click += AddWebsiteBtn_Click;
            NavView.SelectionChanged += NavView_SelectionChanged;

        }

        private void MainWindow_Opened(object? sender, EventArgs e)
        {
            //store the current MainWindow instance in the PublicVariables class
            PV_MainWindow = this;

            //store the current SettingsWindow instance in the PublicVariables class
            PV_SettingsWindow = settingsWindow;

            //set the theme according to the theme manager;
            var thmMgr = AvaloniaLocator.Current.GetService<FluentAvaloniaTheme>();

            if (thmMgr.RequestedTheme == "Light")
            {
                //set the app theme to light
                PV_ChangeApplicationTheme("light");
            }
            else if (thmMgr.RequestedTheme == "Dark")
            {
                //set the app theme to light
                PV_ChangeApplicationTheme("dark");
            }

        }

        private async void AddWebsiteBtn_Click(object? sender, RoutedEventArgs e)
        {
            //show a contentdialog with a textbox to enter the website URL

            //init websitedialogcontentpage
            WebsiteDialogContentPage websiteDialogContentPage = new WebsiteDialogContentPage();

            //init contentdialog
            ContentDialog WebsiteDialog = new ContentDialog();
            WebsiteDialog.Title = "Add a website";
            WebsiteDialog.PrimaryButtonText = " OK ";
            WebsiteDialog.CloseButtonText = " Cancel ";
            WebsiteDialog.DefaultButton = ContentDialogButton.Primary;
            WebsiteDialog.Content = websiteDialogContentPage;

            ContentDialogResult result = await WebsiteDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                //modify the url from the textbox in the websitedialogcontentpage
                if (websiteDialogContentPage.UrlTextBox.Text.StartsWith("https://"))
                {
                    string OriginalUrl = websiteDialogContentPage.UrlTextBox.Text;
                    string ModifiedUrl = websiteDialogContentPage.UrlTextBox.Text.Remove(0, 8);
                }

                //add the website to the gridview in the allitemspage and the websitespage
            }
        }

        private void SettingsBtn_Click(object? sender, RoutedEventArgs e)
        {
            //when the settings button is clicked, configure and open the settings window
            settingsWindow.HeaderTextBox.Text = HeaderText.Text;
            settingsWindow.VersionText.Text = "Current version: " + PV_CurrentVersion;
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
