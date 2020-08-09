using KFDtool.P25.TransferConstructs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace KFDtool.Gui.Control
{
    /// <summary>
    /// Interaction logic for P25ViewSuIdInfo.xaml
    /// </summary>
    public partial class P25ViewSuIdInfo : UserControl
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public P25ViewSuIdInfo()
        {
            InitializeComponent();
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            SuIdItems.ItemsSource = null; // clear table

            List<RspAuthKeyInfo> items;

            try
            {
                items = Interact.ViewSuIdInfo(Settings.SelectedDevice);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex);

                MessageBox.Show(string.Format("Error -- {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }

            if (items != null)
            {
                SuIdItems.ItemsSource = items;

                SuIdItems.Items.SortDescriptions.Add(new SortDescription("WacnId", ListSortDirection.Ascending));
                SuIdItems.Items.SortDescriptions.Add(new SortDescription("SystemId", ListSortDirection.Ascending));
                SuIdItems.Items.SortDescriptions.Add(new SortDescription("UnitId", ListSortDirection.Ascending));

                MessageBox.Show(string.Format("{0} item(s) returned", items.Count), "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
