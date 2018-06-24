using System;
using System.Collections;
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
using System.IO;

using LCK_ClientLibrary;
using LCK_WPFclient.Views;

namespace LCK_WPFclient.CustomControls
{
    public class FilesystemSuggestionProvider : WpfControls.ISuggestionProvider
    {
        IEnumerable WpfControls.ISuggestionProvider.GetSuggestions(string filter)
        {
            List<FlavorWPF> flavors = Globals.AllFlavors;
            List<FlavorWPF> subset = new List<FlavorWPF>();


            if (string.IsNullOrEmpty(filter))
            {
                return null;
            }
            if (filter.Length < 2)
            {
                return null;
            }

            var myRegex = new System.Text.RegularExpressions.Regex(@filter + "*", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            subset = flavors.Where(f => myRegex.IsMatch(f.Name)).ToList();

            return subset;
        }
    }

    /// <summary>
    /// Interaction logic for DropdownMulti.xaml
    /// </summary>
    public partial class DropdownMulti : UserControl
    {

        #region CakeOnly DP

        /// <summary>
        /// Gets or sets the Value which is being displayed
        /// </summary>
        public bool CakeOnly
        {
            get { return (bool)GetValue(CakeOnlyProperty); }
            set { SetValue(CakeOnlyProperty, value); }
        }

        /// <summary>
        /// Identified the Label dependency property
        /// </summary>
        public static readonly DependencyProperty CakeOnlyProperty =
            DependencyProperty.Register("CakeOnly", typeof(bool),
              typeof(DropdownMulti), new PropertyMetadata(null));

        #endregion

        #region ItemsSource DP

        public IEnumerable<FlavorWPF> ItemsSource
        {
            get { return (IEnumerable<FlavorWPF>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable<FlavorWPF>),
              typeof(DropdownMulti), new PropertyMetadata(null));
        #endregion

        #region SelectedItem DP

        public FlavorWPF SelectedItem
        {
            get { return (FlavorWPF)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(FlavorWPF),
              typeof(DropdownMulti), new PropertyMetadata(null));

        #endregion

        public DropdownMulti()
        {
            InitializeComponent();

            LayoutRoot.DataContext = this;
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            LCK_WPFclient.Views.FlavorsPreviewWindow win = new Views.FlavorsPreviewWindow(CakeOnly);
            win.Top = System.Windows.Forms.Control.MousePosition.Y;
            win.Left = System.Windows.Forms.Control.MousePosition.X - this.Width;
            if (ItemsSource != null)
            {
                // use ItemsSource list if one is provided, otherwise use full list
                List<FlavorWPF> tmp =  ItemsSource.Cast<FlavorWPF>().ToList();
                win.Flavors = tmp;
            }
            win.ShowDialog();
            SelectedItem = win.SelectedFlavor;
        }
    }
}
