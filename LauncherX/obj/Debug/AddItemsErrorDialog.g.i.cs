﻿#pragma checksum "..\..\AddItemsErrorDialog.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "44916149D485C540569517117E20A1E77C744609B36DA5C95D9564D4C5611B0F"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using LauncherX;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using Wpf.Ui;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using Wpf.Ui.Controls.Navigation;
using Wpf.Ui.Converters;
using Wpf.Ui.Markup;
using Wpf.Ui.ValidationRules;


namespace LauncherX {
    
    
    /// <summary>
    /// AddItemsErrorDialog
    /// </summary>
    public partial class AddItemsErrorDialog : Wpf.Ui.Controls.UiWindow, System.Windows.Markup.IComponentConnector {
        
        
        #line 30 "..\..\AddItemsErrorDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock textBlock;
        
        #line default
        #line hidden
        
        
        #line 37 "..\..\AddItemsErrorDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox ErrorFiles;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\AddItemsErrorDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button button;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/LauncherX;component/additemserrordialog.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\AddItemsErrorDialog.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.textBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 2:
            this.ErrorFiles = ((System.Windows.Controls.ListBox)(target));
            return;
            case 3:
            this.button = ((System.Windows.Controls.Button)(target));
            
            #line 47 "..\..\AddItemsErrorDialog.xaml"
            this.button.Click += new System.Windows.RoutedEventHandler(this.button_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

