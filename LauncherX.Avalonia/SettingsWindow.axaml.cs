using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using FluentAvalonia.Styling;
using FluentAvalonia.UI.Controls;
using LauncherX.Avalonia.Pages;
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
        public RadioButton SystemThmRadioBtn = new RadioButton();
        public Button AboutBtn = new Button();

        //functions go here

        //function to change the application theme
        public void ChangeApplicationTheme(string theme)
        {
            //get currently running mainwindow and settings window
            var _mainWindow = AvaloniaLocator.Current.GetService<MainWindow>();
            var _settingsWindow = AvaloniaLocator.Current.GetService<SettingsWindow>();

            //get the currently running fluentavalonia theme manager
            var thmMgr = AvaloniaLocator.Current.GetService<FluentAvaloniaTheme>();

            //change the theme accordingly only if the theme manager, mainwindow, and settings window are all not null (they have been initialized)
            if (_mainWindow != null && _settingsWindow != null && thmMgr != null)
            {
                if (theme.ToLower() == "light")
                {
                    //set light theme for both windows
                    thmMgr.RequestedTheme = "Light";

                    _mainWindow.MainWindowMicaMaterial.Material.TintColor = Colors.White;
                    _settingsWindow.SettingsWindowMicaMaterial.Material.TintColor = Colors.White;
                }
                else if (theme.ToLower() == "dark")
                {
                    //set light theme for both windows
                    thmMgr.RequestedTheme = "Dark";

                    _mainWindow.MainWindowMicaMaterial.Material.TintColor = Colors.Black;
                    _settingsWindow.SettingsWindowMicaMaterial.Material.TintColor = Colors.Black;
                }
                else if (theme.ToLower() == "system")
                {
                    //set light theme for both windows
                    thmMgr.RequestedTheme = SysTheme;

                    /*if the systheme is dark, set the mica material for both windows to black. Otherwise, if the systheme is light,
                    set the mica material for both windows to white*/

                    if (SysTheme.ToLower() == "light")
                    {
                        _mainWindow.MainWindowMicaMaterial.Material.TintColor = Colors.White;
                        _settingsWindow.SettingsWindowMicaMaterial.Material.TintColor = Colors.White;
                    }
                    else if (SysTheme.ToLower() == "dark")
                    {
                        _mainWindow.MainWindowMicaMaterial.Material.TintColor = Colors.Black;
                        _settingsWindow.SettingsWindowMicaMaterial.Material.TintColor = Colors.Black;
                    }
                }

            }

        }


        public SettingsWindow()
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
            IconSizeNumUpDown = this.FindControl<NumericUpDown>("IconSizeNumUpDown");
            HeaderTextBox = this.FindControl<TextBox>("HeaderTextBox");
            VersionText = this.FindControl<TextBlock>("VersionText");
            LightThmRadioBtn = this.FindControl<RadioButton>("LightThmRadioBtn");
            DarkThmRadioBtn = this.FindControl<RadioButton>("DarkThmRadioBtn");
            SystemThmRadioBtn = this.FindControl<RadioButton>("SystemThmRadioBtn");
            AboutBtn = this.FindControl<Button>("AboutBtn");

            //all event handlers go here
            LightThmRadioBtn.Checked += LightThmRadioBtn_Checked;
            DarkThmRadioBtn.Checked += DarkThmRadioBtn_Checked;
            SystemThmRadioBtn.Checked += SystemThmRadioBtn_Checked;
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

        private void SystemThmRadioBtn_Checked(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
        {
            //set the app theme to system theme
            ChangeApplicationTheme("system");
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
