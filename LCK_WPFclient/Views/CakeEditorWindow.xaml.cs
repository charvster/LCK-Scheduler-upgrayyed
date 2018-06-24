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
    /// Interaction logic for CakeEditorWindow.xaml
    /// </summary>
    public partial class CakeEditorWindow : Window
    {
        public CakeEditorWindow()
        {
            InitializeComponent();
        }

        public CakeEditorWindow(LCK_WCFcommunication WCF_Comm)
        {
            InitializeComponent();

            this.lck_comm = WCF_Comm;
            RefreshCakes();
        }

        private void lvwCakes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvwCakes.SelectedItem == null)
                return;

            txtName.Text = (lvwCakes.SelectedItem as CakeWPF).Name;
            txtAbv.Text = (lvwCakes.SelectedItem as CakeWPF).AbvName;
            txtDescription.Text = (lvwCakes.SelectedItem as CakeWPF).Description;

            btnAdd_Update.Content = "Update";
        }

        public List<CakeWPF> Cakes
        {
            get { return Globals.AllCakes; }
        }

        private LCK_WCFcommunication lck_comm;
        
        private void RefreshCakes()
        {
            if (lck_comm == null)
                return;

            lvwCakes.ItemsSource = null;
            lvwCakes.ItemsSource = this.lck_comm.GetAllCakes();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtName.Text = "";
            txtAbv.Text = "";
            txtDescription.Text = "";
            btnAdd_Update.Content = "Add";
        }

        private void btnAdd_Update_Click(object sender, RoutedEventArgs e)
        {
            if (txtName.Text == "")
                return;

            if(btnAdd_Update.Content.ToString() == "Add")
            {
                CakeWPF cke = new CakeWPF();
                cke.Name = txtName.Text;
                cke.AbvName = txtAbv.Text;
                cke.Description = txtDescription.Text;
                lck_comm.AddCake(cke);
            }
            else if(btnAdd_Update.Content.ToString() == "Update")
            {
                if (lvwCakes.SelectedItem == null)
                    return;

                CakeWPF cke = new CakeWPF();
                cke.ID = (lvwCakes.SelectedItem as CakeWPF).ID;
                cke.Name = txtName.Text;
                cke.AbvName = txtAbv.Text;
                cke.Description = txtDescription.Text;
                lck_comm.UpdateCake(cke.ID, cke);
            }

            RefreshCakes();
        }

        private void mnuDelete_Click(object sender, RoutedEventArgs e)
        {
            // confirm with user then exe
            MessageBoxResult res = MessageBox.Show("Delete Cake '" + (lvwCakes.SelectedItem as CakeWPF).Name + "'?", "Delete Cake?", MessageBoxButton.YesNo);
            if (res != MessageBoxResult.Yes)
                return;

            bool success = lck_comm.DeleteCake((lvwCakes.SelectedItem as CakeWPF));
            if(success)
                RefreshCakes();

            btnClear_Click(this, null);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
