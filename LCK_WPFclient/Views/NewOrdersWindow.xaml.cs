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
    /// Interaction logic for NewOrdersWindow.xaml
    /// </summary>
    public partial class NewOrdersWindow : Window
    {
        private NewOrdersManager local_noManager;

        public NewOrdersWindow()
        {
            InitializeComponent();
        }

        public NewOrdersWindow(NewOrdersManager NO_Manager)
        {
            InitializeComponent();

            this.local_noManager = NO_Manager;
            this.DataContext = this.local_noManager;
            this.local_noManager.RefreshNewOrders();

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
