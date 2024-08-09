using LauncherXWinUI.Classes;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LauncherXWinUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsWindow : WinUIEx.WindowEx
    {
        public SettingsWindow()
        {
            this.InitializeComponent();
        }

        // Event Handlers
        private void Container_Loaded(object sender, RoutedEventArgs e)
        {
            // Set placeholder titlebar for now, before WASDK 1.6
            this.ExtendsContentIntoTitleBar = true;
            this.SetTitleBar(AppTitleBar);

            // Set Window icon
            UIFunctionsClass.SetWindowLauncherXIcon(this);

            // Set Window Background
            UIFunctionsClass.SetWindowBackground(this, ContainerFallbackBackgroundBrush);

            // Update the textbox and slider to show correct values
            ScaleSlider.Value = UserSettingsClass.GridScale;
            ChangeHeaderTextBox.Text = UserSettingsClass.HeaderText;

            // Create event handlers for the textbox and slider to update settings when their value is changed
            // We only create the event handlers here to prevent them from firing when the window loads
            // Since we are updating the settings using these event handlers, if they fire when the window is created, 
            // they will write wrong (blank) values to the UserSettingsClass
            ScaleSlider.ValueChanged += ScaleSlider_ValueChanged;
            ChangeHeaderTextBox.TextChanged += ChangeHeaderTextBox_TextChanged;

            // Make sure to unsubscribe from the event handlers after
            ScaleSlider.Unloaded += (s, e) => ScaleSlider.ValueChanged -= ScaleSlider_ValueChanged;
            ChangeHeaderTextBox.Unloaded += (s, e) => ChangeHeaderTextBox.TextChanged -= ChangeHeaderTextBox_TextChanged;
        }

        private void ScaleSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            // Update UserSettingsClass
            UserSettingsClass.GridScale = Math.Round(ScaleSlider.Value, 2);
            UserSettingsClass.WriteSettingsFile();
        }

        private void ChangeHeaderTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Update UserSettingsClass
            UserSettingsClass.HeaderText = ChangeHeaderTextBox.Text;
            UserSettingsClass.WriteSettingsFile();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // Close Window
            this.Close();
        }
    }
}
