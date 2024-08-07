using LauncherXWinUI.Classes;
using LauncherXWinUI.Controls;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
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

namespace LauncherXWinUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : WinUIEx.WindowEx
    {
        public MainWindow()
        {
            this.InitializeComponent();
            
            // Create settings directories and clear icon directories
            UserSettingsClass.CreateSettingsDirectories();
            UserSettingsClass.ClearIconDirectories();

            // Upgrade settings and write new settings file if necessary
            if (UserSettingsClass.UpgradeRequired())
            {
                UserSettingsClass.UpgradeUserSettings();
                UserSettingsClass.WriteSettingsFile();
            }

            // Retrieve user settings from file
            UserSettingsClass.TryReadSettingsFile();

            // Set header text
            HeaderTextBlock.Text = UserSettingsClass.HeaderText;
        }

        private void Container_Loaded(object sender, RoutedEventArgs e)
        {
            // Set placeholder titlebar for now, before WASDK 1.6
            this.ExtendsContentIntoTitleBar = true;
            this.SetTitleBar(AppTitleBar);

            // Set Window icon
            UIFunctionsClass.SetWindowLauncherXIcon(this);

            // Set Window Background
            UIFunctionsClass.SetWindowBackground(this, ContainerFallbackBackgroundBrush);
        }

        private async void AddFileBtn_Click(object sender, RoutedEventArgs e)
        {
            AddFileDialog addFileDialog = new AddFileDialog()
            {
                XamlRoot = Container.XamlRoot
            };

            ContentDialogResult result = await addFileDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                // Add the files from the addFileDialog
                foreach (AddFileDialogListViewItem fileItem in addFileDialog.AddedFiles)
                {
                    // Create new GridViewTile for each item
                    GridViewTile gridViewTile = new GridViewTile();
                    gridViewTile.ExecutingPath = fileItem.ExecutingPath;
                    gridViewTile.ExecutingArguments = fileItem.ExecutingArguments;
                    gridViewTile.DisplayText = fileItem.DisplayText;
                    gridViewTile.ImageSource = fileItem.FileIcon;
                    gridViewTile.Size = UserSettingsClass.GridScale;
                    ItemsGridView.Items.Add(gridViewTile);
                }   }
        }
    }
}
