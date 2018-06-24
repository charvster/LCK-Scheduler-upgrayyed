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
using System.Windows.Navigation;
using System.Windows.Shapes;

using LCK_ClientLibrary;

namespace LCK_WPFclient.Views
{
    /// <summary>
    /// Interaction logic for DayView.xaml
    /// </summary>
    public partial class DayView : UserControl
    {
        public DayView()
        {
            InitializeComponent();
        }

        private void lvwVista_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SpecialOrderWPF so = (this.DataContext as DayInfoWPF).SelectedSO;
            so.IsEditable = false;
            
            SpecialOrderWindow win = new SpecialOrderWindow(so);
            win.Show();
        }

        private void lvwCarlsbad_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SpecialOrderWPF so = (this.DataContext as DayInfoWPF).SelectedSO;
            so.IsEditable = false;

            SpecialOrderWindow win = new SpecialOrderWindow(so);
            win.Show();
        }

    }
}
