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
    /// Interaction logic for FlavorsPreviewWindow.xaml
    /// </summary>
    public partial class FlavorsPreviewWindow : Window
    {
        private FlavorPreviewDC _localDC = new FlavorPreviewDC();
        public List<FlavorWPF> Flavors
        {
            get { return _localDC.Flavors; }
            set { _localDC.Flavors = value; }
        }
        public FlavorWPF SelectedFlavor
        {
            get { return _localDC.Selected; }
            set { _localDC.Selected = value; }
        }

        public FlavorsPreviewWindow(bool CakeFlavorsOnly = false)
        {
            InitializeComponent();

            _localDC.CakeOnlyFlavors = CakeFlavorsOnly;
            this.DataContext = _localDC;
        }

        private void Flavor_Click(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }

    public class FlavorPreviewDC : ObservableObject
    {
        public bool CakeOnlyFlavors = false;
        private List<FlavorWPF> _Flavors = Globals.AllFlavors;
        public List<FlavorWPF> Flavors
        {
            get 
            {
                if ((bool)CakeOnlyFlavors)
                {
                    List<FlavorWPF> tmp = new List<FlavorWPF>();
                    foreach (FlavorWPF flav in _Flavors)
                        if (flav.CakeFlavor)
                            tmp.Add(flav);
                 
                    return tmp.OrderBy(s => s.Name).Cast<FlavorWPF>().ToList();
                }
                else
                    return _Flavors.OrderBy(s => s.Name).Cast<FlavorWPF>().ToList();

            }
            set { _Flavors = value; NotifyPropertyChanged("Flavors"); }
        }
        private FlavorWPF _selected = new FlavorWPF();
        public FlavorWPF Selected
        {
            get { return _selected; }
            set { _selected = value; NotifyPropertyChanged("Selected"); }
        }
    }
}
