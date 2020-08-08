using KFDtool.P25.TransferConstructs;
using System;
using System.Windows;

namespace KFDtool.Gui.Dialog
{
    /// <summary>
    /// Interaction logic for DliIpDeviceEdit.xaml
    /// </summary>
    public partial class DliIpDeviceEdit : Window
    {
        public DliIpDeviceEdit()
        {
            InitializeComponent();

            // protocol

            if (Settings.SelectedDevice.DliIpDevice.ProtocolType == DliIpDevice.ProtocolOptions.UDP)
            {
                PcbProtocol.SelectedItem = PcbiProtocolUdp;
            }
            else
            {
                throw new Exception("unknown DliIpProtocol setting");
            }

            // hostname

            TbHostname.Text = Settings.SelectedDevice.DliIpDevice.Hostname;

            // port

            TbPort.Text = Settings.SelectedDevice.DliIpDevice.Port.ToString();

            // session control

            if (Settings.SelectedDevice.DliIpDevice.SessionControlType == SessionControlOptions.None)
            {
                CbSessionControl.SelectedItem = CbiSessionControlNone;
            }
            else if (Settings.SelectedDevice.DliIpDevice.SessionControlType == SessionControlOptions.Standard)
            {
                CbSessionControl.SelectedItem = CbiSessionControlStandard;
            }
            else if (Settings.SelectedDevice.DliIpDevice.SessionControlType == SessionControlOptions.Motorola)
            {
                CbSessionControl.SelectedItem = CbiSessionControlMotorola;
            }
            else
            {
                throw new Exception("unknown DliIpVariant setting");
            }
        }

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            // protocol

            if (PcbProtocol.SelectedItem == PcbiProtocolUdp)
            {
                Settings.SelectedDevice.DliIpDevice.ProtocolType = DliIpDevice.ProtocolOptions.UDP;
            }
            else
            {
                throw new Exception("unknown PcbProtocol selection");
            }

            // hostname

            Settings.SelectedDevice.DliIpDevice.Hostname = TbHostname.Text;

            // port

            int port;

            if (int.TryParse(TbPort.Text, out port))
            {
                if (port >= 0 && port <= 65535)
                {
                    Settings.SelectedDevice.DliIpDevice.Port = port;
                }
                else
                {
                    MessageBox.Show("Valid port range is 0-65535", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Could not parse port", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // session control

            if (CbSessionControl.SelectedItem == CbiSessionControlNone)
            {
                Settings.SelectedDevice.DliIpDevice.SessionControlType = SessionControlOptions.None;
            }
            else if (CbSessionControl.SelectedItem == CbiSessionControlStandard)
            {
                Settings.SelectedDevice.DliIpDevice.SessionControlType = SessionControlOptions.Standard;
            }
            else if (CbSessionControl.SelectedItem == CbiSessionControlMotorola)
            {
                Settings.SelectedDevice.DliIpDevice.SessionControlType = SessionControlOptions.Motorola;
            }
            else
            {
                throw new Exception("unknown CbSessionControl selection");
            }

            Close();
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
