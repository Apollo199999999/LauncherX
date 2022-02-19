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
using System.IO;
using System.Collections.Generic;
using Avalonia.Media.Immutable;
using FluentAvalonia.UI.Media;
using System.Threading.Tasks;

namespace LauncherX.Avalonia
{
    public partial class MainWindow : CoreWindow
    {
        //init controls from xaml
        public Grid TitleBarHost = new Grid();
        public Panel ControlsPanel = new Panel();
        public TextBlock HeaderText = new TextBlock();
        public Button SettingsBtn = new Button();
        public Button AddWebsiteBtn = new Button();
        public Button AddFolderBtn = new Button();
        public Button AddFileBtn = new Button();
        public NavigationView NavView = new NavigationView();
        public NavigationViewItem AllItemsItem = new NavigationViewItem();
        public NavigationViewItem FilesItem = new NavigationViewItem();
        public NavigationViewItem FoldersItem = new NavigationViewItem();
        public NavigationViewItem WebsitesItem = new NavigationViewItem();
        public Frame ContentFrame = new Frame();
        
        //init instances of all the pages to store data
        public AllItemsPage allItemsPage = new AllItemsPage();
        public FilesPage filesPage = new FilesPage();
        public FoldersPage foldersPage = new FoldersPage();
        public WebsitesPage websitesPage = new WebsitesPage();

        //init SettingsWindow
        SettingsWindow settingsWindow = new SettingsWindow();

        //functions go here:

        private void OnRequestedThemeChanged(FluentAvaloniaTheme sender, RequestedThemeChangedEventArgs args)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // TODO: add Windows version to CoreWindow
                if (IsWindows11 && args.NewTheme != FluentAvaloniaTheme.HighContrastModeString)
                {
                    TryEnableMicaEffect(sender);
                }
                else if (args.NewTheme == FluentAvaloniaTheme.HighContrastModeString)
                {
                    // Clear the local value here, and let the normal styles take over for HighContrast theme
                    SetValue(BackgroundProperty, AvaloniaProperty.UnsetValue);
                }
            }
        }


        public void TryEnableMicaEffect(FluentAvaloniaTheme thm)
        {

            // The background colors for the Mica brush are still based around SolidBackgroundFillColorBase resource
            // BUT since we can't control the actual Mica brush color, we have to use the window background to create
            // the same effect. However, we can't use SolidBackgroundFillColorBase directly since its opaque, and if
            // we set the opacity the color become lighter than we want. So we take the normal color, darken it and 
            // apply the opacity until we get the roughly the correct color
            // NOTE that the effect still doesn't look right, but it suffices. Ideally we need access to the Mica
            // CompositionBrush to properly change the color but I don't know if we can do that or not
            if (thm.RequestedTheme == FluentAvaloniaTheme.DarkModeString)
            {
                var color = this.TryFindResource("SolidBackgroundFillColorBase", out var value) ? (Color2)(Color)value : new Color2(32, 32, 32);

                color = color.LightenPercent(-0.2f);

                Background = new ImmutableSolidColorBrush(color, 0.85f);
            }
            else if (thm.RequestedTheme == FluentAvaloniaTheme.LightModeString)
            {
                // Similar effect here
                var color = this.TryFindResource("SolidBackgroundFillColorBase", out var value) ? (Color2)(Color)value : new Color2(243, 243, 243);

                color = color.LightenPercent(0.5f);

                Background = new ImmutableSolidColorBrush(color, 0.9);
            }
        }


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

            //create directories
            Directory.CreateDirectory(PV_WebsiteIconDir);

            //locate controls
            TitleBarHost = this.FindControl<Grid>("TitleBarHost");
            ControlsPanel = this.FindControl<Panel>("ControlsPanel");
            HeaderText = this.FindControl<TextBlock>("HeaderText");
            SettingsBtn = this.FindControl<Button>("SettingsBtn");
            AddWebsiteBtn = this.FindControl<Button>("AddWebsiteBtn");
            AddFolderBtn = this.FindControl<Button>("AddFolderBtn");
            AddFileBtn = this.FindControl<Button>("AddFileBtn");
            NavView = this.FindControl<NavigationView>("NavView");
            AllItemsItem = this.FindControl<NavigationViewItem>("AllItemsItem");
            FilesItem = this.FindControl<NavigationViewItem>("FilesItem");
            FoldersItem = this.FindControl<NavigationViewItem>("FoldersItem");
            WebsitesItem = this.FindControl<NavigationViewItem>("WebsitesItem");
            ContentFrame = this.FindControl<Frame>("ContentFrame");

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

            //set the titlebar of the window
            if (this.TitleBar != null)
            {
                //extend view into titlebar
                this.TitleBar.ExtendViewIntoTitleBar = true;

                //make the titlebar visible and set the margin of the ControlsPanel
                ControlsPanel.Margin = new Thickness(0, 0, 0, 0);
                TitleBarHost.IsVisible = true;

                //set the titlebar
                this.SetTitleBar(TitleBarHost);

                //set the titlebar margin so that it doesn't hide the caption buttons
                TitleBarHost.Margin = new Thickness(0, 0, this.TitleBar.SystemOverlayRightInset, 0);
            }

            var thm = AvaloniaLocator.Current.GetService<FluentAvaloniaTheme>();
            thm.RequestedThemeChanged += OnRequestedThemeChanged;

            // Enable Mica on Windows 11
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {

                if (IsWindows11 && thm.RequestedTheme != FluentAvaloniaTheme.HighContrastModeString)
                {
                    TransparencyBackgroundFallback = Brushes.Transparent;
                    TransparencyLevelHint = WindowTransparencyLevel.Mica;

                    TryEnableMicaEffect(thm);
                }
            }

            thm.ForceWin32WindowToTheme(this);

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
                //add the website
                AddWebsite(websiteDialogContentPage.UrlTextBox.Text, 1.0);
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
                AllItemsPage page = ContentFrame.Content as AllItemsPage;
                page.Content = allItemsPage.Content;
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
