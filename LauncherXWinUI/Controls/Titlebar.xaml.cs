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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LauncherXWinUI.Controls
{
    public sealed partial class Titlebar : UserControl
    {
        public Titlebar()
        {
            this.InitializeComponent();
        }

        // Properties
        /// <summary>
        /// Icon rendered in the titlebar
        /// </summary>
        public ImageSource Icon
        {
            get => (ImageSource)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        DependencyProperty IconProperty = DependencyProperty.Register(
            nameof(Icon),
            typeof(ImageSource),
            typeof(Titlebar),
            new PropertyMetadata(default(ImageSource), new PropertyChangedCallback(OnIconChanged)));

        private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Titlebar titlebar = d as Titlebar;
            ImageSource newImageSource = e.NewValue as ImageSource;

            if (newImageSource != null)
            {
                titlebar.IconElement.Source = newImageSource;
            }
        }

        /// <summary>
        /// Title of the titlebar
        /// </summary>
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        DependencyProperty TitleProperty = DependencyProperty.Register(
            nameof(Title),
            typeof(string),
            typeof(Titlebar),
            new PropertyMetadata(default(string), new PropertyChangedCallback(OnTitleChanged)));

        private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Titlebar titlebar = d as Titlebar;
            string newTitle = e.NewValue as string;

            if (newTitle != null)
            {
                // Update textblock
                titlebar.TitleElement.Text = newTitle;
            }
        }
    }
}
