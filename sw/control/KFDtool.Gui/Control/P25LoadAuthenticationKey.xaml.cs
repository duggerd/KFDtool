using KFDtool.P25.Generator;
using KFDtool.P25.Kmm;
using KFDtool.P25.TransferConstructs;
using KFDtool.P25.Validator;
using KFDtool.Shared;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace KFDtool.Gui.Control
{
    /// <summary>
    /// Interaction logic for P25LoadAuthenticationKey.xaml
    /// </summary>
    public partial class P25LoadAuthenticationKey : UserControl
    {
        public P25LoadAuthenticationKey()
        {
            InitializeComponent();

            ActiveSuId.IsChecked = true; // check here to trigger the cb/txt logic on load
            AlgSelect.SelectedIndex = 0; // set to the first item here to trigger the cbo/txt logic on load
            HideKey.IsChecked = true; // check here to trigger the cb/txt logic on load
        }

        private void UpdateDecToHex(TextBox dec, TextBox hex, bool bypassIsFocused)
        {
            if (dec.IsFocused || bypassIsFocused)
            {
                if (int.TryParse(dec.Text, out int num))
                {
                    hex.Text = string.Format("{0:X}", num);
                }
                else
                {
                    hex.Text = string.Empty;
                }
            }
        }

        private void UpdateHexToDec(TextBox hex, TextBox dec, bool bypassIsFocused)
        {
            if (hex.IsFocused || bypassIsFocused)
            {
                if (int.TryParse(hex.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int num))
                {
                    dec.Text = num.ToString();
                }
                else
                {
                    dec.Text = string.Empty;
                }
            }
        }

        private void WacnIdDec_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateDecToHex(WacnIdDec, WacnIdHex, false);
        }

        private void WacnIdHex_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateHexToDec(WacnIdHex, WacnIdDec, false);
        }

        private void ActiveSuId_Checked(object sender, RoutedEventArgs e)
        {
            WacnIdDec.Text = string.Empty;
            WacnIdDec.IsEnabled = false;

            WacnIdHex.Text = string.Empty;
            WacnIdHex.IsEnabled = false;

            SystemIdDec.Text = string.Empty;
            SystemIdDec.IsEnabled = false;

            SystemIdHex.Text = string.Empty;
            SystemIdHex.IsEnabled = false;

            UnitIdDec.Text = string.Empty;
            UnitIdDec.IsEnabled = false;

            UnitIdHex.Text = string.Empty;
            UnitIdHex.IsEnabled = false;
        }

        private void ActiveSuId_Unchecked(object sender, RoutedEventArgs e)
        {
            WacnIdDec.IsEnabled = true;
            WacnIdHex.IsEnabled = true;
            SystemIdDec.IsEnabled = true;
            SystemIdHex.IsEnabled = true;
            UnitIdDec.IsEnabled = true;
            UnitIdHex.IsEnabled = true;
        }

        private void SystemIdDec_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateDecToHex(SystemIdDec, SystemIdHex, false);
        }

        private void SystemIdHex_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateHexToDec(SystemIdHex, SystemIdDec, false);
        }

        private void UnitIdDec_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateDecToHex(UnitIdDec, UnitIdHex, false);
        }

        private void UnitIdHex_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateHexToDec(UnitIdHex, UnitIdDec, false);
        }

        private void AlgIdDec_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateDecToHex(AlgIdDec, AlgIdHex, false);
        }

        private void AlgIdHex_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateHexToDec(AlgIdHex, AlgIdDec, false);
        }

        private void AlgSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AlgSelect.SelectedItem != null)
            {
                string name = ((ComboBoxItem)AlgSelect.SelectedItem).Name;

                if (name == AlgorithmId.AES128.ToString())
                {
                    AlgIdDec.Text = ((int)AlgorithmId.AES128).ToString();

                    UpdateDecToHex(AlgIdDec, AlgIdHex, true);

                    AlgIdDec.IsEnabled = false;
                    AlgIdHex.IsEnabled = false;
                }
                else
                {
                    AlgIdDec.Text = string.Empty;
                    AlgIdDec.IsEnabled = true;

                    AlgIdHex.Text = string.Empty;
                    AlgIdHex.IsEnabled = true;
                }
            }
        }

        private string GetKey()
        {
            if (HideKey.IsChecked == true)
            {
                return KeyHidden.Password;
            }
            else
            {
                return KeyVisible.Text;
            }
        }

        private void SetKey(string str)
        {
            if (HideKey.IsChecked == true)
            {
                KeyHidden.Password = str;
            }
            else
            {
                KeyVisible.Text = str;
            }
        }

        private void Generate_Clicked(object sender, RoutedEventArgs e)
        {
            int algId;

            if (!int.TryParse(AlgIdDec.Text, out algId))
            {
                MessageBox.Show("Error Parsing Algorithm ID", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!FieldValidator.IsValidAlgorithmId(algId))
            {
                MessageBox.Show("Algorithm ID invalid - valid range 0 to 255 (dec), 0x00 to 0xFF (hex)", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            List<byte> key;

            if (algId == (byte)AlgorithmId.AES128)
            {
                key = KeyGenerator.GenerateVarKey(16);
            }
            // TODO something for non-standard auth key algos
            else
            {
                MessageBox.Show(string.Format("No key generator exists for algorithm ID {0} (0x{0:X2})", algId), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            SetKey(BitConverter.ToString(key.ToArray()).Replace("-", string.Empty));
        }

        private void HideKey_Checked(object sender, RoutedEventArgs e)
        {
            KeyHidden.Password = KeyVisible.Text;
            KeyVisible.Text = string.Empty;
            KeyVisible.Visibility = Visibility.Hidden;
            KeyHidden.Visibility = Visibility.Visible;
        }

        private void HideKey_Unchecked(object sender, RoutedEventArgs e)
        {
            KeyVisible.Text = KeyHidden.Password;
            KeyHidden.Password = null;
            KeyVisible.Visibility = Visibility.Visible;
            KeyHidden.Visibility = Visibility.Hidden;
        }

        private void Load_Clicked(object sender, RoutedEventArgs e)
        {
            bool targetSpecificSuId;
            int wacnId, systemId, unitId, algId;
            List<byte> key;

            if (ActiveSuId.IsChecked == true)
            {
                targetSpecificSuId = false;
                wacnId = 0;
                systemId = 0;
                unitId = 0;
            }
            else
            {
                targetSpecificSuId = true;

                if (!int.TryParse(WacnIdDec.Text, out wacnId))
                {
                    MessageBox.Show("Error Parsing WACN ID", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!int.TryParse(SystemIdDec.Text, out systemId))
                {
                    MessageBox.Show("Error Parsing System ID", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!int.TryParse(UnitIdDec.Text, out unitId))
                {
                    MessageBox.Show("Error Parsing Unit ID", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            if (!int.TryParse(AlgIdDec.Text, out algId))
            {
                MessageBox.Show("Error Parsing Algorithm ID", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                key = Utility.ByteStringToByteList(GetKey());
            }
            catch (Exception)
            {
                MessageBox.Show("Error Parsing Key", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

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

            MessageBox.Show("Key Loaded Successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
