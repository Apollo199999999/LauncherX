using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using FluentAvalonia.Styling;
using FluentAvalonia.UI.Controls;
using Button = Avalonia.Controls.Button;

namespace LauncherX.Avalonia
{
    public partial class SettingsWindow : Window
    {
        //string to store the system theme
        string SysTheme = "";

        //init controls from xaml
        public NumericUpDown IconSizeNumUpDown = new NumericUpDown();
        public TextBox HeaderTextBox = new TextBox();
        public TextBlock VersionText = new TextBlock();
        public RadioButton LightThmRadioBtn = new RadioButton();
        public RadioButton DarkThmRadioBtn = new RadioButton();
        public RadioButton SystemThmRadioBtn = new RadioButton();
        public Button AboutBtn = new Button();

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

            //get the system theme
            var thm = AvaloniaLocator.Current.GetService<FluentAvaloniaTheme>();
            SysTheme = thm.RequestedTheme;

        }

        private void AboutBtn_Click(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
        {
            //show a contentdialog with the about page
        }

        private void SystemThmRadioBtn_Checked(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
        {
            //set the app theme to system theme
            var thm = AvaloniaLocator.Current.GetService<FluentAvaloniaTheme>();
            thm.RequestedTheme = SysTheme;
        }

        private void DarkThmRadioBtn_Checked(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
        {
            //set the app theme to dark theme
            var thm = AvaloniaLocator.Current.GetService<FluentAvaloniaTheme>();
            thm.RequestedTheme = "Dark";
        }

        private void LightThmRadioBtn_Checked(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
        {
            //set the app theme to light theme
            var thm = AvaloniaLocator.Current.GetService<FluentAvaloniaTheme>();
            thm.RequestedTheme = "Light";
        }
    }
}
