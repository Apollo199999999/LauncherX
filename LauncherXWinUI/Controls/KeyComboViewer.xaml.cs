using LauncherXWinUI.Classes;
using LauncherXWinUI.Controls.Dialogs;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Forms;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LauncherXWinUI.Controls
{
    public sealed partial class KeyComboViewer : Microsoft.UI.Xaml.Controls.UserControl
    {
        /// <summary>
        /// GlobalKeyboardHook to detect key presses
        /// </summary>
        private GlobalKeyboardHook globalKeyboardHook;

        /// <summary>
        /// A HotKeyHook just to verify that the key combination input can be registered
        /// Note that this HotKeyHook is NOT used to actually activate LauncherX
        /// </summary>
        private HotKeyHook verifyKeyHook;

        /// <summary>
        /// As the name suggests, an event that is triggered when the user successsfully edits and registers a new keyboard shortcut
        /// </summary>
        public event EventHandler OnNewKeyboardShortcutRegistered;

        public KeyComboViewer()
        {
            InitializeComponent();

            // Initialise GlobalKeyboardHook
            globalKeyboardHook = new GlobalKeyboardHook();
            globalKeyboardHook.KeyDown += new System.Windows.Forms.KeyEventHandler(GlobalKeyboardHook_KeyDown);
            globalKeyboardHook.KeyUp += new System.Windows.Forms.KeyEventHandler(GlobalKeyboardHook_KeyUp);

            // Add the keys you want to hook to the HookedKeys list
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
                globalKeyboardHook.HookedKeys.Add(key);

            // Initialise a HotKeyHook for key combo verification
            verifyKeyHook = new HotKeyHook();
        }

        /// <summary>
        /// KeyCombo that is assigned to this control, expressed as a string, with each key being space separated
        /// </summary>
        public string KeyCombo
        {
            get => (string)GetValue(KeyComboProperty);
            set => SetValue(KeyComboProperty, value);
        }

        DependencyProperty KeyComboProperty = DependencyProperty.Register(
            nameof(KeyCombo),
            typeof(string),
            typeof(KeyComboViewer),
            new PropertyMetadata(default(string), new PropertyChangedCallback(OnKeyComboChanged)));

        private static void OnKeyComboChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            KeyComboViewer keyComboViewer = d as KeyComboViewer;
            string newKeyCombo = e.NewValue as string;
            keyComboViewer.UpdateUIFromKeyCombo(keyComboViewer, newKeyCombo);
            
        }

        // Helper functions

        /// <summary>
        /// Updates UI controls in a KeyComboViewer given a key combination
        /// </summary>
        /// <param name="keyComboViewer">KeyComboViewer to update</param>
        /// <param name="newKeyCombo">Key combination to use</param>
        private void UpdateUIFromKeyCombo(KeyComboViewer keyComboViewer, string newKeyCombo)
        {
            if (newKeyCombo != null)
            {
                // Clear existing key combo
                keyComboViewer.KeyComboKeysPanel.Children.Clear();
                keyComboViewer.KeyComboDialogPanel.Children.Clear();

                // From the key combo, which is a string, obtain a list of keys
                List<Keys> keysList = KeyClass.StringToKeysList(newKeyCombo);

                foreach (Keys key in keysList)
                {
                    // Create and display a KeyPreview control for each key
                    KeyPreview keyPreview = new KeyPreview();
                    keyPreview.Key = key;
                    keyComboViewer.KeyComboKeysPanel.Children.Add(keyPreview);

                    // Additionally, add the KeyPreview control to the panel in the ContentDialog
                    KeyPreview keyPreviewDialog = new KeyPreview();
                    keyPreviewDialog.Key = key;
                    keyPreviewDialog.Size = 2.0;
                    keyComboViewer.KeyComboDialogPanel.Children.Add(keyPreviewDialog);

                    // Also, enable the primary button in the ContentDialog (we assume key combo is valid)
                    keyComboViewer.KeyComboDialog.IsPrimaryButtonEnabled = true;
                }
            }
        }
        /// <summary>
        /// Gets the keys displayed in the KeyComboDialog
        /// </summary>
        /// <returns>A list of System.Windows.Forms.Keys</returns>
        private List<Keys> GetKeyComboDialogKeys()
        {
            List<Keys> pressedKeys = new List<Keys>();
            foreach (KeyPreview keyPreview in KeyComboDialogPanel.Children)
            {
                pressedKeys.Add(keyPreview.Key);
            }

            return pressedKeys;
        }

        /// <summary>
        /// Given a list of Keys, determine whether the key combination is valid
        /// </summary>
        /// <param name="pressedKeys">List of Keys</param>
        /// <returns>Boolean to determine whether the key combo is valid</returns>
        private bool IsKeyComboValid(List<Keys> pressedKeys)
        {
            if (pressedKeys.Count == 0)
            {
                return false;
            }

            // Valid key combos can only start with Windows Key, Ctrl, Alt, or Shift
            bool isValidKeyCombo = false;

            // We do this strange mapping so that LShift and RShift are not differentiated
            // (same with LControl and RControl, etc.)
            Keys firstKey = KeyClass.CharToKeycode(KeyClass.KeycodeToChar(pressedKeys[0]));
            Keys lastKey = KeyClass.CharToKeycode(KeyClass.KeycodeToChar(pressedKeys.Last()));

            if (firstKey != Keys.LWin && firstKey != Keys.ControlKey
                && firstKey != Keys.Menu && firstKey != Keys.ShiftKey)
            {
                //The key combo is invalid
                isValidKeyCombo = false;
            }
            else if (lastKey != Keys.LWin && lastKey != Keys.ControlKey
                && lastKey != Keys.Menu && lastKey != Keys.ShiftKey)
            {
                // The last key is an alphanumeric key, which is a valid key combo so long the user does not press any more keys
                isValidKeyCombo = true;
            }

            return isValidKeyCombo;
        }

        // Event handlers
        private async void ChangeKeyComboBtn_Click(object sender, RoutedEventArgs e)
        {
            // Activate the keyboard hook
            globalKeyboardHook.hook();

            // Launch a dialog that allows the user to modify the key combination
            ContentDialogResult result = await KeyComboDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                List<Keys> pressedKeys = GetKeyComboDialogKeys();

                // First, we need to determine if the key combination can be registered,
                // or whether it is being blocked by an application
                verifyKeyHook.UnregisterAll();
                if (KeyClass.TryRegisterHotKeyFromList(pressedKeys, verifyKeyHook))
                {
                    // Key combo is valid!
                    // Assign the new key combination to be registered later
                    this.KeyCombo = KeyClass.KeysListToString(pressedKeys);

                    // Raise the event
                    OnNewKeyboardShortcutRegistered(this, EventArgs.Empty);
                }
                else
                {
                    // Reset all UI controls
                    UpdateUIFromKeyCombo(this, this.KeyCombo);

                    // Show an error message
                    ContentDialog errorDialog = new ContentDialog();
                    errorDialog.XamlRoot = this.XamlRoot;
                    errorDialog.Style = Microsoft.UI.Xaml.Application.Current.Resources["DefaultContentDialogStyle"] as Style;
                    errorDialog.Title = "Failed to register new keyboard shortcut";
                    errorDialog.PrimaryButtonText = "OK";
                    errorDialog.DefaultButton = ContentDialogButton.Primary;
                    errorDialog.Content = "The keyboard shortcut could not be registered. This may be because another application might already be this keyboard shortcut. " +
                                  "LauncherX cannot override this action. Please use another keyboard shortcut.";

                    await errorDialog.ShowAsync();
                }

                verifyKeyHook.UnregisterAll();

            }
        }

        private void KeyComboDialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            // Unhook the GlobalKeyboardHook
            globalKeyboardHook.unhook();
        }

        // GlobalKeyboardHook event handlers
        /// <summary>
        /// Used in KeyDown to determine if a KeyUp event was triggered before the KeyDown
        /// </summary>
        private bool KeyUpInvoked = false;
        private void GlobalKeyboardHook_KeyUp(object sender, KeyEventArgs e)
        {
            KeyUpInvoked = true;

            // Check if KeyCombo is valid
            List<Keys> pressedKeys = GetKeyComboDialogKeys();
            bool isKeyComboValid = IsKeyComboValid(pressedKeys);

            if (isKeyComboValid)
            {
                InvalidInfoBar.IsOpen = false;
                KeyComboDialog.IsPrimaryButtonEnabled = true;
            }
            else
            {
                // Show invalid key combo InfoBar
                InvalidInfoBar.IsOpen = true;
                KeyComboDialog.IsPrimaryButtonEnabled = false;
            }
        }

        private void GlobalKeyboardHook_KeyDown(object sender, KeyEventArgs e)
        {
            // Disable the primary button first as user is in the process of registering a new key combo
            KeyComboDialog.IsPrimaryButtonEnabled = false;
            InvalidInfoBar.IsOpen = false;

            // First off, retrieve the keys that the user has already pressed
            List<Keys> pressedKeys = GetKeyComboDialogKeys();

            // The logic is that if the user already has a valid key combo, but still invokes key down event, 
            // then the user clearly wants to redo the key combo, so we clear the panel children
            // Or, if the user invoked KeyUp right before KeyDown, then the user wants to redo the key combo.
            if (IsKeyComboValid(pressedKeys) || KeyUpInvoked)
            {
                KeyComboDialogPanel.Children.Clear();
                pressedKeys.Clear();
            }

            // Finally, create a new KeyPreview control to append to the dialog
            // To prevent repeated keys, do so only if the key doesn't already exist in pressedKeys
            if (pressedKeys.Contains(e.KeyCode) == false)
            {
                KeyPreview keyPreview = new KeyPreview();
                keyPreview.Key = e.KeyCode;
                keyPreview.Size = 2.0;
                KeyComboDialogPanel.Children.Add(keyPreview);
            }

            KeyUpInvoked = false;
        }
    }
}
