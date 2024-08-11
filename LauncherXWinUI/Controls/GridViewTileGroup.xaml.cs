using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LauncherXWinUI.Controls
{
    public sealed partial class GridViewTileGroup : UserControl
    {
        public GridViewTileGroup()
        {
            this.InitializeComponent();

            // Set the unique id to some guid
            this.UniqueId = System.Guid.NewGuid().ToString();

            // Subscribe to the event to notify us when new items are added/removed to the GridViewTileGroup
            Items.CollectionChanged += Items_CollectionChanged;
        }

        // Declare properties that this control will have

        /// <summary>
        /// A unique GUID to identify each item in the ItemsGridView
        /// </summary>
        public string UniqueId
        {
            get => (string)GetValue(UniqueIdProperty);
            set => SetValue(UniqueIdProperty, value);
        }

        DependencyProperty UniqueIdProperty = DependencyProperty.Register(
            nameof(UniqueId),
            typeof(string),
            typeof(GridViewTileGroup),
            new PropertyMetadata(default(string)));


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
            typeof(GridViewTileGroup),
            new PropertyMetadata(default(double), new PropertyChangedCallback(OnSizeChanged)));

        private static void OnSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GridViewTileGroup gridViewTileGroup = d as GridViewTileGroup;
            double? newScale = e.NewValue as double?;
            if (newScale != null)
            {
                // Calculate new dimensions
                double newSize = Math.Sqrt(newScale.Value);
                double newWidth = 105 * newSize;
                double newHeight = 95 * newSize;

                // Update control dimensions
                gridViewTileGroup.GridViewTileGroupControl.Width = newWidth;
                gridViewTileGroup.GridViewTileGroupControl.Height = newHeight;
                gridViewTileGroup.GroupGrid.Width = newWidth;
                gridViewTileGroup.GroupGrid.Height = newHeight;

                // TODO: Update image margin and dimensions
               
            }
        }

        /// <summary>
        /// List of GridViewTile objects in this GridViewTileGroup 
        /// </summary>
        /// Name the property "Items" for parity with GridView.Items and ListView.Items
        public ObservableCollection<GridViewTile> Items
        {
            get => (ObservableCollection<GridViewTile>)GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }

        DependencyProperty ItemsProperty = DependencyProperty.Register(
            nameof(Items),
            typeof(ObservableCollection<GridViewTile>),
            typeof(GridViewTileGroup),
            new PropertyMetadata(new ObservableCollection<GridViewTile>()));

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            
        }


        // Methods
        /// <summary>
        /// Highlights the control by drawing a border around it
        /// </summary>
        public void HighlightControl()
        {
            ControlBorder.BorderThickness = new Thickness(2);
        }

        /// <summary>
        /// Unhighlights the control by hiding its border
        /// </summary>
        public void UnhighlightControl()
        {
            ControlBorder.BorderThickness = new Thickness(0);
        }
    }
}
