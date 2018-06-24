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
    public class CakeSuggestionProvider : WpfControls.ISuggestionProvider
    {
        IEnumerable WpfControls.ISuggestionProvider.GetSuggestions(string filter)
        {
            List<FlavorWPF> flavors = Globals.AllCakeFlavors;
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
    /// Interaction logic for DropdownMulti_CakeOnly.xaml
    /// </summary>
    public partial class DropdownMulti_CakeOnly : UserControl
    {
                
        #region ItemsSource DP

        public IEnumerable<FlavorWPF> ItemsSource
        {
            get { return (IEnumerable<FlavorWPF>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable<FlavorWPF>),
              typeof(DropdownMulti_CakeOnly), new PropertyMetadata(null));
        #endregion

        #region SelectedItem DP

        public FlavorWPF SelectedItem
        {
            get { return (FlavorWPF)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(FlavorWPF),
              typeof(DropdownMulti_CakeOnly), new PropertyMetadata(null));

        #endregion

        public DropdownMulti_CakeOnly()
        {
            InitializeComponent();

            LayoutRoot.DataContext = this;
        }

    }
}
