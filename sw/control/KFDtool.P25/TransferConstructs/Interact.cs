using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KFDtool.P25.TransferConstructs
{
    public class Interact
    {
        public static string ReadAdapterProtocolVersion(BaseDevice device)
        {
            if (device.DeviceType == BaseDevice.DeviceTypeOptions.TwiKfdtool)
            {
                return InteractTwiKfdtool.ReadAdapterProtocolVersion(device.TwiKfdtoolDevice);
            }
            else
            {
                throw new Exception(string.Format("The device type {0} does not support ReadAdapterProtocolVersion", device.DeviceType.ToString()));
            }
        }

        public static string ReadFirmwareVersion(BaseDevice device)
        {
            if (device.DeviceType == BaseDevice.DeviceTypeOptions.TwiKfdtool)
            {
                return InteractTwiKfdtool.ReadFirmwareVersion(device.TwiKfdtoolDevice);
            }
            else
            {
                throw new Exception(string.Format("The device type {0} does not support ReadFirmwareVersion", device.DeviceType.ToString()));
            }
        }

        public static string ReadUniqueId(BaseDevice device)
        {
            if (device.DeviceType == BaseDevice.DeviceTypeOptions.TwiKfdtool)
            {
                return InteractTwiKfdtool.ReadUniqueId(device.TwiKfdtoolDevice);
            }
            else
            {
                throw new Exception(string.Format("The device type {0} does not support ReadUniqueId", device.DeviceType.ToString()));
            }
        }

        public static string ReadModel(BaseDevice device)
        {
            if (device.DeviceType == BaseDevice.DeviceTypeOptions.TwiKfdtool)
            {
                return InteractTwiKfdtool.ReadModel(device.TwiKfdtoolDevice);
            }
            else
            {
                throw new Exception(string.Format("The device type {0} does not support ReadModel", device.DeviceType.ToString()));
            }
        }

        public static string ReadHardwareRevision(BaseDevice device)
        {
            if (device.DeviceType == BaseDevice.DeviceTypeOptions.TwiKfdtool)
            {
                return InteractTwiKfdtool.ReadHardwareRevision(device.TwiKfdtoolDevice);
            }
            else
            {
                throw new Exception(string.Format("The device type {0} does not support ReadHardwareRevision", device.DeviceType.ToString()));
            }
        }

        public static string ReadSerialNumber(BaseDevice device)
        {
            if (device.DeviceType == BaseDevice.DeviceTypeOptions.TwiKfdtool)
            {
                return InteractTwiKfdtool.ReadSerialNumber(device.TwiKfdtoolDevice);
            }
            else
            {
                throw new Exception(string.Format("The device type {0} does not support ReadSerialNumber", device.DeviceType.ToString()));
            }
        }

        public static void EnterBslMode(BaseDevice device)
        {
            if (device.DeviceType == BaseDevice.DeviceTypeOptions.TwiKfdtool)
            {
                InteractTwiKfdtool.EnterBslMode(device.TwiKfdtoolDevice);
            }
            else
            {
                throw new Exception(string.Format("The device type {0} does not support EnterBslMode", device.DeviceType.ToString()));
            }
        }

        public static string SelfTest(BaseDevice device)
        {
            if (device.DeviceType == BaseDevice.DeviceTypeOptions.TwiKfdtool)
            {
                return InteractTwiKfdtool.SelfTest(device.TwiKfdtoolDevice);
            }
            else
            {
                throw new Exception(string.Format("The device type {0} does not support SelfTest", device.DeviceType.ToString()));
            }
        }

        public static void CheckTargetMrConnection(BaseDevice device)
        {
            if (device.DeviceType == BaseDevice.DeviceTypeOptions.TwiKfdtool)
            {
                InteractTwiKfdtool.CheckTargetMrConnection(device.TwiKfdtoolDevice);
            }
            else if (device.DeviceType == BaseDevice.DeviceTypeOptions.DliIp)
            {
                InteractDliIp.CheckTargetMrConnection(device.DliIpDevice);
            }
            else
            {
                throw new Exception(string.Format("The device type {0} does not support CheckTargetMrConnection", device.DeviceType.ToString()));
            }
        }

        public static void Keyload(BaseDevice device, List<CmdKeyItem> keys)
        {
            if (device.DeviceType == BaseDevice.DeviceTypeOptions.TwiKfdtool)
            {
                InteractTwiKfdtool.Keyload(device.TwiKfdtoolDevice, keys);
            }
            else if (device.DeviceType == BaseDevice.DeviceTypeOptions.DliIp)
            {
                InteractDliIp.Keyload(device.DliIpDevice, keys);
            }
            else
            {
                throw new Exception(string.Format("The device type {0} does not support Keyload", device.DeviceType.ToString()));
            }
        }

        public static void EraseKey(BaseDevice device, List<CmdKeyItem> keys)
        {
            if (device.DeviceType == BaseDevice.DeviceTypeOptions.TwiKfdtool)
            {
                InteractTwiKfdtool.EraseKey(device.TwiKfdtoolDevice, keys);
            }
            else if (device.DeviceType == BaseDevice.DeviceTypeOptions.DliIp)
            {
                InteractDliIp.EraseKey(device.DliIpDevice, keys);
            }
            else
            {
                throw new Exception(string.Format("The device type {0} does not support EraseKey", device.DeviceType.ToString()));
            }
        }

        public static void EraseAllKeys(BaseDevice device)
        {
            if (device.DeviceType == BaseDevice.DeviceTypeOptions.TwiKfdtool)
            {
                InteractTwiKfdtool.EraseAllKeys(device.TwiKfdtoolDevice);
            }
            else if (device.DeviceType == BaseDevice.DeviceTypeOptions.DliIp)
            {
                InteractDliIp.EraseAllKeys(device.DliIpDevice);
            }
            else
            {
                throw new Exception(string.Format("The device type {0} does not support EraseAllKeys", device.DeviceType.ToString()));
            }
        }

        public static List<RspKeyInfo> ViewKeyInfo(BaseDevice device)
        {
            if (device.DeviceType == BaseDevice.DeviceTypeOptions.TwiKfdtool)
            {
                return InteractTwiKfdtool.ViewKeyInfo(device.TwiKfdtoolDevice);
            }
            else if (device.DeviceType == BaseDevice.DeviceTypeOptions.DliIp)
            {
                return InteractDliIp.ViewKeyInfo(device.DliIpDevice);
            }
            else
            {
                throw new Exception(string.Format("The device type {0} does not support ViewKeyInfo", device.DeviceType.ToString()));
            }
        }

        public static RspRsiInfo LoadConfig(BaseDevice device, int kmfRsi, int mnp)
        {
            if (device.DeviceType == BaseDevice.DeviceTypeOptions.TwiKfdtool)
            {
                return InteractTwiKfdtool.LoadConfig(device.TwiKfdtoolDevice, kmfRsi, mnp);
            }
            else if (device.DeviceType == BaseDevice.DeviceTypeOptions.DliIp)
            {
                return InteractDliIp.LoadConfig(device.DliIpDevice, kmfRsi, mnp);
            }
            else
            {
                throw new Exception(string.Format("The device type {0} does not support LoadConfig", device.DeviceType.ToString()));
            }
        }

        public static RspRsiInfo ChangeRsi(BaseDevice device, int rsiOld, int rsiNew, int mnp)
        {
            if (device.DeviceType == BaseDevice.DeviceTypeOptions.TwiKfdtool)
            {
                return InteractTwiKfdtool.ChangeRsi(device.TwiKfdtoolDevice, rsiOld, rsiNew, mnp);
            }
            else if (device.DeviceType == BaseDevice.DeviceTypeOptions.DliIp)
            {
                return InteractDliIp.ChangeRsi(device.DliIpDevice, rsiOld, rsiNew, mnp);
            }
            else
            {
                throw new Exception(string.Format("The device type {0} does not support ChangeRsi", device.DeviceType.ToString()));
            }
        }

        public static List<RspRsiInfo> ViewRsiItems(BaseDevice device)
        {
            if (device.DeviceType == BaseDevice.DeviceTypeOptions.TwiKfdtool)
            {
                return InteractTwiKfdtool.ViewRsiItems(device.TwiKfdtoolDevice);
            }
            else if (device.DeviceType == BaseDevice.DeviceTypeOptions.DliIp)
            {
                return InteractDliIp.ViewRsiItems(device.DliIpDevice);
            }
            else
            {
                throw new Exception(string.Format("The device type {0} does not support ViewRsiItems", device.DeviceType.ToString()));
            }
        }

        public static int ViewMnp(BaseDevice device)
        {
            if (device.DeviceType == BaseDevice.DeviceTypeOptions.TwiKfdtool)
            {
                return InteractTwiKfdtool.ViewMnp(device.TwiKfdtoolDevice);
            }
            else if (device.DeviceType == BaseDevice.DeviceTypeOptions.DliIp)
            {
                return InteractDliIp.ViewMnp(device.DliIpDevice);
            }
            else
            {
                throw new Exception(string.Format("The device type {0} does not support ViewMnp", device.DeviceType.ToString()));
            }
        }

        public static int ViewKmfRsi(BaseDevice device)
        {
            if (device.DeviceType == BaseDevice.DeviceTypeOptions.TwiKfdtool)
            {
                return InteractTwiKfdtool.ViewKmfRsi(device.TwiKfdtoolDevice);
            }
            else if (device.DeviceType == BaseDevice.DeviceTypeOptions.DliIp)
            {
                return InteractDliIp.ViewKmfRsi(device.DliIpDevice);
            }
            else
            {
                throw new Exception(string.Format("The device type {0} does not support ViewKmfRsi", device.DeviceType.ToString()));
            }
        }

        public static List<RspKeysetInfo> ViewKeysetTaggingInfo(BaseDevice device)
        {
            if (device.DeviceType == BaseDevice.DeviceTypeOptions.TwiKfdtool)
            {
                return InteractTwiKfdtool.ViewKeysetTaggingInfo(device.TwiKfdtoolDevice);
            }
            else if (device.DeviceType == BaseDevice.DeviceTypeOptions.DliIp)
            {
                return InteractDliIp.ViewKeysetTaggingInfo(device.DliIpDevice);
            }
            else
            {
                throw new Exception(string.Format("The device type {0} does not support ViewKeysetTaggingInfo", device.DeviceType.ToString()));
            }
        }

        public static RspChangeoverInfo ActivateKeyset(BaseDevice device, int keysetSuperseded, int keysetActivated)
        {
            if (device.DeviceType == BaseDevice.DeviceTypeOptions.TwiKfdtool)
            {
                return InteractTwiKfdtool.ActivateKeyset(device.TwiKfdtoolDevice, keysetSuperseded, keysetActivated);
            }
            else if (device.DeviceType == BaseDevice.DeviceTypeOptions.DliIp)
            {
                return InteractDliIp.ActivateKeyset(device.DliIpDevice, keysetSuperseded, keysetActivated);
            }
            else
            {
                throw new Exception(string.Format("The device type {0} does not support ActivateKeyset", device.DeviceType.ToString()));
            }
        }

        public static void LoadAuthenticationKey(BaseDevice device, bool targetSpecificSuId, int wacnId, int systemId, int unitId, byte[] key)
        {
            if (device.DeviceType == BaseDevice.DeviceTypeOptions.TwiKfdtool)
            {
                InteractTwiKfdtool.LoadAuthenticationKey(device.TwiKfdtoolDevice, targetSpecificSuId, wacnId, systemId, unitId, key);
            }
            else if (device.DeviceType == BaseDevice.DeviceTypeOptions.DliIp)
            {
                InteractDliIp.LoadAuthenticationKey(device.DliIpDevice, targetSpecificSuId, wacnId, systemId, unitId, key);
            }
            else
            {
                throw new Exception(string.Format("The device type {0} does not support LoadAuthenticationKey", device.DeviceType.ToString()));
            }
        }

        public static void DeleteAuthenticationKey(BaseDevice device, bool targetSpecificSuId, bool deleteAllKeys, int wacnId, int systemId, int unitId)
        {
            if (device.DeviceType == BaseDevice.DeviceTypeOptions.TwiKfdtool)
            {
                InteractTwiKfdtool.DeleteAuthenticationKey(device.TwiKfdtoolDevice, targetSpecificSuId, deleteAllKeys, wacnId, systemId, unitId);
            }
            else if (device.DeviceType == BaseDevice.DeviceTypeOptions.DliIp)
            {
                InteractDliIp.DeleteAuthenticationKey(device.DliIpDevice, targetSpecificSuId, deleteAllKeys, wacnId, systemId, unitId);
            }
            else
            {
                throw new Exception(string.Format("The device type {0} does not support LoadAuthenticationKey", device.DeviceType.ToString()));
            }
        }
    }
}
