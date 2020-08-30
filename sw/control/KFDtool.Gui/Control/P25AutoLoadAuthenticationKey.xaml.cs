using KFDtool.ImportExport;
using KFDtool.P25.TransferConstructs;
using KFDtool.P25.Validator;
using KFDtool.Shared;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace KFDtool.Gui.Control
{
    /// <summary>
    /// Interaction logic for P25AutoLoadAuthenticationKey.xaml
    /// </summary>
    public partial class P25AutoLoadAuthenticationKey : UserControl
    {
        private const string NO_FILE_SELECTED = "No File Selected";

        private string KeyFilePath;

        private List<HarrisLlak> Keys;

        public P25AutoLoadAuthenticationKey()
        {
            InitializeComponent();

            ResetState();
        }

        private void ResetState()
        {
            KeyFilePath = string.Empty;
            lblPath.Content = NO_FILE_SELECTED;
            btnLoad.IsEnabled = false;
            Keys = null;
        }

        private void Select_Button_Click(object sender, RoutedEventArgs e)
        {
            /*try
            {

            }
            catch (Exception ex)
            {

            }*/

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Harris UKEK File (*.ukek)|*.ukek|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                KeyFilePath = openFileDialog.FileName;
                lblPath.Content = KeyFilePath;

                if (KeyFilePath.Equals(string.Empty))
                {
                    MessageBox.Show("No file selected", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                byte[] data;

                try
                {
                    data = File.ReadAllBytes(KeyFilePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("Error reading file -- {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                HarrisUkekFile harrisUkekFile = new HarrisUkekFile();
                harrisUkekFile.ParseFile(data);
                Keys = harrisUkekFile.GetLlaks();

                btnLoad.IsEnabled = true;

                MessageBox.Show(string.Format("Read {0} LLAKs from Harris UKEK file", Keys.Count), "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Load_Button_Click(object sender, RoutedEventArgs e)
        {
            /*try
            {

            }
            catch (Exception ex)
            {

            }*/

            RspAuthKeyInfo activeSuId = Interact.ViewActiveSuIdInfo(Settings.SelectedDevice);

            HarrisLlak unitKey = null;

            foreach (HarrisLlak keyItem in Keys)
            {
                if ((keyItem.WacnId == activeSuId.WacnId) && (keyItem.SystemId == activeSuId.SystemId) && (keyItem.UnitId == activeSuId.UnitId))
                {
                    if (unitKey == null)
                    {
                        unitKey = keyItem;
                    }
                    else
                    {
                        MessageBox.Show(string.Format("UKEK file contains multiple LLAKs for the same SUID - WACN: {0}, SYS: {1}, UNIT: {2}", activeSuId.WacnId, activeSuId.SystemId, activeSuId.UnitId), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
            }

            if (unitKey == null)
            {
                MessageBox.Show(string.Format("UKEK file does not contain a LLAK for the active SUID - WACN: {0}, SYS: {1}, UNIT: {2}", activeSuId.WacnId, activeSuId.SystemId, activeSuId.UnitId), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            bool targetSpecificSuId = true;
            int wacnId = unitKey.WacnId;
            int systemId = unitKey.SystemId;
            int unitId = unitKey.UnitId;
            int algId = unitKey.AlgId;
            List<byte> key = unitKey.Key;

            (ValidateResult result, string message) = FieldValidator.AuthenticationKeyloadValidate(targetSpecificSuId, wacnId, systemId, unitId, algId, key);

            if (result == ValidateResult.Warning)
            {
                if (MessageBox.Show(string.Format("{1}{0}{0}Continue?", Environment.NewLine, message), "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.No)
                {
                    return;
                }
            }
            else if (result == ValidateResult.Error)
            {
                MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                Interact.LoadAuthenticationKey(Settings.SelectedDevice, targetSpecificSuId, wacnId, systemId, unitId, key.ToArray());
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error -- {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBox.Show(string.Format("Successfully loaded {4} LLA Key(s) to SUID:{0}{0}WACN ID: {1} (0x{1:X}){0}System ID: {2} (0x{2:X}){0}Unit ID: {3} (0x{3:X})", Environment.NewLine, wacnId, systemId, unitId, 1), "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
