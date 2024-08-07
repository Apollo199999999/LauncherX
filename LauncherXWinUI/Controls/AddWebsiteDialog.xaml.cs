using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Diagnostics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LauncherXWinUI.Controls
{
    public sealed partial class AddWebsiteDialog : ContentDialog
    {
        public AddWebsiteDialog()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// The full website URL input by the user
        /// </summary>
        public string InputWebsiteUrl
        {
            get => (string)GetValue(InputWebsiteUrlProperty);
            set => SetValue(InputWebsiteUrlProperty, value);
        }

        DependencyProperty InputWebsiteUrlProperty = DependencyProperty.Register(
            nameof(InputWebsiteUrl),
            typeof(string),
            typeof(AddWebsiteDialog),
            new PropertyMetadata(default(string)));

        // Event Handlers
        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Assign the user input to the InputWebsiteUrl property of this control
            InputWebsiteUrl = HttpComboBox.SelectedItem.ToString() + UrlInputTextBox.Text;
        }

        private void UrlInputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Enable/Disable the primary button of the ContentDialog depending on if there is text input in the TextBox
            if (UrlInputTextBox.Text.Length > 0)
            {
                this.IsPrimaryButtonEnabled = true;
            }
            else
            {
                this.IsPrimaryButtonEnabled = false;
            }
        }
    }
}
