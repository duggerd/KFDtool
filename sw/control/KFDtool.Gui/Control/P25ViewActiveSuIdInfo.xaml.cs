using KFDtool.P25.TransferConstructs;
using System;
using System.Windows;
using System.Windows.Controls;

namespace KFDtool.Gui.Control
{
    /// <summary>
    /// Interaction logic for P25ViewActiveSuIdInfo.xaml
    /// </summary>
    public partial class P25ViewActiveSuIdInfo : UserControl
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public P25ViewActiveSuIdInfo()
        {
            InitializeComponent();
        }

        private void Read_Click(object sender, RoutedEventArgs e)
        {
            WacnIdDec.Text = string.Empty;
            WacnIdHex.Text = string.Empty;
            SystemIdDec.Text = string.Empty;
            SystemIdHex.Text = string.Empty;
            UnitIdDec.Text = string.Empty;
            UnitIdHex.Text = string.Empty;
            KeyAssigned.Text = string.Empty;
            Active.Text = string.Empty;

            try
            {
                RspAuthKeyInfo activeSuId = Interact.ViewActiveSuIdInfo(Settings.SelectedDevice);

                WacnIdDec.Text = activeSuId.WacnId.ToString();
                WacnIdHex.Text = activeSuId.WacnId.ToString("X");
                SystemIdDec.Text = activeSuId.SystemId.ToString();
                SystemIdHex.Text = activeSuId.SystemId.ToString("X");
                UnitIdDec.Text = activeSuId.UnitId.ToString();
                UnitIdHex.Text = activeSuId.UnitId.ToString("X");
                KeyAssigned.Text = activeSuId.KeyAssigned.ToString();
                Active.Text = activeSuId.ActiveSuId.ToString();
            }
            catch (Exception ex)
            {
                Logger.Warn(ex);

                MessageBox.Show(string.Format("Error -- {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }

            MessageBox.Show("Active SUID Read Successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
