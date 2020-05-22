using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LauncherX
{
    /// <summary>
    /// Interaction logic for AddItemsErrorDialog.xaml
    /// </summary>
    public partial class AddItemsErrorDialog 
    {
        public AddItemsErrorDialog()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            //close the window
            this.Close();
        }
    }
}
