using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using FluentAvalonia.Styling;
using FluentAvalonia.UI.Controls;
using LauncherX.Avalonia.Pages;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;
using static LauncherX.Avalonia.PublicVariables;
using Button = Avalonia.Controls.Button;
using static LauncherX.Avalonia.UserSettings;
using Avalonia.Media.Immutable;
using FluentAvalonia.UI.Media;

namespace LauncherX.Avalonia
{
    //class to store user settings
   
    public partial class SettingsWindow : CoreWindow
    {
        //init controls from xaml
        public Grid TitleBarHost = new Grid();
        public Panel ControlsPanel = new Panel();
        public NumericUpDown IconSizeNumUpDown = new NumericUpDown();
        public TextBox HeaderTextBox = new TextBox();
        public TextBlock VersionText = new TextBlock();
        public RadioButton LightThmRadioBtn = new RadioButton();
        public RadioButton DarkThmRadioBtn = new RadioButton();
        public Button AboutBtn = new Button();
        public Button CheckUpdatesBtn = new Button();
        public Button SaveBtn = new Button();

        //functions go here

        //function to save settings
        public void SaveSettings()
        {
            //init the usersettings class to store user settings
            Settings_IconSize = IconSizeNumUpDown.Value;
            Settings_HeaderText = HeaderTextBox.Text;
            if (LightThmRadioBtn.IsChecked == true)
            {
                Settings_Theme = "Light";
            }
            else if (DarkThmRadioBtn.IsChecked == true)
            {
                Settings_Theme = "Dark";
            }

            

            //TODO: serialize json
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                //serialize json in the appdata folder
            }

        }

        public SettingsWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            //SettingsWindow event handlers go here
            this.Opened += SettingsWindow_Opened;
            this.Closing += SettingsWindow_Closing;
        }

        private void SettingsWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            //don't close the window, only hide it
            e.Cancel = true;

            //activate the mainwindow before hiding the settingswindow
            PV_MainWindow.Activate();

            //hide the settings window
            this.Hide();

        }

        private void SettingsWindow_Opened(object? sender, System.EventArgs e)
        {
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

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            //locate controls
            TitleBarHost = this.FindControl<Grid>("TitleBarHost");
            ControlsPanel = this.FindControl<Panel>("ControlsPanel");
            IconSizeNumUpDown = this.FindControl<NumericUpDown>("IconSizeNumUpDown");
            HeaderTextBox = this.FindControl<TextBox>("HeaderTextBox");
            VersionText = this.FindControl<TextBlock>("VersionText");
            LightThmRadioBtn = this.FindControl<RadioButton>("LightThmRadioBtn");
            DarkThmRadioBtn = this.FindControl<RadioButton>("DarkThmRadioBtn");
            AboutBtn = this.FindControl<Button>("AboutBtn");
            CheckUpdatesBtn = this.FindControl<Button>("CheckUpdatesBtn");
            SaveBtn = this.FindControl<Button>("SaveBtn");

            //all event handlers go here
            LightThmRadioBtn.Checked += LightThmRadioBtn_Checked;
            DarkThmRadioBtn.Checked += DarkThmRadioBtn_Checked;
            AboutBtn.Click += AboutBtn_Click;
            CheckUpdatesBtn.Click += CheckUpdatesBtn_Click;
            SaveBtn.Click += SaveBtn_Click;

        }

        private void SaveBtn_Click(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
        {
            //hide this window after saving settings
            this.Hide();
        }

        private async void CheckUpdatesBtn_Click(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
        {
            //check for updates if there is an intenet connection
            if (PV_CheckForInternetConnection() == true)
            {
                if (PV_CheckForUpdates() == true)
                {
                    //show a contentdialog to prompt the user to update
                    ContentDialog UpdateDialog = new ContentDialog();
                    UpdateDialog.Title = "Update available";
                    UpdateDialog.Content = "An update is available, would you like to download it?";
                    UpdateDialog.PrimaryButtonText = " Yes ";
                    UpdateDialog.CloseButtonText = " No ";
                    UpdateDialog.DefaultButton = ContentDialogButton.Primary;
                    ContentDialogResult result = await UpdateDialog.ShowAsync();

                    if (result == ContentDialogResult.Primary)
                    {
                        //navigate to the update page and shutdown the application
                        PV_OpenBrowser("https://www.github.com/Apollo199999999/LauncherX/releases");

                        this.Close();
                        if (PV_MainWindow != null)
                        {
                            PV_MainWindow.Close();
                        }

                    }
                }
                else if (PV_CheckForUpdates() == false)
                {
                    //show a contentdialog to show that there are no updates available
                    ContentDialog NoUpdateDialog = new ContentDialog();
                    NoUpdateDialog.Title = "You're up to date!";
                    NoUpdateDialog.Content = "No updates are available for LauncherX";
                    NoUpdateDialog.CloseButtonText = " OK ";
                    NoUpdateDialog.DefaultButton = ContentDialogButton.Close;
                    ContentDialogResult result = await NoUpdateDialog.ShowAsync();
                }
            }
            else if (PV_CheckForInternetConnection() == false)
            {
                //no internet conntection available, show an error message
                ContentDialog NoInternetDialog = new ContentDialog();
                NoInternetDialog.Title = "No internet connection";
                NoInternetDialog.Content = "LauncherX cannot check for updates as there is no internet connection. " +
                    "Connect to the internet and try again"; ;
                NoInternetDialog.CloseButtonText = " OK ";
                NoInternetDialog.DefaultButton = ContentDialogButton.Close;
                ContentDialogResult result = await NoInternetDialog.ShowAsync();
            }
            
           
        }

        private async void AboutBtn_Click(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
        {
            //show a contentdialog with the about page

            //init contentdialog
            ContentDialog AboutDialog = new ContentDialog();
            AboutDialog.Title = "About LauncherX";
            AboutDialog.CloseButtonText = " OK ";
            AboutDialog.DefaultButton = ContentDialogButton.Close;
            AboutDialog.Content = new AboutDialogContentPage();

            var result = await AboutDialog.ShowAsync();
        }

        private void DarkThmRadioBtn_Checked(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
        {
            //set the app theme to dark theme
            PV_ChangeApplicationTheme("dark");
        }

        private void LightThmRadioBtn_Checked(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
        {
            //set the app theme to light theme
            PV_ChangeApplicationTheme("light");
            
        }
    }
}
