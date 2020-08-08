using KFDtool.P25.TransferConstructs;
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

namespace KFDtool.Gui.Control
{
    /// <summary>
    /// Interaction logic for P25DeleteAuthenticationKey.xaml
    /// </summary>
    public partial class P25DeleteAuthenticationKey : UserControl
    {
        public P25DeleteAuthenticationKey()
        {
            InitializeComponent();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Interact.DeleteAuthenticationKey(Settings.SelectedDevice, true, false, 0xA4398, 0xF10, 0x99B584);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error -- {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBox.Show("Key Deleted Successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
