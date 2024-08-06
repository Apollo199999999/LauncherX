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
using System.Reflection.Emit;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LauncherXWinUI.Controls
{
    public sealed partial class GridViewTile : UserControl
    {
        public GridViewTile()
        {
            this.InitializeComponent();
        }

        // Declare properties that this control will have
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
            typeof(GridViewTile), 
            new PropertyMetadata(default(double), new PropertyChangedCallback(OnSizeChanged)));

        private static void OnSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GridViewTile gridViewTile = d as GridViewTile;
            double? newSize = e.NewValue as double?;
            if (newSize != null)
            {
                gridViewTile.GridViewTileControl.Width = 105 * Math.Sqrt(newSize.Value);
                gridViewTile.GridViewTileControl.Height = 95 * Math.Sqrt(newSize.Value);
            }
        }

    }
}
