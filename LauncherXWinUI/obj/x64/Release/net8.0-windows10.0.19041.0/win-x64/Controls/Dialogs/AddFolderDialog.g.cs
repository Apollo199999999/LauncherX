﻿#pragma checksum "C:\Users\fligh\source\repos\LauncherX\LauncherXWinUI\Controls\Dialogs\AddFolderDialog.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "23065B65E66189364A54A89457C2B2180F6A6980ABEB6DF73BAFAF7A48A5FB0B"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LauncherXWinUI.Controls.Dialogs
{
    partial class AddFolderDialog : 
        global::Microsoft.UI.Xaml.Controls.ContentDialog, 
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
            case 1: // Controls\Dialogs\AddFolderDialog.xaml line 2
                {
                    global::Microsoft.UI.Xaml.Controls.ContentDialog element1 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.ContentDialog>(target);
                    ((global::Microsoft.UI.Xaml.Controls.ContentDialog)element1).PrimaryButtonClick += this.ContentDialog_PrimaryButtonClick;
                }
                break;
            case 2: // Controls\Dialogs\AddFolderDialog.xaml line 45
                {
                    this.SelectedFoldersListView = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.ListView>(target);
                }
                break;
            case 3: // Controls\Dialogs\AddFolderDialog.xaml line 54
                {
                    this.PlaceholderListViewItem = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.TextBlock>(target);
                }
                break;
            case 4: // Controls\Dialogs\AddFolderDialog.xaml line 32
                {
                    this.PickAFolderButton = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.PickAFolderButton).Click += this.PickAFolderButton_Click;
                }
                break;
            case 5: // Controls\Dialogs\AddFolderDialog.xaml line 37
                {
                    this.OpenFolderProgressRing = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.ProgressRing>(target);
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
