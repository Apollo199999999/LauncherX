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
using System.Runtime.InteropServices; // For DllImport
using WinRT; // required to support Window.As<ICompositionSupportsSystemBackdrop>()
using SettingsUI;
using SettingsUI.Helpers;
using LauncherX.WinUI3.Frames;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LauncherX.WinUI3
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void ParentControlsGrid_Loaded(object sender, RoutedEventArgs e)
        {
            //CONFIGURE WINDOW

            //Enable Mica
            SystemBackdropsHelper systemBackdropsHelper = new SystemBackdropsHelper(this);
            systemBackdropsHelper.SetBackdrop(BackdropType.Mica);

            //Set window size and minimum window size
            WindowHelper.SetWindowSize(this, 900, 600);
            WindowHelper.RegisterWindowMinMax(this);
            WindowHelper.MinWindowWidth = 600;
            WindowHelper.MinWindowHeight = 400;

        }

        private async void AddCategoryBtn_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog();
            dialog.XamlRoot = this.Content.XamlRoot;
            dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            dialog.Title = "Create new collection";
            dialog.PrimaryButtonText = "Create Collection";
            dialog.CloseButtonText = "Cancel";
            dialog.DefaultButton = ContentDialogButton.Primary;
            dialog.Content = new CreateCollectionDialogContent();

            var result = await dialog.ShowAsync();

        }
    }
}
