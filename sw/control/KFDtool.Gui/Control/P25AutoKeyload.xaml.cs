using KFDtool.ImportExport;
using KFDtool.P25.TransferConstructs;
using KFDtool.P25.Validator;
using KFDtool.Shared;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for P25AutoKeyload.xaml
    /// </summary>
    public partial class P25AutoKeyload : UserControl
    {
        private const string NO_FILE_SELECTED = "No File Selected";

        private string KeyFilePath;

        private List<HarrisKek> Keys;

        public P25AutoKeyload()
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
                Keys = harrisUkekFile.GetKeks();

                btnLoad.IsEnabled = true;

                MessageBox.Show(string.Format("Read {0} KEKs from Harris UKEK file", Keys.Count), "Information", MessageBoxButton.OK, MessageBoxImage.Information);
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

            List<RspRsiInfo> rsiItems = Interact.ViewRsiItems(Settings.SelectedDevice);

            if (rsiItems.Count == 0)
            {
                MessageBox.Show("Target does not have any RSIs", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            List<CmdKeyItem> keys = new List<CmdKeyItem>();

            foreach (RspRsiInfo rsiInfo in rsiItems)
            {
                foreach (HarrisKek kekItem in Keys)
                {
                    if (rsiInfo.RSI == kekItem.Rsi)
                    {
                        CmdKeyItem keyItem = new CmdKeyItem();

                        keyItem.UseActiveKeyset = false;
                        keyItem.KeysetId = kekItem.KeysetId;
                        keyItem.Sln = kekItem.Sln;
                        keyItem.IsKek = true;
                        keyItem.KeyId = kekItem.KeyId;
                        keyItem.AlgorithmId = kekItem.AlgorithmId;
                        keyItem.Key = kekItem.Key;

                        (ValidateResult result, string message) = FieldValidator.KeyloadValidate(keyItem.KeysetId, keyItem.Sln, keyItem.IsKek, keyItem.KeyId, keyItem.AlgorithmId, keyItem.Key);

                        if (result == ValidateResult.Warning)
                        {
                            if (MessageBox.Show(string.Format("{1}{0}{0}Continue?", Environment.NewLine, message), "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                            {
                                return;
                            }
                        }
                        else if (result == ValidateResult.Error)
                        {
                            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        keys.Add(keyItem);
                    }
                }
            }

            if (keys.Count == 0)
            {
                MessageBox.Show("UKEK file does not contain any KEKs matching the target RSI", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                Interact.Keyload(Settings.SelectedDevice, keys);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error -- {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBox.Show("Key(s) Loaded Successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
