﻿#pragma checksum "C:\Users\fligh\source\repos\LauncherX\LauncherXWinUI\SettingsWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "1CB7B6165F63E5826A1078C4F47BB1201A7398BF45E506CA68B4F617C399CF24"
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
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 3.0.0.2406")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 2: // SettingsWindow.xaml line 19
                {
                    this.Container = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Grid>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Grid)this.Container).Loaded += this.Container_Loaded;
                }
                break;
            case 3: // SettingsWindow.xaml line 25
                {
                    this.ContainerFallbackBackgroundBrush = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Media.SolidColorBrush>(target);
                }
                break;
            case 4: // SettingsWindow.xaml line 39
                {
                    this.AppTitleBar = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Border>(target);
                }
                break;
            case 5: // SettingsWindow.xaml line 43
                {
                    this.AppTitle = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.TextBlock>(target);
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
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 3.0.0.2406")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Microsoft.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Microsoft.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}

