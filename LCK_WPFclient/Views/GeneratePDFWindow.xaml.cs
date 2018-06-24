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

using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using LCK_ClientLibrary;

namespace LCK_WPFclient.Views
{
    /// <summary>
    /// Interaction logic for GeneratePDFWindow.xaml
    /// </summary>
    public partial class GeneratePDFWindow : Window
    {
        //private string destFolder = @"E:\Consulting\LCCK\Scheduler upgrade\development\LCK_WPFclient\bin\Debug\temp\";
        private string destFolder = ConfigSettings_Static.ScanOrderDefaultPath;
        public string DestinationFilename = "";

        public GeneratePDFWindow()
        {
            InitializeComponent();
        }

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PdfDocument doc = new PdfDocument();

                int idx = 0;
                foreach (ImageListBoxItem itm in lstImages.Items)
                {
                    if (itm == null)
                        continue;
                    string tmp = itm.filename;
                    doc.Pages.Add(new PdfPage());
                    XGraphics gfx = XGraphics.FromPdfPage(doc.Pages[idx]);
                    XImage img = XImage.FromFile(tmp);

                    gfx.DrawImage(img, 0, 0);
                    idx++;
                }

                if (txtPDFname.Text == "")
                {
                    MessageBox.Show("Missing Output pdf name.");
                    return;
                }

                DestinationFilename = destFolder + @"\" + txtPDFname.Text + ".pdf";

                if(!System.IO.Directory.Exists(destFolder))
                {
                    System.Windows.Forms.SaveFileDialog saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
                    saveFileDialog1.InitialDirectory = @"C:\";
                    //saveFileDialog1.CheckFileExists = true;
                    saveFileDialog1.CheckPathExists = true;
                    saveFileDialog1.DefaultExt = "pdf";
                    saveFileDialog1.Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*";
                    saveFileDialog1.RestoreDirectory = true;
                    saveFileDialog1.FileName = txtPDFname.Text;
                    if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        DestinationFilename = saveFileDialog1.FileName;
                    }
                    else
                        return;
                }

                doc.Save(DestinationFilename);
                doc.Close();
                
                // close window
                this.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error Message=" + ex.Message);
                DestinationFilename = "";
            }

        }

        private void btnAddImage_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog();
            dlg.Filter = "JPG Files (*.jpg)|*.jpg|All Files (*.*)|*.*";
            dlg.Multiselect = true;
            if(dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                foreach (string str in dlg.FileNames)
                {
                    lstImages.Items.Add(new ImageListBoxItem(str));
                }
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnMoveUp_Click(object sender, RoutedEventArgs e)
        {
            ImageListBoxItem itm = (ImageListBoxItem)lstImages.SelectedItem;
            int idx = lstImages.SelectedIndex;
            if (idx < 1)
                return;
            lstImages.Items.Remove(itm);
            lstImages.Items.Insert(idx-1, itm);
        }

        private void btnMoveDown_Click(object sender, RoutedEventArgs e)
        {
            ImageListBoxItem itm = (ImageListBoxItem)lstImages.SelectedItem;
            int idx = lstImages.SelectedIndex;
            if (idx >= lstImages.Items.Count-1)
                return;
            lstImages.Items.Remove(itm);
            lstImages.Items.Insert(idx+1, itm);
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            ImageListBoxItem itm = (ImageListBoxItem)lstImages.SelectedItem;
            lstImages.Items.Remove(itm);
        }
    }

    public class ImageListBoxItem
    {
        public string filename { get; set; }

        public ImageListBoxItem(string filename)
        {
            this.filename = filename;
        }

        public override string ToString()
        {
            return System.IO.Path.GetFileName(filename);
        }
    }
}
