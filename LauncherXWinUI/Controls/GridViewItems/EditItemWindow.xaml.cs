using LauncherXWinUI.Classes;
using Microsoft.UI.Xaml;
using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LauncherXWinUI.Controls.GridViewItems
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// Most of the logic for this window is handled in GridViewTile, as this is merely a dialog for that
    /// </summary>
    public sealed partial class EditItemWindow : WinUIEx.WindowEx
    {
        public EditItemWindow()
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
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
