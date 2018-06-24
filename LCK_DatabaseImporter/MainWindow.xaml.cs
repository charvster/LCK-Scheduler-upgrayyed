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
using Microsoft.Win32;

namespace LCK_DatabaseImporter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AccessToSQLite convertor = new AccessToSQLite();

        private List<string> LogEntries = new List<string>();

        public MainWindow()
        {
            InitializeComponent();

            Log("Application Started.");

            //convertor.SpecialOrderTable();
        }

        private void Log(string msg)
        {
            LogEntries.Add(msg);
            RefreshLog();
        }

        private void RefreshLog()
        {
            lvwLog.ItemsSource = null;
            lvwLog.ItemsSource = LogEntries;
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Access DB files (*.mdb)|*.mdb|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (openFileDialog.ShowDialog() == true)
            {
                txtOldDB.Text = openFileDialog.FileName;
            }

        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            if (!System.IO.File.Exists(txtOldDB.Text))
            {
                Log("Selected Access File not found");
                return;
            }

            // run thru each transfer function in @convertor updating Log window with completion and entries added
            int rtn = 0;

            // Export Flavors table
            if (chkFlavors.IsChecked == true)
            {
                Log("Export of Flavors table started...");
                rtn = convertor.FlavorTable();
                if (rtn == -1)
                {
                    Log("Error processing Flavors table. Aborting export.");
                    return;
                }
                else
                    Log("Flavors table exported with " + rtn + " rows affected.");
            }

            // Export Batch table
            if (chkBatch.IsChecked == true)
            {
                Log("Export of Batch table started...");
                rtn = convertor.BatchTable();
                if (rtn == -1)
                {
                    Log("Error processing Batch table. Aborting export.");
                    return;
                }
                else
                    Log("Batch table exported with " + rtn + " rows affected.");
            }

            // Export Special_Orders table
            if (chkSO.IsChecked == true)
            {
                Log("Export of Special_Orders table started...");
                rtn = convertor.SpecialOrderTable();
                if (rtn == -1)
                {
                    Log("Error processing Special_Orders table. Aborting export.");
                    return;
                }
                else
                    Log("Special_Orders table exported with " + rtn + " rows affected.");
            }

            // Export SpecialOrder_Batch table
            if (chkSO_Batch.IsChecked == true)
            {
                Log("Export of SpecialOrder_Batch table started...");
                rtn = convertor.SO_BatchTable();
                if (rtn == -1)
                {
                    Log("Error processing SpecialOrder_Batch table. Aborting export.");
                    return;
                }
                else
                    Log("SpecialOrder_Batch table exported with " + rtn + " rows affected.");
            }
        }

        private void btnClose_Cick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
