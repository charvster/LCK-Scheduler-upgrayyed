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
    /// Interaction logic for FlavorEditorWindow.xaml
    /// </summary>
    public partial class FlavorEditorWindow : Window
    {
        private List<FlavorWPF> _flavors = new List<FlavorWPF>();
        public List<FlavorWPF> Flavors
        {
            get { return Globals.AllFlavors; }
        }

        private LCK_WCFcommunication lck_comm;

        public FlavorEditorWindow()
        {
            InitializeComponent();
        }

        public FlavorEditorWindow(LCK_WCFcommunication WCF_Comm)
        {
            InitializeComponent();

            this.lck_comm = WCF_Comm;
            RefreshFlavors();
        }

        private void RefreshFlavors()
        {
            if (lck_comm == null)
                return;

            lvwFlavors.ItemsSource = null;
            lvwFlavors.ItemsSource = this.lck_comm.GetAllFlavors();
        }

        private void lvwFlavors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvwFlavors.SelectedItem == null)
                return;

            txtName.Text = (lvwFlavors.SelectedItem as FlavorWPF).Name;
            txtDescription.Text = (lvwFlavors.SelectedItem as FlavorWPF).Description;
            chkNotAFlavor.IsChecked = (lvwFlavors.SelectedItem as FlavorWPF).NotAflavor;
            chkCakeFlavor.IsChecked = (lvwFlavors.SelectedItem as FlavorWPF).CakeFlavor;
            chkInvisible.IsChecked = (lvwFlavors.SelectedItem as FlavorWPF).Invisible;

            btnAdd_Update.Content = "Update";
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearUI();
        }

        private void ClearUI()
        {
            txtName.Text = "";
            txtDescription.Text = "";
            chkNotAFlavor.IsChecked = false;
            chkCakeFlavor.IsChecked = false;
            btnAdd_Update.Content = "Add";
        }

        private void btnAdd_Update_Click(object sender, RoutedEventArgs e)
        {
            if (txtName.Text == "")
                return;

            // build and populate flavorWPF object
            FlavorWPF flav = new FlavorWPF();
            flav.Name = txtName.Text;
            flav.Description = txtDescription.Text;
            flav.NotAflavor = (bool)chkNotAFlavor.IsChecked;
            flav.CakeFlavor = (bool)chkCakeFlavor.IsChecked;
            flav.Invisible = (bool)chkInvisible.IsChecked;

            if(btnAdd_Update.Content.ToString() == "Add")
            {
                lck_comm.AddFlavor(flav);
            }
            else if(btnAdd_Update.Content.ToString() == "Update")
            {
                if (lvwFlavors.SelectedItem == null)
                    return;

                flav.ID = (lvwFlavors.SelectedItem as FlavorWPF).ID;
                lck_comm.UpdateFlavor(flav.ID, flav);
            }

            ClearUI();
            RefreshFlavors();
        }

        private void mnuDelete_Click(object sender, RoutedEventArgs e)
        {
            // confirm with user then exe
            MessageBoxResult res = MessageBox.Show("Delete Flavor '" + (lvwFlavors.SelectedItem as FlavorWPF).Name + "'?", "Delete Flavor?", MessageBoxButton.YesNo);
            if (res != MessageBoxResult.Yes)
                return;

            bool success = lck_comm.DeleteFlavor((lvwFlavors.SelectedItem as FlavorWPF));
            if(success)
                RefreshFlavors();

            btnClear_Click(this, null);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
