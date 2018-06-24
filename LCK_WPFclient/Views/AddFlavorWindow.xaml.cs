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

using LCK_ClientLibrary;

namespace LCK_WPFclient.Views
{
    /// <summary>
    /// Interaction logic for AddFlavorWindow.xaml
    /// </summary>
    public partial class AddFlavorWindow : Window
    {
        public bool Cancel = true;
        public BatchWPF SelectedDailyBatch = new BatchWPF();

        public AddFlavorWindow()
        {
            InitializeComponent();

            cboFlavors.ItemsSource = null;
            cboFlavors.ItemsSource = Globals.AllFlavors;
        }

        private void btnAdd(object sender, RoutedEventArgs e)
        {
            Cancel = false;
            this.Close();
        }

        private void btnCancel(object sender, RoutedEventArgs e)
        {
            Cancel = true;
            this.Close();
        }

        private void cboFlavors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedDailyBatch.Flavor = (FlavorWPF)cboFlavors.SelectedItem;
        }
    }
}
