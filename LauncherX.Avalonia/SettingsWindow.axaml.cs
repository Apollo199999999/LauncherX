using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using FluentAvalonia.Styling;
using FluentAvalonia.UI.Controls;
using LauncherX.Avalonia.Pages;
using System;
using System.Runtime.InteropServices;
using static LauncherX.Avalonia.PublicVariables;
using Button = Avalonia.Controls.Button;

namespace LauncherX.Avalonia
{
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
                ChangeApplicationTheme("light");
            }
            else if (thmMgr.RequestedTheme == "Dark")
            {
                //set the app theme to light
                ChangeApplicationTheme("dark");
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

            //all event handlers go here
            LightThmRadioBtn.Checked += LightThmRadioBtn_Checked;
            DarkThmRadioBtn.Checked += DarkThmRadioBtn_Checked;
            AboutBtn.Click += AboutBtn_Click;

        }

        private async void AboutBtn_Click(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
        {
            //show a contentdialog with the about page

            //init contentdialog
            ContentDialog AboutDialog = new ContentDialog();
            AboutDialog.Title = "About LauncherX";
            AboutDialog.PrimaryButtonText = "OK";
            AboutDialog.DefaultButton = ContentDialogButton.Primary;
            AboutDialog.Content = new AboutDialogContentPage();

            var result = await AboutDialog.ShowAsync();
        }

        private void DarkThmRadioBtn_Checked(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
        {
            //set the app theme to dark theme
            ChangeApplicationTheme("dark");
        }

        private void LightThmRadioBtn_Checked(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
        {
            //set the app theme to light theme
            ChangeApplicationTheme("light");
            
        }
    }
}
