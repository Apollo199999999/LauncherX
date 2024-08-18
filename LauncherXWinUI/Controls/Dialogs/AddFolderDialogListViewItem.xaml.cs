using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LauncherXWinUI.Controls.Dialogs
{
    public sealed partial class AddFolderDialogListViewItem : UserControl
    {
        public AddFolderDialogListViewItem()
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
            typeof(AddFolderDialogListViewItem),
            new PropertyMetadata(default(string), new PropertyChangedCallback(OnDisplayTextChanged)));

        private static void OnDisplayTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AddFolderDialogListViewItem addFolderDialogListViewItem = d as AddFolderDialogListViewItem;
            string newDisplayText = e.NewValue as string;
            if (newDisplayText != null)
            {
                addFolderDialogListViewItem.ListItemTextBlock.Text = newDisplayText;
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
            typeof(AddFolderDialogListViewItem),
            new PropertyMetadata(default(string), new PropertyChangedCallback(OnExecutingPathChanged)));

        private static void OnExecutingPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AddFolderDialogListViewItem addFolderDialogListViewItem = d as AddFolderDialogListViewItem;
            string newExecutingPath = e.NewValue as string;
            if (newExecutingPath != null)
            {
                ToolTipService.SetToolTip(addFolderDialogListViewItem.ListItemTextBlock, newExecutingPath);
            }
        }

        /// <summary>
        /// Path to the icon of the folder
        /// </summary>
        public ImageSource FolderIcon
        {
            get => (ImageSource)GetValue(FileIconProperty);
            set => SetValue(FileIconProperty, value);
        }

        DependencyProperty FileIconProperty = DependencyProperty.Register(
            nameof(FolderIcon),
            typeof(ImageSource),
            typeof(AddFolderDialogListViewItem),
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
    }
}
