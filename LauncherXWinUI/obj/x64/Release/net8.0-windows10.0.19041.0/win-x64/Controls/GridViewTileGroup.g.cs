﻿#pragma checksum "C:\Users\fligh\source\repos\LauncherX\LauncherXWinUI\Controls\GridViewTileGroup.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "888223D93943D0BA82C248E01FF571BAAECC3C7D3A30D9E33FD055E7B612EE2B"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LauncherXWinUI.Controls
{
    partial class GridViewTileGroup : 
        global::Microsoft.UI.Xaml.Controls.UserControl, 
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
            case 1: // Controls\GridViewTileGroup.xaml line 2
                {
                    this.GridViewTileGroupControl = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.UserControl>(target);
                }
                break;
            case 2: // Controls\GridViewTileGroup.xaml line 14
                {
                    this.AddItemFlyout = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.MenuFlyout>(target);
                }
                break;
            case 3: // Controls\GridViewTileGroup.xaml line 19
                {
                    this.ControlBorder = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Border>(target);
                }
                break;
            case 4: // Controls\GridViewTileGroup.xaml line 24
                {
                    this.GroupPanel = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.StackPanel>(target);
                    ((global::Microsoft.UI.Xaml.Controls.StackPanel)this.GroupPanel).RightTapped += this.GroupPanel_RightTapped;
                    ((global::Microsoft.UI.Xaml.Controls.StackPanel)this.GroupPanel).Tapped += this.GroupPanel_Tapped;
                }
                break;
            case 5: // Controls\GridViewTileGroup.xaml line 29
                {
                    this.ItemsPreviewGrid = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Grid>(target);
                }
                break;
            case 6: // Controls\GridViewTileGroup.xaml line 43
                {
                    this.TileText = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.TextBlock>(target);
                }
                break;
            case 7: // Controls\GridViewTileGroup.xaml line 52
                {
                    this.GroupDialog = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.ContentDialog>(target);
                }
                break;
            case 8: // Controls\GridViewTileGroup.xaml line 61
                {
                    this.GroupDialogContent = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Grid>(target);
                }
                break;
            case 9: // Controls\GridViewTileGroup.xaml line 66
                {
                    this.GroupDialogTitleTextBox = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.TextBox>(target);
                }
                break;
            case 10: // Controls\GridViewTileGroup.xaml line 71
                {
                    this.ItemsGridView = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.GridView>(target);
                }
                break;
            case 11: // Controls\GridViewTileGroup.xaml line 86
                {
                    this.RightClickMenu = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.MenuFlyout>(target);
                }
                break;
            case 12: // Controls\GridViewTileGroup.xaml line 87
                {
                    this.MenuRemoveOption = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.MenuFlyoutItem>(target);
                    ((global::Microsoft.UI.Xaml.Controls.MenuFlyoutItem)this.MenuRemoveOption).Click += this.MenuRemoveOption_Click;
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

