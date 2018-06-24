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
    /// Interaction logic for DayPreviewView.xaml
    /// </summary>
    public partial class DayPreviewView : UserControl
    {
        public DayPreviewView()
        {
            InitializeComponent();
        }

        private void mouseLtUp_ControlClicked(object sender, MouseButtonEventArgs e)
        {
            DayInfoWPF dc = (DayInfoWPF)this.DataContext;

            // throw event to be caught by parent and pass along the datacontext
            if (dc.Clicked != null) { dc.Clicked(dc, new DayInfoWPF.DayInfoEventArg()); }
        }
    }
}
