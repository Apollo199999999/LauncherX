﻿#pragma checksum "C:\Users\fligh\source\repos\LauncherX\LauncherXWinUI\Controls\Dialogs\AddFileDialog.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "195C81D38A17EFB304FDAAF2BEACD04D4DF8674B99638ADDE9707047879EF401"
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
    partial class AddFileDialog : 
        global::Microsoft.UI.Xaml.Controls.ContentDialog, 
        global::Microsoft.UI.Xaml.Markup.IComponentConnector
    {

        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 3.0.0.2411")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1: // Controls\Dialogs\AddFileDialog.xaml line 2
                {
                    global::Microsoft.UI.Xaml.Controls.ContentDialog element1 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.ContentDialog>(target);
                    ((global::Microsoft.UI.Xaml.Controls.ContentDialog)element1).PrimaryButtonClick += this.ContentDialog_PrimaryButtonClick;
                }
                break;
            case 2: // Controls\Dialogs\AddFileDialog.xaml line 46
                {
                    this.SelectedFilesListView = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.ListView>(target);
                }
                break;
            case 3: // Controls\Dialogs\AddFileDialog.xaml line 55
                {
                    this.PlaceholderListViewItem = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.TextBlock>(target);
                }
                break;
            case 4: // Controls\Dialogs\AddFileDialog.xaml line 33
                {
                    this.PickAFileButton = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.PickAFileButton).Click += this.PickAFileButton_Click;
                }
                break;
            case 5: // Controls\Dialogs\AddFileDialog.xaml line 38
                {
                    this.OpenFilesProgressRing = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.ProgressRing>(target);
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
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 3.0.0.2411")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Microsoft.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Microsoft.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}

