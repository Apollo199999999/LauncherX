using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LauncherXWinUI.Controls.Dialogs
{
    public sealed partial class AddFileDialogListViewItem : UserControl
    {
        public AddFileDialogListViewItem()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Text to be displayed on the ListView item
        /// </summary>
        public string DisplayText
        {
            get => (string)GetValue(DisplayTextProperty);
            set => SetValue(DisplayTextProperty, value);
        }

        DependencyProperty DisplayTextProperty = DependencyProperty.Register(
            nameof(DisplayText),
            typeof(string),
            typeof(AddFileDialogListViewItem),
            new PropertyMetadata(default(string), new PropertyChangedCallback(OnDisplayTextChanged)));

        private static void OnDisplayTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AddFileDialogListViewItem addFileDialogListViewItem = d as AddFileDialogListViewItem;
            string newDisplayText = e.NewValue as string;
            if (newDisplayText != null)
            {
                addFileDialogListViewItem.ListItemTextBlock.Text = newDisplayText;
            }
        }

        /// <summary>
        /// The item that will be executed when this file is added
        /// </summary>
        public string ExecutingPath
        {
            get => (string)GetValue(ExecutingPathProperty);
            set => SetValue(ExecutingPathProperty, value);
        }

        DependencyProperty ExecutingPathProperty = DependencyProperty.Register(
            nameof(ExecutingPath),
            typeof(string),
            typeof(AddFileDialogListViewItem),
            new PropertyMetadata(default(string), new PropertyChangedCallback(OnExecutingPathChanged)));

        private static void OnExecutingPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AddFileDialogListViewItem addFileDialogListViewItem = d as AddFileDialogListViewItem;
            string newExecutingPath = e.NewValue as string;
            if (newExecutingPath != null)
            {
                ToolTipService.SetToolTip(addFileDialogListViewItem.ListItemTextBlock, newExecutingPath);
            }
        }

        /// <summary>
        /// Launch arguments, if necessary
        /// </summary>
        public string ExecutingArguments
        {
            get => (string)GetValue(ExecutingArgumentsProperty);
            set => SetValue(ExecutingArgumentsProperty, value);
        }

        DependencyProperty ExecutingArgumentsProperty = DependencyProperty.Register(
            nameof(ExecutingArguments),
            typeof(string),
            typeof(AddFileDialogListViewItem),
            new PropertyMetadata(""));


        /// <summary>
        /// Path to the icon of the file
        /// </summary>
        public BitmapImage FileIcon
        {
            get => (BitmapImage)GetValue(FileIconProperty);
            set => SetValue(FileIconProperty, value);
        }

        DependencyProperty FileIconProperty = DependencyProperty.Register(
            nameof(FileIcon),
            typeof(BitmapImage),
            typeof(AddFileDialogListViewItem),
            new PropertyMetadata(default(ImageSource)));

        // Event Handlers
        private void RemoveBtn_Click(object sender, RoutedEventArgs e)
        {
            // Remove this from the parent ListView
            ListView parentListView = this.Parent as ListView;
            if (parentListView != null)
            {
                parentListView.Items.Remove(this);
            }
        }

        private void LaunchArgsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ExecutingArguments = LaunchArgsTextBox.Text;
        }
    }
}
