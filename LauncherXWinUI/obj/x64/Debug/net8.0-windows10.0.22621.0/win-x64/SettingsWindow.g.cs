﻿#pragma checksum "C:\Users\fligh\source\repos\LauncherX\LauncherXWinUI\SettingsWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "D23CE857C2D79D27417EFFFB954ED6E66E7827A17852E03034707EA5FF8CB67D"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LauncherXWinUI
{
    partial class SettingsWindow : 
        global::WinUIEx.WindowEx, 
        global::Microsoft.UI.Xaml.Markup.IComponentConnector
    {

        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 3.0.0.2408")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 2: // SettingsWindow.xaml line 20
                {
                    this.Container = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Grid>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Grid)this.Container).Loaded += this.Container_Loaded;
                }
                break;
            case 3: // SettingsWindow.xaml line 26
                {
                    this.ContainerFallbackBackgroundBrush = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Media.SolidColorBrush>(target);
                }
                break;
            case 4: // SettingsWindow.xaml line 39
                {
                    this.AppTitleBar = global::WinRT.CastExtensions.As<global::LauncherXWinUI.Controls.Titlebar>(target);
                }
                break;
            case 5: // SettingsWindow.xaml line 202
                {
                    this.CloseButton = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.CloseButton).Click += this.CloseButton_Click;
                }
                break;
            case 6: // SettingsWindow.xaml line 145
                {
                    this.UpdateInfoBar = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.InfoBar>(target);
                }
                break;
            case 7: // SettingsWindow.xaml line 161
                {
                    this.NoUpdateInfoBar = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.InfoBar>(target);
                }
                break;
            case 8: // SettingsWindow.xaml line 168
                {
                    this.UpdateFailInfoBar = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.InfoBar>(target);
                }
                break;
            case 9: // SettingsWindow.xaml line 152
                {
                    this.GetUpdateBtn = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.GetUpdateBtn).Click += this.GetUpdateBtn_Click;
                }
                break;
            case 10: // SettingsWindow.xaml line 131
                {
                    this.CheckUpdatesProgressRing = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.ProgressRing>(target);
                }
                break;
            case 11: // SettingsWindow.xaml line 135
                {
                    this.CheckUpdatesBtn = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.CheckUpdatesBtn).Click += this.CheckUpdatesBtn_Click;
                }
                break;
            case 12: // SettingsWindow.xaml line 102
                {
                    this.FullscreenToggleSwitch = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.ToggleSwitch>(target);
                }
                break;
            case 13: // SettingsWindow.xaml line 91
                {
                    this.ChangeHeaderTextBox = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.TextBox>(target);
                }
                break;
            case 14: // SettingsWindow.xaml line 71
                {
                    this.ScaleSlider = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Slider>(target);
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }


        /// <summary>
        /// GetBindingConnector(int connectionId, object target)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 3.0.0.2408")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Microsoft.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Microsoft.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}

