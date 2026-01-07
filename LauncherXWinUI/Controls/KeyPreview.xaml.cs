using LauncherXWinUI.Classes;
using LauncherXWinUI.Controls.GridViewItems;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Windows.Forms;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LauncherXWinUI.Controls
{
    public sealed partial class KeyPreview : Microsoft.UI.Xaml.Controls.UserControl
    {
        public KeyPreview()
        {
            InitializeComponent();
        }

        // Declare properties that this control will have

        /// <summary>
        /// Key that is assigned to this control
        /// </summary>
        public Keys Key
        {
            get => (Keys)GetValue(KeyProperty);
            set => SetValue(KeyProperty, value);
        }

        DependencyProperty KeyProperty = DependencyProperty.Register(
            nameof(Key),
            typeof(Keys),
            typeof(KeyPreview),
            new PropertyMetadata(default(Keys), new PropertyChangedCallback(OnKeyChanged)));

        private static void OnKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            KeyPreview keyPreview = d as KeyPreview;
            Keys? newKey = e.NewValue as Keys?;
            if (newKey != null)
            {
                keyPreview.KeyTextBlock.Text = KeyClass.KeycodeToChar(newKey.Value);
            }
        }

        /// <summary>
        /// Size of the control
        /// </summary>
        public double Size
        {
            get => (double)GetValue(SizeProperty);
            set => SetValue(SizeProperty, value);
        }

        DependencyProperty SizeProperty = DependencyProperty.Register(
            nameof(Size),
            typeof(double),
            typeof(KeyPreview),
            new PropertyMetadata(1.0, new PropertyChangedCallback(OnSizeChanged)));

        private static void OnSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            KeyPreview keyPreview = d as KeyPreview;
            double? newScale = e.NewValue as double?;
            if (newScale != null)
            {
                double newSize = Math.Sqrt(newScale.Value);
                keyPreview.KeyContainer.MinWidth = newSize * 40;
                keyPreview.KeyContainer.Margin = new Thickness(newSize * 5);
                keyPreview.KeyContainer.Padding = new Thickness(newSize * 10);
                keyPreview.KeyTextBlock.FontSize = newSize * 14;
            }
        }
    }
}
