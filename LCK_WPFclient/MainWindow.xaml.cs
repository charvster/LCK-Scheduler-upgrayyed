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
using System.ComponentModel;
using System.Runtime.Serialization.Formatters.Binary;

//using LCK_WPFclient.LCK_ServiceReference;

namespace LCK_WPFclient
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public WCF_CommManager main = new WCF_CommManager();

        public MainWindow()
        {
            InitializeComponent();

            Version ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            this.Title = "Scheduler Upgrayyed v" + ver.Major.ToString() + "." + ver.Minor.ToString(); 

            this.DataContext = main;
            
        }

        private void btnNewOrders_Click(object sender, RoutedEventArgs e)
        {
            //LCK_WPFclient.Views.FlavorsPreviewWindow win = new Views.FlavorsPreviewWindow();
            //win.ShowDialog();
            //return;

            main.ShowNewOrders();
        }

        private void btnCakeEditor_Click(object sender, RoutedEventArgs e)
        {
            main.ShowCakeEditor();
        }

        private void btnFlavorEditor_Click(object sender, RoutedEventArgs e)
        {
            main.ShowFlavorEditor();
        }
        
        private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            // Work around - without this line the first click, after selecting a day in the calendar, does nothing. The second click performs desired action
            Mouse.Capture(null);
        }

        private void btnToday_Click(object sender, RoutedEventArgs e)
        {
            main.SelectedDate = DateTime.Now;
            calSelectDay.DisplayDate = DateTime.Now;
        }

    }
}
