using KFDtool.P25.ManualRekey;
using KFDtool.P25.NetworkProtocol;
using System;
using System.Collections.Generic;

namespace KFDtool.P25.TransferConstructs
{
    public class InteractDliIp
    {
        private static ManualRekeyApplication GetMra(DliIpDevice device)
        {
            if (device.ProtocolType == DliIpDevice.ProtocolOptions.UDP)
            {
                int timeout = 5000;

                UdpProtocol udpProtocol = new UdpProtocol(device.Hostname, device.Port, timeout);

                return new ManualRekeyApplication(udpProtocol, device.SessionControlType);
            }
            else
            {
                throw new ArgumentOutOfRangeException("Protocol");
            }
        }

        public static void CheckTargetMrConnection(DliIpDevice device)
        {
            try
            {
                ManualRekeyApplication mra = GetMra(device);

                mra.CheckTargetMrConnection();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void Keyload(DliIpDevice device, List<CmdKeyItem> keys)
        {
            try
            {
                ManualRekeyApplication mra = GetMra(device);

                mra.Keyload(keys);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void EraseKey(DliIpDevice device, List<CmdKeyItem> keys)
        {
            try
            {
                ManualRekeyApplication mra = GetMra(device);

                mra.EraseKeys(keys);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void EraseAllKeys(DliIpDevice device)
        {
            try
            {
                ManualRekeyApplication mra = GetMra(device);

                mra.EraseAllKeys();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<RspKeyInfo> ViewKeyInfo(DliIpDevice device)
        {
            try
            {
                ManualRekeyApplication mra = GetMra(device);

                return mra.ViewKeyInfo();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static RspRsiInfo LoadConfig(DliIpDevice device, int kmfRsi, int mnp)
        {
            try
            {
                ManualRekeyApplication mra = GetMra(device);

                return mra.LoadConfig(kmfRsi, mnp);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static RspRsiInfo ChangeRsi(DliIpDevice device, int rsiOld, int rsiNew, int mnp)
        {
            try
            {
                ManualRekeyApplication mra = GetMra(device);

                return mra.ChangeRsi(rsiOld, rsiNew, mnp);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<RspRsiInfo> ViewRsiItems(DliIpDevice device)
        {
            try
            {
                ManualRekeyApplication mra = GetMra(device);

                return mra.ViewRsiItems();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static int ViewMnp(DliIpDevice device)
        {
            try
            {
                ManualRekeyApplication mra = GetMra(device);

                return mra.ViewMnp();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static int ViewKmfRsi(DliIpDevice device)
        {
            try
            {
                ManualRekeyApplication mra = GetMra(device);

                return mra.ViewKmfRsi();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<RspKeysetInfo> ViewKeysetTaggingInfo(DliIpDevice device)
        {
            try
            {
                ManualRekeyApplication mra = GetMra(device);

                return mra.ViewKeysetTaggingInfo();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static RspChangeoverInfo ActivateKeyset(DliIpDevice device, int keysetSuperseded, int keysetActivated)
        {
            try
            {
                ManualRekeyApplication mra = GetMra(device);

                return mra.ActivateKeyset(keysetSuperseded, keysetActivated);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void LoadAuthenticationKey(DliIpDevice device, bool targetSpecificSuId, int wacnId, int systemId, int unitId, byte[] key)
        {
            try
            {
                ManualRekeyApplication mra = GetMra(device);

                mra.LoadAuthenticationKey(targetSpecificSuId, wacnId, systemId, unitId, key);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void DeleteAuthenticationKey(DliIpDevice device, bool targetSpecificSuId, bool deleteAllKeys, int wacnId, int systemId, int unitId)
        {
            try
            {
                ManualRekeyApplication mra = GetMra(device);

                mra.DeleteAuthenticationKey(targetSpecificSuId, deleteAllKeys, wacnId, systemId, unitId);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
