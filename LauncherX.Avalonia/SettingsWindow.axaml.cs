using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using FluentAvalonia.UI.Controls;

namespace LauncherX.Avalonia
{
    public partial class SettingsWindow : CoreWindow
    {
        //init controls from xaml
        public NumericUpDown IconSizeNumUpDown = new NumericUpDown();
        public TextBox HeaderTextBox = new TextBox();
        public TextBlock VersionText = new TextBlock();
        public InfoBar AboutInfoBar = new InfoBar();
        public RadioButton LightThmRadioBtn = new RadioButton();
        public RadioButton DarkThmRadioBtn = new RadioButton();
        public RadioButton SystemThmRadioBtn = new RadioButton();

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
            AboutInfoBar = this.FindControl<InfoBar>("AboutInfoBar");
            LightThmRadioBtn = this.FindControl<RadioButton>("LightThmRadioBtn");
            DarkThmRadioBtn = this.FindControl<RadioButton>("DarkThmRadioBtn");
            SystemThmRadioBtn = this.FindControl<RadioButton>("SystemThmRadioBtn")

            //all event handlers go here
            AboutInfoBar.PointerEnter += AboutInfoBar_PointerEnter;
            AboutInfoBar.PointerLeave += AboutInfoBar_PointerLeave;

        }

        private void AboutInfoBar_PointerLeave(object? sender, global::Avalonia.Input.PointerEventArgs e)
        {
            
        }

        private void AboutInfoBar_PointerEnter(object? sender, global::Avalonia.Input.PointerEventArgs e)
        {
           
        }
    }
}
