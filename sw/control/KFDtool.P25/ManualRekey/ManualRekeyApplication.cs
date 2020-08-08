using KFDtool.Adapter.Protocol.Adapter;
using KFDtool.P25.DataLinkIndependent;
using KFDtool.P25.DeviceProtocol;
using KFDtool.P25.Kmm;
using KFDtool.P25.NetworkProtocol;
using KFDtool.P25.Partition;
using KFDtool.P25.ThreeWire;
using KFDtool.P25.TransferConstructs;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KFDtool.P25.ManualRekey
{
    public class ManualRekeyApplication
    {
        private static Logger Log = LogManager.GetCurrentClassLogger();

        private bool WithPreamble { get; set; }

        private byte Mfid { get; set; }

        private IDeviceProtocol DeviceProtocol;

        public ManualRekeyApplication(AdapterProtocol adapterProtocol)
        {
            DeviceProtocol = new ThreeWireProtocol(adapterProtocol);
            WithPreamble = false;
            Mfid = 0x00;
        }

        public ManualRekeyApplication(UdpProtocol udpProtocol, SessionControlOptions sessionControlType)
        {
            DeviceProtocol = new DataLinkIndependentProtocol(udpProtocol, sessionControlType);

            WithPreamble = true;

            if (sessionControlType == SessionControlOptions.Motorola)
            {
                Mfid = 0x90;
            }
            else
            {
                Mfid = 0x00;
            }
        }

        private void Begin()
        {
            DeviceProtocol.SendKeySignature();

            DeviceProtocol.InitSession();
        }

        private KmmBody TxRxKmm(KmmBody commandKmmBody)
        {
            KmmFrame commandKmmFrame = new KmmFrame(commandKmmBody);

            byte[] toRadio = WithPreamble ? commandKmmFrame.ToBytesWithPreamble(Mfid) : commandKmmFrame.ToBytes();

            byte[] fromRadio = DeviceProtocol.PerformKmmTransfer(toRadio);

            KmmFrame responseKmmFrame = new KmmFrame(WithPreamble, fromRadio);

            return responseKmmFrame.KmmBody;
        }

        private void End()
        {
            DeviceProtocol.EndSession();
        }

        public void CheckTargetMrConnection()
        {
            DeviceProtocol.CheckTargetMrConnection();
        }

        public void Keyload(List<CmdKeyItem> keyItems)
        {
            List<List<CmdKeyItem>> keyGroups = KeyPartitioner.PartitionKeys(keyItems);

            Begin();

            try
            {
                InventoryCommandListActiveKsetIds cmdKmmBody1 = new InventoryCommandListActiveKsetIds();

                KmmBody rspKmmBody1 = TxRxKmm(cmdKmmBody1);

                int activeKeysetId = 0;

                if (rspKmmBody1 is InventoryResponseListActiveKsetIds)
                {
                    InventoryResponseListActiveKsetIds kmm = rspKmmBody1 as InventoryResponseListActiveKsetIds;

                    Log.Debug("number of active keyset ids: {0}", kmm.KsetIds.Count);

                    for (int i = 0; i < kmm.KsetIds.Count; i++)
                    {
                        Log.Debug("* keyset id index {0} *", i);
                        Log.Debug("keyset id: {0} (dec), {0:X} (hex)", kmm.KsetIds[i]);
                    }

                    // TODO support more than one crypto group
                    if (kmm.KsetIds.Count > 0)
                    {
                        activeKeysetId = kmm.KsetIds[0];
                    }
                    else
                    {
                        activeKeysetId = 1; // to match KVL3000+ R3.53.03 behavior
                    }
                }
                else if (rspKmmBody1 is NegativeAcknowledgment)
                {
                    NegativeAcknowledgment kmm = rspKmmBody1 as NegativeAcknowledgment;

                    string statusDescr = OperationStatusExtensions.ToStatusString(kmm.Status);
                    string statusReason = OperationStatusExtensions.ToReasonString(kmm.Status);
                    throw new Exception(string.Format("received negative acknowledgment{0}status: {1} (0x{2:X2}){0}{3}", Environment.NewLine, statusDescr, (byte)kmm.Status, statusReason));
                }
                else
                {
                    throw new Exception("unexpected kmm");
                }

                foreach (List<CmdKeyItem> keyGroup in keyGroups)
                {
                    ModifyKeyCommand modifyKeyCommand = new ModifyKeyCommand();

                    // TODO support more than one crypto group
                    if (keyGroup[0].UseActiveKeyset && !keyGroup[0].IsKek)
                    {
                        modifyKeyCommand.KeysetId = activeKeysetId;
                    }
                    else if (keyGroup[0].UseActiveKeyset && keyGroup[0].IsKek)
                    {
                        modifyKeyCommand.KeysetId = 0xFF; // to match KVL3000+ R3.53.03 behavior
                    }
                    else
                    {
                        modifyKeyCommand.KeysetId = keyGroup[0].KeysetId;
                    }

                    modifyKeyCommand.AlgorithmId = keyGroup[0].AlgorithmId;

                    foreach (CmdKeyItem key in keyGroup)
                    {
                        Log.Debug(key.ToString());

                        KeyItem keyItem = new KeyItem();
                        keyItem.SLN = key.Sln;
                        keyItem.KeyId = key.KeyId;
                        keyItem.Key = key.Key.ToArray();
                        keyItem.KEK = key.IsKek;
                        keyItem.Erase = false;

                        modifyKeyCommand.KeyItems.Add(keyItem);
                    }

                    KmmBody rspKmmBody2 = TxRxKmm(modifyKeyCommand);

                    if (rspKmmBody2 is RekeyAcknowledgment)
                    {
                        RekeyAcknowledgment kmm = rspKmmBody2 as RekeyAcknowledgment;

                        Log.Debug("number of key status: {0}", kmm.Keys.Count);

                        for (int i = 0; i < kmm.Keys.Count; i++)
                        {
                            KeyStatus status = kmm.Keys[i];

                            Log.Debug("* key status index {0} *", i);
                            Log.Debug("algorithm id: {0} (dec), {0:X} (hex)", status.AlgorithmId);
                            Log.Debug("key id: {0} (dec), {0:X} (hex)", status.KeyId);
                            Log.Debug("status: {0} (dec), {0:X} (hex)", status.Status);

                            if (status.Status != 0)
                            {
                                string statusDescr = OperationStatusExtensions.ToStatusString((OperationStatus)status.Status);
                                string statusReason = OperationStatusExtensions.ToReasonString((OperationStatus)status.Status);
                                throw new Exception(
                                    string.Format(
                                        "received unexpected key status{0}" +
                                        "algorithm id: {1} (0x{1:X}){0}" +
                                        "key id: {2} (0x{2:X}){0}" +
                                        "status: {3} (0x{3:X}){0}" +
                                        "status description: {4}{0}" +
                                        "status reason: {5}",
                                        Environment.NewLine, status.AlgorithmId, status.KeyId, status.Status, statusDescr, statusReason
                                    )
                                );
                            }
                        }
                    }
                    else if (rspKmmBody2 is NegativeAcknowledgment)
                    {
                        NegativeAcknowledgment kmm = rspKmmBody2 as NegativeAcknowledgment;

                        string statusDescr = OperationStatusExtensions.ToStatusString(kmm.Status);
                        string statusReason = OperationStatusExtensions.ToReasonString(kmm.Status);
                        throw new Exception(string.Format("received negative acknowledgment{0}status: {1} (0x{2:X2}){0}{3}", Environment.NewLine, statusDescr, (byte)kmm.Status, statusReason));
                    }
                    else
                    {
                        throw new Exception("received unexpected kmm");
                    }
                }
            }
            catch
            {
                End();

                throw;
            }

            End();
        }

        public void EraseKeys(List<CmdKeyItem> keyItems)
        {
            List<List<CmdKeyItem>> keyGroups = KeyPartitioner.PartitionKeys(keyItems);

            Begin();

            try
            {
                InventoryCommandListActiveKsetIds cmdKmmBody1 = new InventoryCommandListActiveKsetIds();

                KmmBody rspKmmBody1 = TxRxKmm(cmdKmmBody1);

                int activeKeysetId = 0;

                if (rspKmmBody1 is InventoryResponseListActiveKsetIds)
                {
                    InventoryResponseListActiveKsetIds kmm = rspKmmBody1 as InventoryResponseListActiveKsetIds;

                    Log.Debug("number of active keyset ids: {0}", kmm.KsetIds.Count);

                    for (int i = 0; i < kmm.KsetIds.Count; i++)
                    {
                        Log.Debug("* keyset id index {0} *", i);
                        Log.Debug("keyset id: {0} (dec), {0:X} (hex)", kmm.KsetIds[i]);
                    }

                    // TODO support more than one crypto group
                    if (kmm.KsetIds.Count > 0)
                    {
                        activeKeysetId = kmm.KsetIds[0];
                    }
                    else
                    {
                        activeKeysetId = 1; // to match KVL3000+ R3.53.03 behavior
                    }
                }
                else if (rspKmmBody1 is NegativeAcknowledgment)
                {
                    NegativeAcknowledgment kmm = rspKmmBody1 as NegativeAcknowledgment;

                    string statusDescr = OperationStatusExtensions.ToStatusString(kmm.Status);
                    string statusReason = OperationStatusExtensions.ToReasonString(kmm.Status);
                    throw new Exception(string.Format("received negative acknowledgment{0}status: {1} (0x{2:X2}){0}{3}", Environment.NewLine, statusDescr, (byte)kmm.Status, statusReason));
                }
                else
                {
                    throw new Exception("unexpected kmm");
                }

                foreach (List<CmdKeyItem> keyGroup in keyGroups)
                {
                    ModifyKeyCommand modifyKeyCommand = new ModifyKeyCommand();

                    // TODO support more than one crypto group
                    if (keyGroup[0].UseActiveKeyset && !keyGroup[0].IsKek)
                    {
                        modifyKeyCommand.KeysetId = activeKeysetId;
                    }
                    else if (keyGroup[0].UseActiveKeyset && keyGroup[0].IsKek)
                    {
                        modifyKeyCommand.KeysetId = 0xFF; // to match KVL3000+ R3.53.03 behavior
                    }
                    else
                    {
                        modifyKeyCommand.KeysetId = keyGroup[0].KeysetId;
                    }

                    modifyKeyCommand.AlgorithmId = 0x81; // to match KVL3000+ R3.53.03 behavior

                    foreach (CmdKeyItem key in keyGroup)
                    {
                        Log.Debug(key.ToString());

                        KeyItem keyItem = new KeyItem();
                        keyItem.SLN = key.Sln;
                        keyItem.KeyId = 65535; // to match KVL3000+ R3.53.03 behavior
                        keyItem.Key = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }; // to match KVL3000+ R3.53.03 behavior
                        keyItem.KEK = key.IsKek;
                        keyItem.Erase = true;

                        modifyKeyCommand.KeyItems.Add(keyItem);
                    }

                    KmmBody rspKmmBody2 = TxRxKmm(modifyKeyCommand);

                    if (rspKmmBody2 is RekeyAcknowledgment)
                    {
                        RekeyAcknowledgment kmm = rspKmmBody2 as RekeyAcknowledgment;

                        Log.Debug("number of key status: {0}", kmm.Keys.Count);

                        for (int i = 0; i < kmm.Keys.Count; i++)
                        {
                            KeyStatus status = kmm.Keys[i];

                            Log.Debug("* key status index {0} *", i);
                            Log.Debug("algorithm id: {0} (dec), {0:X} (hex)", status.AlgorithmId);
                            Log.Debug("key id: {0} (dec), {0:X} (hex)", status.KeyId);
                            Log.Debug("status: {0} (dec), {0:X} (hex)", status.Status);

                            if (status.Status != 0)
                            {
                                string statusDescr = OperationStatusExtensions.ToStatusString((OperationStatus)status.Status);
                                string statusReason = OperationStatusExtensions.ToReasonString((OperationStatus)status.Status);
                                throw new Exception(
                                    string.Format(
                                        "received unexpected key status{0}" +
                                        "algorithm id: {1} (0x{1:X}){0}" +
                                        "key id: {2} (0x{2:X}){0}" +
                                        "status: {3} (0x{3:X}){0}" +
                                        "status description: {4}{0}" +
                                        "status reason: {5}",
                                        Environment.NewLine, status.AlgorithmId, status.KeyId, status.Status, statusDescr, statusReason
                                    )
                                );
                            }
                        }
                    }
                    else if (rspKmmBody2 is NegativeAcknowledgment)
                    {
                        NegativeAcknowledgment kmm = rspKmmBody2 as NegativeAcknowledgment;

                        string statusDescr = OperationStatusExtensions.ToStatusString(kmm.Status);
                        string statusReason = OperationStatusExtensions.ToReasonString(kmm.Status);
                        throw new Exception(string.Format("received negative acknowledgment{0}status: {1} (0x{2:X2}){0}{3}", Environment.NewLine, statusDescr, (byte)kmm.Status, statusReason));
                    }
                    else
                    {
                        throw new Exception("received unexpected kmm");
                    }
                }
            }
            catch
            {
                End();

                throw;
            }

            End();
        }

        public void EraseAllKeys()
        {
            Begin();

            try
            {
                ZeroizeCommand commandKmmBody = new ZeroizeCommand();

                KmmBody responseKmmBody = TxRxKmm(commandKmmBody);

                if (responseKmmBody is ZeroizeResponse)
                {
                    Log.Debug("zerozied");
                }
                else if (responseKmmBody is NegativeAcknowledgment)
                {
                    NegativeAcknowledgment kmm = responseKmmBody as NegativeAcknowledgment;

                    string statusDescr = OperationStatusExtensions.ToStatusString(kmm.Status);
                    string statusReason = OperationStatusExtensions.ToReasonString(kmm.Status);
                    throw new Exception(string.Format("received negative acknowledgment{0}status: {1} (0x{2:X2}){0}{3}", Environment.NewLine, statusDescr, (byte)kmm.Status, statusReason));
                }
                else
                {
                    throw new Exception("unexpected kmm");
                }
            }
            catch
            {
                End();

                throw;
            }

            End();
        }

        public List<RspKeyInfo> ViewKeyInfo()
        {
            List<RspKeyInfo> result = new List<RspKeyInfo>();

            Begin();

            try
            {
                bool more = true;
                int marker = 0;

                while (more)
                {
                    InventoryCommandListActiveKeys commandKmmBody = new InventoryCommandListActiveKeys();
                    commandKmmBody.InventoryMarker = marker;
                    commandKmmBody.MaxKeysRequested = 78;

                    KmmBody responseKmmBody = TxRxKmm(commandKmmBody);

                    if (responseKmmBody is InventoryResponseListActiveKeys)
                    {
                        InventoryResponseListActiveKeys kmm = responseKmmBody as InventoryResponseListActiveKeys;

                        marker = kmm.InventoryMarker;

                        Log.Debug("inventory marker: {0}", marker);

                        if (marker == 0)
                        {
                            more = false;
                        }

                        Log.Debug("number of keys returned: {0}", kmm.Keys.Count);

                        for (int i = 0; i < kmm.Keys.Count; i++)
                        {
                            KeyInfo info = kmm.Keys[i];

                            Log.Debug("* key index {0} *", i);
                            Log.Debug("keyset id: {0} (dec), {0:X} (hex)", info.KeySetId);
                            Log.Debug("sln: {0} (dec), {0:X} (hex)", info.SLN);
                            Log.Debug("algorithm id: {0} (dec), {0:X} (hex)", info.AlgorithmId);
                            Log.Debug("key id: {0} (dec), {0:X} (hex)", info.KeyId);

                            RspKeyInfo res = new RspKeyInfo();

                            res.KeysetId = info.KeySetId;
                            res.Sln = info.SLN;
                            res.AlgorithmId = info.AlgorithmId;
                            res.KeyId = info.KeyId;

                            result.Add(res);
                        }
                    }
                    else if (responseKmmBody is NegativeAcknowledgment)
                    {
                        NegativeAcknowledgment kmm = responseKmmBody as NegativeAcknowledgment;

                        string statusDescr = OperationStatusExtensions.ToStatusString(kmm.Status);
                        string statusReason = OperationStatusExtensions.ToReasonString(kmm.Status);
                        throw new Exception(string.Format("received negative acknowledgment{0}status: {1} (0x{2:X2}){0}{3}", Environment.NewLine, statusDescr, (byte)kmm.Status, statusReason));
                    }
                    else
                    {
                        throw new Exception("unexpected kmm");
                    }
                }
            }
            catch
            {
                End();

                throw;
            }

            End();

            return result;
        }

        public int ViewKmfRsi()
        {
            int result = new int();

            Begin();

            try
            {
                InventoryCommandListKmfRsi commandKmmBody = new InventoryCommandListKmfRsi();

                KmmBody responseKmmBody = TxRxKmm(commandKmmBody);

                if (responseKmmBody is InventoryResponseListKmfRsi)
                {
                    Log.Debug("MNP response");

                    InventoryResponseListKmfRsi kmm = responseKmmBody as InventoryResponseListKmfRsi;

                    result = kmm.KmfRsi;
                }
                else if (responseKmmBody is NegativeAcknowledgment)
                {
                    NegativeAcknowledgment kmm = responseKmmBody as NegativeAcknowledgment;

                    string statusDescr = OperationStatusExtensions.ToStatusString(kmm.Status);
                    string statusReason = OperationStatusExtensions.ToReasonString(kmm.Status);
                    throw new Exception(string.Format("received negative acknowledgment{0}status: {1} (0x{2:X2}){0}{3}", Environment.NewLine, statusDescr, (byte)kmm.Status, statusReason));
                }
                else
                {
                    throw new Exception("unexpected kmm");
                }
            }
            catch
            {
                End();

                throw;
            }

            End();

            return result;
        }

        public int ViewMnp()
        {
            int result = new int();

            Begin();

            try
            {
                InventoryCommandListMnp commandKmmBody = new InventoryCommandListMnp();

                KmmBody responseKmmBody = TxRxKmm(commandKmmBody);

                if (responseKmmBody is InventoryResponseListMnp)
                {
                    Log.Debug("MNP response");

                    InventoryResponseListMnp kmm = responseKmmBody as InventoryResponseListMnp;

                    result = kmm.MessageNumberPeriod;
                }
                else if (responseKmmBody is NegativeAcknowledgment)
                {
                    NegativeAcknowledgment kmm = responseKmmBody as NegativeAcknowledgment;

                    string statusDescr = OperationStatusExtensions.ToStatusString(kmm.Status);
                    string statusReason = OperationStatusExtensions.ToReasonString(kmm.Status);
                    throw new Exception(string.Format("received negative acknowledgment{0}status: {1} (0x{2:X2}){0}{3}", Environment.NewLine, statusDescr, (byte)kmm.Status, statusReason));
                }
                else
                {
                    throw new Exception("unexpected kmm");
                }
            }
            catch
            {
                End();

                throw;
            }

            End();

            return result;
        }

        public RspRsiInfo LoadConfig(int kmfRsi, int mnp)
        {
            RspRsiInfo result = new RspRsiInfo();

            Begin();

            try
            {
                LoadConfigCommand cmdKmmBody = new LoadConfigCommand();
                cmdKmmBody.KmfRsi = kmfRsi;
                cmdKmmBody.MessageNumberPeriod = mnp;
                KmmBody rspKmmBody = TxRxKmm(cmdKmmBody);
                if (rspKmmBody is LoadConfigResponse)
                {
                    LoadConfigResponse kmm = rspKmmBody as LoadConfigResponse;
                    result.RSI = kmm.RSI;
                    result.MN = kmm.MN;
                    result.Status = kmm.Status;
                }
                else if (rspKmmBody is NegativeAcknowledgment)
                {
                    NegativeAcknowledgment kmm = rspKmmBody as NegativeAcknowledgment;

                    string statusDescr = OperationStatusExtensions.ToStatusString(kmm.Status);
                    string statusReason = OperationStatusExtensions.ToReasonString(kmm.Status);
                    throw new Exception(string.Format("received negative acknowledgment{0}status: {1} (0x{2:X2}){0}{3}", Environment.NewLine, statusDescr, (byte)kmm.Status, statusReason));
                }
                else
                {
                    throw new Exception("unexpected kmm");
                }
            }
            catch
            {
                End();

                throw;
            }

            End();

            return result;
        }

        public RspRsiInfo ChangeRsi(int rsiOld, int rsiNew, int mnp)
        {
            RspRsiInfo result = new RspRsiInfo();

            Begin();

            try
            {
                ChangeRsiCommand cmdKmmBody = new ChangeRsiCommand();
                cmdKmmBody.RsiOld = rsiOld;
                cmdKmmBody.RsiNew = rsiNew;
                cmdKmmBody.MessageNumber = mnp;

                KmmBody rspKmmBody = TxRxKmm(cmdKmmBody);

                if (rspKmmBody is ChangeRsiResponse)
                {
                    ChangeRsiResponse kmm = rspKmmBody as ChangeRsiResponse;
                    result.RSI = rsiNew;
                    result.MN = mnp;
                    result.Status = kmm.Status;
                }
                else if (rspKmmBody is NegativeAcknowledgment)
                {
                    NegativeAcknowledgment kmm = rspKmmBody as NegativeAcknowledgment;

                    string statusDescr = OperationStatusExtensions.ToStatusString(kmm.Status);
                    string statusReason = OperationStatusExtensions.ToReasonString(kmm.Status);
                    throw new Exception(string.Format("received negative acknowledgment{0}status: {1} (0x{2:X2}){0}{3}", Environment.NewLine, statusDescr, (byte)kmm.Status, statusReason));
                }
                else
                {
                    throw new Exception("unexpected kmm");
                }
            }
            catch
            {
                End();

                throw;
            }

            End();

            return result;
        }

        public List<RspRsiInfo> ViewRsiItems()
        {
            List<RspRsiInfo> result = new List<RspRsiInfo>();

            Begin();

            try
            {
                bool more = true;
                int marker = 0;

                while (more)
                {
                    InventoryCommandListRsiItems commandKmmBody = new InventoryCommandListRsiItems();

                    KmmBody responseKmmBody = TxRxKmm(commandKmmBody);

                    if (responseKmmBody is InventoryResponseListRsiItems)
                    {
                        InventoryResponseListRsiItems kmm = responseKmmBody as InventoryResponseListRsiItems;

                        Log.Debug("inventory marker: {0}", marker);

                        if (marker == 0)
                        {
                            more = false;
                        }

                        Log.Debug("number of RSIs returned: {0}", kmm.RsiItems.Count);

                        for (int i = 0; i < kmm.RsiItems.Count; i++)
                        {
                            RsiItem item = kmm.RsiItems[i];

                            Log.Debug("* rsi index {0} *", i);
                            Log.Debug("rsi id: {0} (dec), {0:X} (hex)", item.RSI);
                            Log.Debug("mn: {0} (dec), {0:X} (hex)", item.MessageNumber);

                            RspRsiInfo res = new RspRsiInfo();

                            res.RSI = (int)item.RSI;
                            res.MN = item.MessageNumber;

                            result.Add(res);
                        }
                    }
                    else if (responseKmmBody is NegativeAcknowledgment)
                    {
                        NegativeAcknowledgment kmm = responseKmmBody as NegativeAcknowledgment;

                        string statusDescr = OperationStatusExtensions.ToStatusString(kmm.Status);
                        string statusReason = OperationStatusExtensions.ToReasonString(kmm.Status);
                        throw new Exception(string.Format("received negative acknowledgment{0}status: {1} (0x{2:X2}){0}{3}", Environment.NewLine, statusDescr, (byte)kmm.Status, statusReason));
                    }
                    else
                    {
                        throw new Exception("unexpected kmm");
                    }
                }
            }
            catch
            {
                End();

                throw;
            }

            End();

            return result;
        }

        public List<RspKeysetInfo> ViewKeysetTaggingInfo()
        {
            List<RspKeysetInfo> result = new List<RspKeysetInfo>();

            Begin();

            try
            {
                InventoryCommandListKeysetTaggingInfo commandKmmBody = new InventoryCommandListKeysetTaggingInfo();

                KmmBody responseKmmBody = TxRxKmm(commandKmmBody);

                if (responseKmmBody is InventoryResponseListKeysetTaggingInfo)
                {
                    Log.Debug("KeysetTaggingInfo response");

                    InventoryResponseListKeysetTaggingInfo kmm = responseKmmBody as InventoryResponseListKeysetTaggingInfo;

                    for (int i = 0; i < kmm.KeysetItems.Count; i++)
                    {
                        KeysetItem item = kmm.KeysetItems[i];

                        RspKeysetInfo res = new RspKeysetInfo();

                        res.KeysetId = item.KeysetId;
                        res.KeysetName = item.KeysetName;
                        res.KeysetType = item.KeysetType;
                        res.ActivationDateTime = item.ActivationDateTime;
                        res.ReservedField = item.ReservedField;

                        result.Add(res);
                    }
                }
                else if (responseKmmBody is NegativeAcknowledgment)
                {
                    NegativeAcknowledgment kmm = responseKmmBody as NegativeAcknowledgment;

                    string statusDescr = OperationStatusExtensions.ToStatusString(kmm.Status);
                    string statusReason = OperationStatusExtensions.ToReasonString(kmm.Status);
                    throw new Exception(string.Format("received negative acknowledgment{0}status: {1} (0x{2:X2}){0}{3}", Environment.NewLine, statusDescr, (byte)kmm.Status, statusReason));
                }
                else
                {
                    throw new Exception("unexpected kmm");
                }
            }
            catch
            {
                End();

                throw;
            }

            End();

            return result;
        }

        public RspChangeoverInfo ActivateKeyset(int keysetSuperseded, int keysetActivated)
        {
            RspChangeoverInfo result = new RspChangeoverInfo();

            Begin();

            try
            {
                ChangeoverCommand cmdKmmBody = new ChangeoverCommand();
                cmdKmmBody.KeysetIdSuperseded = keysetSuperseded;
                cmdKmmBody.KeysetIdActivated = keysetActivated;
                KmmBody rspKmmBody = TxRxKmm(cmdKmmBody);

                if (rspKmmBody is ChangeoverResponse)
                {
                    ChangeoverResponse kmm = rspKmmBody as ChangeoverResponse;

                    result.KeysetIdSuperseded = kmm.KeysetIdSuperseded;
                    result.KeysetIdActivated = kmm.KeysetIdActivated;
                }
                else if (rspKmmBody is NegativeAcknowledgment)
                {
                    NegativeAcknowledgment kmm = rspKmmBody as NegativeAcknowledgment;

                    string statusDescr = OperationStatusExtensions.ToStatusString(kmm.Status);
                    string statusReason = OperationStatusExtensions.ToReasonString(kmm.Status);
                    throw new Exception(string.Format("received negative acknowledgment{0}status: {1} (0x{2:X2}){0}{3}", Environment.NewLine, statusDescr, (byte)kmm.Status, statusReason));
                }
                else
                {
                    throw new Exception("unexpected kmm");
                }
            }
            catch
            {
                End();

                throw;
            }

            End();

            return result;
        }

        public void LoadAuthenticationKey(bool targetSpecificSuId, int wacnId, int systemId, int unitId, byte[] key)
        {
            Begin();

            try
            {
                SuId commandSuId = new SuId(wacnId, systemId, unitId);

                LoadAuthenticationKeyCommand cmdKmmBody = new LoadAuthenticationKeyCommand(targetSpecificSuId, commandSuId, key);

                KmmBody rspKmmBody = TxRxKmm(cmdKmmBody);

                if (rspKmmBody is LoadAuthenticationKeyResponse)
                {
                    LoadAuthenticationKeyResponse kmm = rspKmmBody as LoadAuthenticationKeyResponse;

                    if (kmm.AssignmentSuccess != true || kmm.Status != OperationStatus.CommandWasPerformed)
                    {
                        throw new Exception(string.Format("abnormal response - assignment success: {0}, status: {1} (0x{2:X2})", kmm.AssignmentSuccess, kmm.Status.ToString(), (byte)kmm.Status));
                    }
                }
                else if (rspKmmBody is NegativeAcknowledgment)
                {
                    NegativeAcknowledgment kmm = rspKmmBody as NegativeAcknowledgment;

                    string statusDescr = OperationStatusExtensions.ToStatusString(kmm.Status);
                    string statusReason = OperationStatusExtensions.ToReasonString(kmm.Status);
                    throw new Exception(string.Format("received negative acknowledgment{0}status: {1} (0x{2:X2}){0}{3}", Environment.NewLine, statusDescr, (byte)kmm.Status, statusReason));
                }
                else
                {
                    throw new Exception("unexpected kmm");
                }
            }
            catch
            {
                End();

                throw;
            }

            End();
        }

        public void DeleteAuthenticationKey(bool targetSpecificSuId, bool deleteAllKeys, int wacnId, int systemId, int unitId)
        {
            Begin();

            try
            {
                SuId commandSuId = new SuId(wacnId, systemId, unitId);

                DeleteAuthenticationKeyCommand cmdKmmBody = new DeleteAuthenticationKeyCommand(targetSpecificSuId, deleteAllKeys, commandSuId);

                KmmBody rspKmmBody = TxRxKmm(cmdKmmBody);

                if (rspKmmBody is DeleteAuthenticationKeyResponse)
                {
                    DeleteAuthenticationKeyResponse kmm = rspKmmBody as DeleteAuthenticationKeyResponse;

                    if (kmm.Status != OperationStatus.CommandWasPerformed)
                    {
                        throw new Exception(string.Format("abnormal response - status: {0} (0x{1:X2})", kmm.Status.ToString(), (byte)kmm.Status));
                    }
                }
                else if (rspKmmBody is NegativeAcknowledgment)
                {
                    NegativeAcknowledgment kmm = rspKmmBody as NegativeAcknowledgment;

                    string statusDescr = OperationStatusExtensions.ToStatusString(kmm.Status);
                    string statusReason = OperationStatusExtensions.ToReasonString(kmm.Status);
                    throw new Exception(string.Format("received negative acknowledgment{0}status: {1} (0x{2:X2}){0}{3}", Environment.NewLine, statusDescr, (byte)kmm.Status, statusReason));
                }
                else
                {
                    throw new Exception("unexpected kmm");
                }
            }
            catch
            {
                End();

                throw;
            }

            End();
        }

        public void ViewSuidInfo()
        {
            Begin();

            try
            {
                InventoryCommandListActiveSuId cmdKmmBody = new InventoryCommandListActiveSuId();

                KmmBody rspKmmBody = TxRxKmm(cmdKmmBody);

                if (rspKmmBody is InventoryResponseListActiveSuId)
                {
                    InventoryResponseListActiveSuId kmm = rspKmmBody as InventoryResponseListActiveSuId;

                    if (kmm.Status != OperationStatus.CommandWasPerformed)
                    {
                        throw new Exception(string.Format("abnormal response - status: {0} (0x{1:X2})", kmm.Status.ToString(), (byte)kmm.Status));
                    }

                    Log.Fatal("WACN: 0x{0:X}, System: 0x{1:X}, Unit: 0x{2:X}, Key Assigned: {3}, Is Active: {4}", kmm.SuId.WacnId, kmm.SuId.SystemId, kmm.SuId.UnitId, kmm.KeyAssigned, kmm.ActiveSuId); // TODO remove
                }
                else if (rspKmmBody is NegativeAcknowledgment)
                {
                    NegativeAcknowledgment kmm = rspKmmBody as NegativeAcknowledgment;

                    string statusDescr = OperationStatusExtensions.ToStatusString(kmm.Status);
                    string statusReason = OperationStatusExtensions.ToReasonString(kmm.Status);
                    throw new Exception(string.Format("received negative acknowledgment{0}status: {1} (0x{2:X2}){0}{3}", Environment.NewLine, statusDescr, (byte)kmm.Status, statusReason));
                }
                else
                {
                    throw new Exception("unexpected kmm");
                }
            }
            catch
            {
                End();

                throw;
            }

            End();
        }

        public void ViewActiveSuidInfo()
        {
            throw new NotImplementedException();
        }
    }
}
