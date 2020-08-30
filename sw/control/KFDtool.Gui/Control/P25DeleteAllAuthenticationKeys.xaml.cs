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
    /// Interaction logic for P25DeleteAllAuthenticationKeys.xaml
    /// </summary>
    public partial class P25DeleteAllAuthenticationKeys : UserControl
    {
        public P25DeleteAllAuthenticationKeys()
        {
            InitializeComponent();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Interact.DeleteAuthenticationKey(Settings.SelectedDevice, false, true, 0, 0, 0);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error -- {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBox.Show("All Keys Erased Successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
