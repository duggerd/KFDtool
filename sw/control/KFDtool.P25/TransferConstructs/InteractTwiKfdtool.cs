using KFDtool.Adapter.Protocol.Adapter;
using KFDtool.P25.ManualRekey;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KFDtool.P25.TransferConstructs
{
    public class InteractTwiKfdtool
    {
        private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static string ReadAdapterProtocolVersion(TwiKfdtoolDevice device)
        {
            string version = string.Empty;

            if (device.ComPort == string.Empty)
            {
                throw new ArgumentException("No device selected");
            }

            AdapterProtocol ap = null;

            try
            {
                ap = new AdapterProtocol(device.ComPort);

                ap.Open();

                ap.Clear();

                byte[] ver = ap.ReadAdapterProtocolVersion();

                version = string.Format("{0}.{1}.{2}", ver[0], ver[1], ver[2]);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                try
                {
                    if (ap != null)
                    {
                        ap.Close();
                    }
                }
                catch (IOException ex)
                {
                    Logger.Warn("could not close serial port: {0}", ex.Message);
                }
            }

            return version;
        }

        public static string ReadFirmwareVersion(TwiKfdtoolDevice device)
        {
            string version = string.Empty;

            if (device.ComPort == string.Empty)
            {
                throw new ArgumentException("No device selected");
            }

            AdapterProtocol ap = null;

            try
            {
                ap = new AdapterProtocol(device.ComPort);

                ap.Open();

                ap.Clear();

                byte[] ver = ap.ReadFirmwareVersion();

                version = string.Format("{0}.{1}.{2}", ver[0], ver[1], ver[2]);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                try
                {
                    if (ap != null)
                    {
                        ap.Close();
                    }
                }
                catch (IOException ex)
                {
                    Logger.Warn("could not close serial port: {0}", ex.Message);
                }
            }

            return version;
        }

        public static string ReadUniqueId(TwiKfdtoolDevice device)
        {
            string uniqueId = string.Empty;

            if (device.ComPort == string.Empty)
            {
                throw new ArgumentException("No device selected");
            }

            AdapterProtocol ap = null;

            try
            {
                ap = new AdapterProtocol(device.ComPort);

                ap.Open();

                ap.Clear();

                byte[] id = ap.ReadUniqueId();

                if (id.Length == 0)
                {
                    uniqueId = "NONE";
                }
                else
                {
                    uniqueId = BitConverter.ToString(id).Replace("-", string.Empty);

                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                try
                {
                    if (ap != null)
                    {
                        ap.Close();
                    }
                }
                catch (IOException ex)
                {
                    Logger.Warn("could not close serial port: {0}", ex.Message);
                }
            }

            return uniqueId;
        }

        public static string ReadModel(TwiKfdtoolDevice device)
        {
            string model = string.Empty;

            if (device.ComPort == string.Empty)
            {
                throw new ArgumentException("No device selected");
            }

            AdapterProtocol ap = null;

            try
            {
                ap = new AdapterProtocol(device.ComPort);

                ap.Open();

                ap.Clear();

                byte mod = ap.ReadModelId();

                if (mod == 0x00)
                {
                    model = "NOT SET";
                }
                else if (mod == 0x01)
                {
                    model = "KFD100";
                }
                else
                {
                    model = "UNKNOWN";
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                try
                {
                    if (ap != null)
                    {
                        ap.Close();
                    }
                }
                catch (IOException ex)
                {
                    Logger.Warn("could not close serial port: {0}", ex.Message);
                }
            }

            return model;
        }

        public static string ReadHardwareRevision(TwiKfdtoolDevice device)
        {
            string version = string.Empty;

            if (device.ComPort == string.Empty)
            {
                throw new ArgumentException("No device selected");
            }

            AdapterProtocol ap = null;

            try
            {
                ap = new AdapterProtocol(device.ComPort);

                ap.Open();

                ap.Clear();

                byte[] ver = ap.ReadHardwareRevision();

                if (ver[0] == 0x00 && ver[1] == 0x00)
                {
                    version = "NOT SET";
                }
                else
                {
                    version = string.Format("{0}.{1}", ver[0], ver[1]);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                try
                {
                    if (ap != null)
                    {
                        ap.Close();
                    }
                }
                catch (IOException ex)
                {
                    Logger.Warn("could not close serial port: {0}", ex.Message);
                }
            }

            return version;
        }

        public static string ReadSerialNumber(TwiKfdtoolDevice device)
        {
            string serialNumber = string.Empty;

            if (device.ComPort == string.Empty)
            {
                throw new ArgumentException("No device selected");
            }

            AdapterProtocol ap = null;

            try
            {
                ap = new AdapterProtocol(device.ComPort);

                ap.Open();

                ap.Clear();

                byte[] ser = ap.ReadSerialNumber();

                if (ser.Length == 0)
                {
                    serialNumber = "NONE";
                }
                else
                {
                    serialNumber = Encoding.ASCII.GetString(ser);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                try
                {
                    if (ap != null)
                    {
                        ap.Close();
                    }
                }
                catch (IOException ex)
                {
                    Logger.Warn("could not close serial port: {0}", ex.Message);
                }
            }

            return serialNumber;
        }

        public static void EnterBslMode(TwiKfdtoolDevice device)
        {
            if (device.ComPort == string.Empty)
            {
                throw new ArgumentException("No device selected");
            }

            AdapterProtocol ap = null;

            try
            {
                ap = new AdapterProtocol(device.ComPort);

                ap.Open();

                ap.Clear();

                ap.EnterBslMode();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                try
                {
                    if (ap != null)
                    {
                        ap.Close();
                    }
                }
                catch (IOException ex)
                {
                    Logger.Warn("could not close serial port: {0}", ex.Message);
                }
            }
        }

        public static string SelfTest(TwiKfdtoolDevice device)
        {
            string result = string.Empty;

            if (device.ComPort == string.Empty)
            {
                throw new ArgumentException("No device selected");
            }

            AdapterProtocol ap = null;

            try
            {
                ap = new AdapterProtocol(device.ComPort);

                ap.Open();

                ap.Clear();

                byte res = ap.SelfTest();

                if (res == 0x01)
                {
                    result = string.Format("Data shorted to ground (0x{0:X2})", res);
                }
                else if (res == 0x02)
                {
                    result = string.Format("Sense shorted to ground (0x{0:X2})", res);
                }
                else if (res == 0x03)
                {
                    result = string.Format("Data shorted to power (0x{0:X2})", res);
                }
                else if (res == 0x04)
                {
                    result = string.Format("Sense shorted to power (0x{0:X2})", res);
                }
                else if (res == 0x05)
                {
                    result = string.Format("Data and Sense shorted (0x{0:X2})", res);
                }
                else if (res == 0x06)
                {
                    result = string.Format("Sense and Data shorted (0x{0:X2})", res);
                }
                else if (res != 0x00)
                {
                    result = string.Format("Unknown self test result (0x{0:X2})", res);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                try
                {
                    if (ap != null)
                    {
                        ap.Close();
                    }
                }
                catch (IOException ex)
                {
                    Logger.Warn("could not close serial port: {0}", ex.Message);
                }
            }

            return result;
        }

        public static void CheckTargetMrConnection(TwiKfdtoolDevice device)
        {
            if (device.ComPort == string.Empty)
            {
                throw new ArgumentException("No device selected");
            }

            AdapterProtocol ap = null;

            try
            {
                ap = new AdapterProtocol(device.ComPort);

                ap.Open();

                ap.Clear();

                ManualRekeyApplication mra = new ManualRekeyApplication(ap);

                mra.CheckTargetMrConnection();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                try
                {
                    if (ap != null)
                    {
                        ap.Close();
                    }
                }
                catch (IOException ex)
                {
                    Logger.Warn("could not close serial port: {0}", ex.Message);
                }
            }
        }

        public static void Keyload(TwiKfdtoolDevice device, List<CmdKeyItem> keys)
        {
            if (device.ComPort == string.Empty)
            {
                throw new ArgumentException("No device selected");
            }

            AdapterProtocol ap = null;

            try
            {
                ap = new AdapterProtocol(device.ComPort);

                ap.Open();

                ap.Clear();

                ManualRekeyApplication mra = new ManualRekeyApplication(ap);

                mra.Keyload(keys);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                try
                {
                    if (ap != null)
                    {
                        ap.Close();
                    }
                }
                catch (IOException ex)
                {
                    Logger.Warn("could not close serial port: {0}", ex.Message);
                }
            }
        }

        public static void EraseKey(TwiKfdtoolDevice device, List<CmdKeyItem> keys)
        {
            if (device.ComPort == string.Empty)
            {
                throw new ArgumentException("No device selected");
            }

            AdapterProtocol ap = null;

            try
            {
                ap = new AdapterProtocol(device.ComPort);

                ap.Open();

                ap.Clear();

                ManualRekeyApplication mra = new ManualRekeyApplication(ap);

                mra.EraseKeys(keys);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                try
                {
                    if (ap != null)
                    {
                        ap.Close();
                    }
                }
                catch (IOException ex)
                {
                    Logger.Warn("could not close serial port: {0}", ex.Message);
                }
            }
        }

        public static void EraseAllKeys(TwiKfdtoolDevice device)
        {
            if (device.ComPort == string.Empty)
            {
                throw new ArgumentException("No device selected");
            }

            AdapterProtocol ap = null;

            try
            {
                ap = new AdapterProtocol(device.ComPort);

                ap.Open();

                ap.Clear();

                ManualRekeyApplication mra = new ManualRekeyApplication(ap);

                mra.EraseAllKeys();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                try
                {
                    if (ap != null)
                    {
                        ap.Close();
                    }
                }
                catch (IOException ex)
                {
                    Logger.Warn("could not close serial port: {0}", ex.Message);
                }
            }
        }

        public static List<RspKeyInfo> ViewKeyInfo(TwiKfdtoolDevice device)
        {
            List<RspKeyInfo> result = new List<RspKeyInfo>();

            if (device.ComPort == string.Empty)
            {
                throw new ArgumentException("No device selected");
            }

            AdapterProtocol ap = null;

            try
            {
                ap = new AdapterProtocol(device.ComPort);

                ap.Open();

                ap.Clear();

                ManualRekeyApplication mra = new ManualRekeyApplication(ap);

                result.AddRange(mra.ViewKeyInfo());
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                try
                {
                    if (ap != null)
                    {
                        ap.Close();
                    }
                }
                catch (IOException ex)
                {
                    Logger.Warn("could not close serial port: {0}", ex.Message);
                }
            }

            return result;
        }

        public static RspRsiInfo LoadConfig(TwiKfdtoolDevice device, int kmfRsi, int mnp)
        {
            RspRsiInfo result = new RspRsiInfo();

            if (device.ComPort == string.Empty)
            {
                throw new ArgumentException("No device selected");
            }

            AdapterProtocol ap = null;

            try
            {
                ap = new AdapterProtocol(device.ComPort);

                ap.Open();

                ap.Clear();

                ManualRekeyApplication mra = new ManualRekeyApplication(ap);

                result = mra.LoadConfig(kmfRsi, mnp);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                try
                {
                    if (ap != null)
                    {
                        ap.Close();
                    }
                }
                catch (IOException ex)
                {
                    Logger.Warn("could not close serial port: {0}", ex.Message);
                }
            }

            return result;
        }

        public static RspRsiInfo ChangeRsi(TwiKfdtoolDevice device, int rsiOld, int rsiNew, int mnp)
        {
            RspRsiInfo result = new RspRsiInfo();

            if (device.ComPort == string.Empty)
            {
                throw new ArgumentException("No device selected");
            }

            AdapterProtocol ap = null;

            try
            {
                ap = new AdapterProtocol(device.ComPort);

                ap.Open();

                ap.Clear();

                ManualRekeyApplication mra = new ManualRekeyApplication(ap);

                result = mra.ChangeRsi(rsiOld, rsiNew, mnp);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                try
                {
                    if (ap != null)
                    {
                        ap.Close();
                    }
                }
                catch (IOException ex)
                {
                    Logger.Warn("could not close serial port: {0}", ex.Message);
                }
            }

            return result;
        }

        public static List<RspRsiInfo> ViewRsiItems(TwiKfdtoolDevice device)
        {
            List<RspRsiInfo> result = new List<RspRsiInfo>();

            if (device.ComPort == string.Empty)
            {
                throw new ArgumentException("No device selected");
            }

            AdapterProtocol ap = null;

            try
            {
                ap = new AdapterProtocol(device.ComPort);

                ap.Open();

                ap.Clear();

                ManualRekeyApplication mra = new ManualRekeyApplication(ap);

                result.AddRange(mra.ViewRsiItems());
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                try
                {
                    if (ap != null)
                    {
                        ap.Close();
                    }
                }
                catch (IOException ex)
                {
                    Logger.Warn("could not close serial port: {0}", ex.Message);
                }
            }

            return result;
        }

        public static int ViewMnp(TwiKfdtoolDevice device)
        {
            int result = new int();

            if (device.ComPort == string.Empty)
            {
                throw new ArgumentException("No device selected");
            }

            AdapterProtocol ap = null;

            try
            {
                ap = new AdapterProtocol(device.ComPort);

                ap.Open();

                ap.Clear();

                ManualRekeyApplication mra = new ManualRekeyApplication(ap);

                result = mra.ViewMnp();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                try
                {
                    if (ap != null)
                    {
                        ap.Close();
                    }
                }
                catch (IOException ex)
                {
                    Logger.Warn("could not close serial port: {0}", ex.Message);
                }
            }

            return result;
        }

        public static int ViewKmfRsi(TwiKfdtoolDevice device)
        {
            int result = new int();

            if (device.ComPort == string.Empty)
            {
                throw new ArgumentException("No device selected");
            }

            AdapterProtocol ap = null;

            try
            {
                ap = new AdapterProtocol(device.ComPort);

                ap.Open();

                ap.Clear();

                ManualRekeyApplication mra = new ManualRekeyApplication(ap);

                result = mra.ViewKmfRsi();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                try
                {
                    if (ap != null)
                    {
                        ap.Close();
                    }
                }
                catch (IOException ex)
                {
                    Logger.Warn("could not close serial port: {0}", ex.Message);
                }
            }

            return result;
        }

        public static List<RspKeysetInfo> ViewKeysetTaggingInfo(TwiKfdtoolDevice device)
        {
            List<RspKeysetInfo> result = new List<RspKeysetInfo>();

            if (device.ComPort == string.Empty)
            {
                throw new ArgumentException("No device selected");
            }

            AdapterProtocol ap = null;

            try
            {
                ap = new AdapterProtocol(device.ComPort);

                ap.Open();

                ap.Clear();

                ManualRekeyApplication mra = new ManualRekeyApplication(ap);

                result = mra.ViewKeysetTaggingInfo();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                try
                {
                    if (ap != null)
                    {
                        ap.Close();
                    }
                }
                catch (IOException ex)
                {
                    Logger.Warn("could not close serial port: {0}", ex.Message);
                }
            }

            return result;
        }

        public static RspChangeoverInfo ActivateKeyset(TwiKfdtoolDevice device, int keysetSuperseded, int keysetActivated)
        {
            RspChangeoverInfo result = new RspChangeoverInfo();

            if (device.ComPort == string.Empty)
            {
                throw new ArgumentException("No device selected");
            }

            AdapterProtocol ap = null;

            try
            {
                ap = new AdapterProtocol(device.ComPort);

                ap.Open();

                ap.Clear();

                ManualRekeyApplication mra = new ManualRekeyApplication(ap);

                //result = mra.LoadConfig(kmfRsi, mnp);
                result = mra.ActivateKeyset(keysetSuperseded, keysetActivated);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                try
                {
                    if (ap != null)
                    {
                        ap.Close();
                    }
                }
                catch (IOException ex)
                {
                    Logger.Warn("could not close serial port: {0}", ex.Message);
                }
            }

            return result;
        }

        public static void LoadAuthenticationKey(TwiKfdtoolDevice device, bool targetSpecificSuId, int wacnId, int systemId, int unitId, byte[] key)
        {
            if (device.ComPort == string.Empty)
            {
                throw new ArgumentException("No device selected");
            }

            AdapterProtocol ap = null;

            try
            {
                ap = new AdapterProtocol(device.ComPort);

                ap.Open();

                ap.Clear();

                ManualRekeyApplication mra = new ManualRekeyApplication(ap);

                mra.LoadAuthenticationKey(targetSpecificSuId, wacnId, systemId, unitId, key);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                try
                {
                    if (ap != null)
                    {
                        ap.Close();
                    }
                }
                catch (IOException ex)
                {
                    Logger.Warn("could not close serial port: {0}", ex.Message);
                }
            }
        }

        public static void DeleteAuthenticationKey(TwiKfdtoolDevice device, bool targetSpecificSuId, bool deleteAllKeys, int wacnId, int systemId, int unitId)
        {
            if (device.ComPort == string.Empty)
            {
                throw new ArgumentException("No device selected");
            }

            AdapterProtocol ap = null;

            try
            {
                ap = new AdapterProtocol(device.ComPort);

                ap.Open();

                ap.Clear();

                ManualRekeyApplication mra = new ManualRekeyApplication(ap);

                mra.DeleteAuthenticationKey(targetSpecificSuId, deleteAllKeys, wacnId, systemId, unitId);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                try
                {
                    if (ap != null)
                    {
                        ap.Close();
                    }
                }
                catch (IOException ex)
                {
                    Logger.Warn("could not close serial port: {0}", ex.Message);
                }
            }
        }

        public static List<RspAuthKeyInfo> ViewSuIdInfo(TwiKfdtoolDevice device)
        {
            if (device.ComPort == string.Empty)
            {
                throw new ArgumentException("No device selected");
            }

            AdapterProtocol ap = null;

            try
            {
                ap = new AdapterProtocol(device.ComPort);

                ap.Open();

                ap.Clear();

                ManualRekeyApplication mra = new ManualRekeyApplication(ap);

                return mra.ViewSuIdInfo();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                try
                {
                    if (ap != null)
                    {
                        ap.Close();
                    }
                }
                catch (IOException ex)
                {
                    Logger.Warn("could not close serial port: {0}", ex.Message);
                }
            }
        }

        public static RspAuthKeyInfo ViewActiveSuIdInfo(TwiKfdtoolDevice device)
        {
            if (device.ComPort == string.Empty)
            {
                throw new ArgumentException("No device selected");
            }

            AdapterProtocol ap = null;

            try
            {
                ap = new AdapterProtocol(device.ComPort);

                ap.Open();

                ap.Clear();

                ManualRekeyApplication mra = new ManualRekeyApplication(ap);

                return mra.ViewActiveSuIdInfo();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                try
                {
                    if (ap != null)
                    {
                        ap.Close();
                    }
                }
                catch (IOException ex)
                {
                    Logger.Warn("could not close serial port: {0}", ex.Message);
                }
            }
        }
    }
}
