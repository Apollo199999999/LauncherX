using LauncherXWinUI.Classes;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using WinUIEx.Messaging;

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

            // Disable maximise
            UIFunctionsClass.PreventWindowMaximise(this);

            // Update the textbox and slider to show correct values
            ScaleSlider.Value = UserSettingsClass.GridScale;
            ChangeHeaderTextBox.Text = UserSettingsClass.HeaderText;
            FullscreenToggleSwitch.IsOn = UserSettingsClass.UseFullscreen;
            GridAlignComboBox.SelectedItem = UserSettingsClass.GridPosition;

            // Create event handlers for the textbox and slider to update settings when their value is changed
            // We only create the event handlers here to prevent them from firing when the window loads
            // Since we are updating the settings using these event handlers, if they fire when the window is created, 
            // they will write wrong (blank) values to the UserSettingsClass
            ScaleSlider.ValueChanged += ScaleSlider_ValueChanged;
            ChangeHeaderTextBox.TextChanged += ChangeHeaderTextBox_TextChanged;
            FullscreenToggleSwitch.Toggled += FullscreenToggleSwitch_Toggled;
            GridAlignComboBox.SelectionChanged += GridAlignComboBox_SelectionChanged;

            // Make sure to unsubscribe from the event handlers after
            ScaleSlider.Unloaded += (s, e) => ScaleSlider.ValueChanged -= ScaleSlider_ValueChanged;
            ChangeHeaderTextBox.Unloaded += (s, e) => ChangeHeaderTextBox.TextChanged -= ChangeHeaderTextBox_TextChanged;
            FullscreenToggleSwitch.Unloaded += (s, e) => FullscreenToggleSwitch.Toggled -= FullscreenToggleSwitch_Toggled;
            GridAlignComboBox.Unloaded += (s, e) => GridAlignComboBox.SelectionChanged -= GridAlignComboBox_SelectionChanged;
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

        private void FullscreenToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            // Update UserSettingsClass
            UserSettingsClass.UseFullscreen = FullscreenToggleSwitch.IsOn;
            UserSettingsClass.WriteSettingsFile();
        }
        private void GridAlignComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Update UserSettingsClass
            UserSettingsClass.GridPosition = GridAlignComboBox.SelectedItem.ToString();
            UserSettingsClass.WriteSettingsFile();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // Close Window
            this.Close();
        }

        private async void CheckUpdatesBtn_Click(object sender, RoutedEventArgs e)
        {
            // Show infobars and progress ring depending on whether updates are available or not
            CheckUpdatesProgressRing.IsActive = true;

            // Close all InfoBars first
            UpdateFailInfoBar.IsOpen = false;
            NoUpdateInfoBar.IsOpen = false;
            UpdateFailInfoBar.IsOpen = false;

            bool? isUpdateAvailable = await UpdatesClass.IsUpdateAvailable();
            if (isUpdateAvailable == true)
            {
                UpdateInfoBar.IsOpen = true;
            }
            else if (isUpdateAvailable == false)
            {
                NoUpdateInfoBar.IsOpen = true;
            }
            else
            {
                UpdateFailInfoBar.IsOpen = true;
            }

            CheckUpdatesProgressRing.IsActive = false;
        }

        private void GetUpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to GitHub releases page and exit application
            Process.Start(new ProcessStartInfo { FileName = "https://github.com/Apollo199999999/LauncherX/releases", UseShellExecute = true});
            Application.Current.Exit();
        }

        private async void ImportDataBtn_Click(object sender, RoutedEventArgs e)
        {
            ImportProgressRing.IsActive = true;

            // Allow the user to select a zip file
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "ZIP Files (.ZIP)|*.zip";

            bool extractSuccess = false;

            if(openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string settingsFile = openFileDialog.FileName;

                // Read the contents of the zip file without extracting, check if it contains valid files
                bool foundSettingsFile = false;
                bool foundFilesDir = false;

                using (ZipArchive zipFile = ZipFile.OpenRead(settingsFile))
                {

                    foreach (ZipArchiveEntry zip in zipFile.Entries)
                    {
                        if (foundSettingsFile && foundFilesDir)
                        {
                            break;
                        }

                        if (zip.FullName == "userSettings.json")
                        {
                            foundSettingsFile = true;
                        }

                        if (zip.FullName.StartsWith("Files/"))
                        {
                            foundFilesDir = true; 
                        }
                    }

                }

                if (!foundSettingsFile || !foundFilesDir)
                {
                    // Invalid settings zip file
                    extractSuccess = false;

                }
                else
                {
                    try
                    {
                        // Clear SettingsDir first
                        System.IO.DirectoryInfo di = new DirectoryInfo(UserSettingsClass.SettingsDir);

                        foreach (FileInfo file in di.GetFiles())
                        {
                            file.Delete();
                        }
                        foreach (DirectoryInfo dir in di.GetDirectories())
                        {
                            dir.Delete(true);
                        }

                        // Extract the zip file to the settings directory
                        await Task.Run(() => ZipFile.ExtractToDirectory(settingsFile, UserSettingsClass.SettingsDir, true));

                        // Update settings and UI for SettingsWindow and MainWindow
                        UserSettingsClass.TryReadSettingsFile();
                        Container_Loaded(Container, null);
                        App.MainWindow.Container_Loaded(App.MainWindow, null);

                        extractSuccess = true;
                    }
                    catch
                    {
                        extractSuccess = false;
                    }
                }


                if (extractSuccess)
                {
                    // Success dialog
                    ContentDialog sucessDialog = new ContentDialog();
                    sucessDialog.XamlRoot = Container.XamlRoot;
                    sucessDialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
                    sucessDialog.Title = "Import success!";
                    sucessDialog.PrimaryButtonText = "OK";
                    sucessDialog.DefaultButton = ContentDialogButton.Primary;
                    sucessDialog.Content = "Your settings have been successfully imported into LauncherX.";

                    await sucessDialog.ShowAsync();
                }
                else
                {
                    // Error message
                    ContentDialog errorDialog = new ContentDialog();
                    errorDialog.XamlRoot = Container.XamlRoot;
                    errorDialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
                    errorDialog.Title = "Import failed";
                    errorDialog.PrimaryButtonText = "OK";
                    errorDialog.DefaultButton = ContentDialogButton.Primary;
                    errorDialog.Content = "Failed to import settings. Check that the settings file selected is valid or try again later.";

                    await errorDialog.ShowAsync();
                }

            }

            ImportProgressRing.IsActive = false;

        }

        private async void ExportDataBtn_Click(object sender, RoutedEventArgs e)
        {
            ExportProgressRing.IsActive = true;

            // Open a SaveFileDialog to tell us where to export settings to
            System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog.Filter = "ZIP Files (.ZIP)|*.zip";
            saveFileDialog.FileName = "settings.zip";

            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string saveSettingsPath = saveFileDialog.FileName;

                try
                {
                    if (File.Exists(saveSettingsPath) == true)
                    {
                        File.Delete(saveSettingsPath);
                    }
                    // Create zip file
                    await Task.Run(() => ZipFile.CreateFromDirectory(UserSettingsClass.SettingsDir, saveSettingsPath));

                    // Show a simple success dialog
                    ContentDialog sucessDialog = new ContentDialog();
                    sucessDialog.XamlRoot = Container.XamlRoot;
                    sucessDialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
                    sucessDialog.Title = "Export success!";
                    sucessDialog.PrimaryButtonText = "OK";
                    sucessDialog.DefaultButton = ContentDialogButton.Primary;
                    sucessDialog.Content = "Your settings have been exported to: " + saveSettingsPath;

                    await sucessDialog.ShowAsync();

                }
                catch
                {
                    // Show error dialog
                    ContentDialog errorDialog = new ContentDialog();
                    errorDialog.XamlRoot = Container.XamlRoot;
                    errorDialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
                    errorDialog.Title = "Export failed";
                    errorDialog.PrimaryButtonText = "OK";
                    errorDialog.DefaultButton = ContentDialogButton.Primary;
                    errorDialog.Content = "Failed to export settings. Please try again later or create an issue on GitHub.";

                    await errorDialog.ShowAsync();
                }
            }

            ExportProgressRing.IsActive = false;
        }
    }
}
