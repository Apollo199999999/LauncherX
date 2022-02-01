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

namespace LauncherX.Avalonia
{
    //class to store user settings
   
    public partial class SettingsWindow : Window
    {
        //init controls from xaml
        public ExperimentalAcrylicBorder SettingsWindowMicaMaterial = new ExperimentalAcrylicBorder();
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
            this.Hide();
        }

        private void SettingsWindow_Opened(object? sender, System.EventArgs e)
        {
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


        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            //locate controls
            SettingsWindowMicaMaterial = this.FindControl<ExperimentalAcrylicBorder>("SettingsWindowMicaMaterial");
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
