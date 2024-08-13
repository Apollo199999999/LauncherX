using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

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
                gridViewTileGroup.ControlBorder.Width = newWidth;
                gridViewTileGroup.ControlBorder.Height = newHeight;
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

        // Event Handlers
        private async void GroupPanel_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            // Show the ContentDialog to display the items in this Group
            ItemsGridView.Items.Clear();
            foreach (GridViewTile gridViewTile in Items)
            {
                ItemsGridView.Items.Add(gridViewTile);
            }

            ItemsGridView.Items.VectorChanged += ItemsGridViewItems_VectorChanged;

            // When the dialog is closed, do 3 things:
            // 1. Unsubscribe from the necessary events
            // 2. Clear the items in the ItemsGridView, so that when GridViewTiles from this control are added to a new GridViewTileGroup control to add more GridViewTiles (see MainWindow.xaml.cs),
            // there won't be a case where a GridViewTile has 2 parents
            // 3. Unselect this control in the ItemsGridView in MainWindow
            GroupDialog.CloseButtonClick += (s, e) =>
            {
                GroupDialogTitleTextBox.TextChanged -= GroupDialogTitleTextBox_TextChanged;
                ItemsGridView.Items.VectorChanged -= ItemsGridViewItems_VectorChanged;
                ItemsGridView.Items.Clear();

                // Unselect this item
                GridView parentGridView = this.Parent as GridView;
                if (parentGridView != null)
                {
                    parentGridView.SelectedItem = null;
                }
            };

            // Configure the title of the GroupDialog
            GroupDialogTitleTextBox.Text = DisplayText;
            GroupDialogTitleTextBox.TextChanged += GroupDialogTitleTextBox_TextChanged;

            // Configure the size of the GroupDialog
            GroupDialogContent.Width = App.MainWindow.Width * 0.8;
            GroupDialogContent.Height = App.MainWindow.Height * 0.6;
            App.MainWindow.SizeChanged += (s, e) =>
            {
                GroupDialogContent.Width = App.MainWindow.Width * 0.8;
                GroupDialogContent.Height = App.MainWindow.Height * 0.6;
            };

            var result = await GroupDialog.ShowAsync();
        }

        // Only fires when the GroupDialog is open
        private void GroupDialogTitleTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DisplayText = GroupDialogTitleTextBox.Text;
        }

        // Only fires when the GroupDialog is open
        private void ItemsGridViewItems_VectorChanged(Windows.Foundation.Collections.IObservableVector<object> sender, Windows.Foundation.Collections.IVectorChangedEventArgs @event)
        {
            // Update the ItemsProperty based on the items in the ItemsGridView
            Items.Clear();
            foreach (GridViewTile gridViewTile in ItemsGridView.Items)
            {
                Items.Add(gridViewTile);
            }
        }

        private void GroupPanel_RightTapped(object sender, Microsoft.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            // Show right click menu options
            MenuFlyout flyoutBase = (MenuFlyout)FlyoutBase.GetAttachedFlyout(GroupPanel);
            flyoutBase.ShowAt(GroupPanel, e.GetPosition(GroupPanel));
        }

        private void MenuRemoveOption_Click(object sender, RoutedEventArgs e)
        {
            // Remove this group
            GridView parentGridView = this.Parent as GridView;
            if (parentGridView != null)
            {
                parentGridView.Items.Remove(this);
            }
        }
    }
}
