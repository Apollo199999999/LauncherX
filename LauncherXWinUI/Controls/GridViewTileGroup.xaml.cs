using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Windows.UI;

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

            // For some reason, StackPanel needs a background for events to work properly
            GroupPanel.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

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
                gridViewTileGroup.GroupPanel.Width = newWidth;
                gridViewTileGroup.GroupPanel.Height = newHeight;
                gridViewTileGroup.ItemsPreviewGrid.Margin = new Thickness(newSize * 22.5, newSize * 5, newSize * 22.5, 0);
                gridViewTileGroup.ItemsPreviewGrid.Height = newWidth - newSize * 22.5 - newSize * 22.5;
                gridViewTileGroup.ItemsPreviewGrid.Width = newWidth - newSize * 22.5 - newSize * 22.5;

                // Update image controls
                foreach (Image image in gridViewTileGroup.ItemsPreviewGrid.Children)
                {
                    image.Width = gridViewTileGroup.ItemsPreviewGrid.Width / 2.5;
                    image.Height = gridViewTileGroup.ItemsPreviewGrid.Width / 2.5;
                    image.Stretch = Stretch.Uniform;
                }

                // Update the font size of the textblock
                gridViewTileGroup.TileText.FontSize = newSize * 12;
            }
        }

        /// <summary>
        /// Text that is displayed below the ItemsPreviewGridView
        /// </summary>
        public string DisplayText
        {
            get => (string)GetValue(DisplayTextProperty);
            set => SetValue(DisplayTextProperty, value);
        }

        DependencyProperty DisplayTextProperty = DependencyProperty.Register(
            nameof(DisplayText),
            typeof(string),
            typeof(GridViewTileGroup),
            new PropertyMetadata(default(string), new PropertyChangedCallback(OnDisplayTextChanged)));

        private static void OnDisplayTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GridViewTileGroup gridViewTileGroup = d as GridViewTileGroup;
            string newDisplayText = e.NewValue as string;

            if (newDisplayText != null)
            {
                // Update textblock
                gridViewTileGroup.TileText.Text = newDisplayText;
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
            ItemsPreviewGrid.Children.Clear();

            // Create new image objects to display the first 4 items in the items collection
            for (int i = 0; i < Items.Count; i++)
            {
                if (i >= 4)
                {
                    break;
                }

                Image image = new Image();
                image.Source = Items[i].ImageSource;
                image.Width = ItemsPreviewGrid.Width / 2.5;
                image.Height = ItemsPreviewGrid.Width / 2.5;
                image.Stretch = Stretch.Uniform;
                image.HorizontalAlignment = HorizontalAlignment.Center;
                image.VerticalAlignment = VerticalAlignment.Center;
                image.SetValue(Grid.RowProperty, Math.Floor((double)i / 2));
                image.SetValue(Grid.ColumnProperty, i % 2);
                ItemsPreviewGrid.Children.Add(image);
            }

        }


        // Methods
        /// <summary>
        /// Show the flyout to indicate that a item can be added to this group
        /// </summary>
        public void ShowAddItemIndicator()
        {
            MenuFlyout flyoutBase = (MenuFlyout)FlyoutBase.GetAttachedFlyout(GridViewTileGroupControl);
            flyoutBase.ShowAt(GridViewTileGroupControl);
            ControlBorder.BorderThickness = new Thickness(2);
        }

        /// <summary>
        /// Hide the flyout that indicates that a item can be added to this group
        /// </summary>
        public void HideAddItemIndicator()
        {
            MenuFlyout flyoutBase = (MenuFlyout)FlyoutBase.GetAttachedFlyout(GridViewTileGroupControl);
            flyoutBase.Hide();
            ControlBorder.BorderThickness = new Thickness(0);
        }

        /// <summary>
        /// Remove the items in this GridViewTileGroup from the parent GridView
        /// </summary>
        public void RemoveGroupItemsFromGridView()
        {
            GridView parentGridView = this.Parent as GridView;
            if (parentGridView != null)
            {
                foreach (GridViewTile gridViewTile in this.Items)
                {
                    if (parentGridView.Items.Contains(gridViewTile))
                    {
                        parentGridView.Items.Remove(gridViewTile);
                    }
                }
            }
        }
    }
}
