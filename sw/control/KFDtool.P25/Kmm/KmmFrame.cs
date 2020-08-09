using KFDtool.Shared;
using System;
using System.Collections;
using System.Collections.Generic;

namespace KFDtool.P25.Kmm
{
    public class KmmFrame
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public KmmBody KmmBody { get; private set; }

        // TODO src rsi

        // TODO dest rsi

        public KmmFrame(KmmBody kmmBody)
        {
            if (kmmBody == null)
            {
                throw new ArgumentNullException("kmmBody");
            }

            KmmBody = kmmBody;
        }

        public KmmFrame(bool hasPreamble, byte[] contents)
        {
            if (hasPreamble)
            {
                ParseWithPreamble(contents);
            }
            else
            {
                Parse(contents);
            }
        }

        public byte[] ToBytes()
        {
            byte[] body = KmmBody.ToBytes();

            int length = 10 + body.Length;

            byte[] frame = new byte[length];

            /* message id */
            frame[0] = (byte)KmmBody.MessageId;

            /* message length */
            int messageLength = 7 + body.Length;
            frame[1] = (byte)((messageLength >> 8) & 0xFF);
            frame[2] = (byte)(messageLength & 0xFF);

            /* message format */
            BitArray messageFormat = new BitArray(8, false);
            messageFormat.Set(7, Convert.ToBoolean(((byte)KmmBody.ResponseKind & 0x02) >> 1));
            messageFormat.Set(6, Convert.ToBoolean((byte)KmmBody.ResponseKind & 0x01));
            messageFormat.CopyTo(frame, 3);

            /* destination rsi */
            frame[4] = 0xFF;
            frame[5] = 0xFF;
            frame[6] = 0xFF;

            /* source rsi */
            frame[7] = 0xFF;
            frame[8] = 0xFF;
            frame[9] = 0xFF;

            /* message body */
            Array.Copy(body, 0, frame, 10, body.Length);

            return frame;
        }

        public byte[] ToBytesWithPreamble(byte mfid)
        {
            // TODO add encryption, currently hardcoded to clear

            List<byte> data = new List<byte>();

            data.Add(0x00); // version

            data.Add(mfid); // mfid

            data.Add(0x80); // algid

            data.Add(0x00); // key id
            data.Add(0x00); // key id

            data.Add(0x00); // mi
            data.Add(0x00); // mi
            data.Add(0x00); // mi
            data.Add(0x00); // mi
            data.Add(0x00); // mi
            data.Add(0x00); // mi
            data.Add(0x00); // mi
            data.Add(0x00); // mi
            data.Add(0x00); // mi

            data.AddRange(ToBytes());

            return data.ToArray();
        }

        private void Parse(byte[] contents)
        {
            if (contents.Length < 10)
            {
                throw new ArgumentOutOfRangeException(string.Format("length mismatch - expected at least 10, got {0} - {1}", contents.Length.ToString(), BitConverter.ToString(contents)));
            }

            MessageId messageId = (MessageId)contents[0];

            Logger.Debug("rx standard mfid kmm frame, message id: {0} (0x{1:X2}), contents: {2}", messageId.ToString(), (byte)messageId, Utility.DataFormat(contents));

            int messageLength = 0;
            messageLength |= contents[1] << 8;
            messageLength |= contents[2];

            int messageBodyLength = messageLength - 7;
            byte[] messageBody = new byte[messageBodyLength];
            Array.Copy(contents, 10, messageBody, 0, messageBodyLength);

            if (messageId == MessageId.ChangeRsiResponse)
            {
                KmmBody kmmBody = new ChangeRsiResponse();
                kmmBody.Parse(messageBody);
                KmmBody = kmmBody;
            }
            else if (messageId == MessageId.ChangeoverResponse)
            {
                KmmBody kmmBody = new ChangeoverResponse();
                kmmBody.Parse(messageBody);
                KmmBody = kmmBody;
            }
            else if (messageId == MessageId.InventoryCommand)
            {
                if (messageBody.Length > 0)
                {
                    InventoryType inventoryType = (InventoryType)messageBody[0];

                    if (inventoryType == InventoryType.ListActiveKsetIds)
                    {
                        KmmBody kmmBody = new InventoryCommandListActiveKsetIds();
                        kmmBody.Parse(messageBody);
                        KmmBody = kmmBody;
                    }
                    else if (inventoryType == InventoryType.ListRsiItems)
                    {
                        KmmBody kmmBody = new InventoryCommandListRsiItems();
                        kmmBody.Parse(messageBody);
                        KmmBody = kmmBody;
                    }
                    else if (inventoryType == InventoryType.ListActiveKeys)
                    {
                        KmmBody kmmBody = new InventoryCommandListActiveKeys();
                        kmmBody.Parse(messageBody);
                        KmmBody = kmmBody;
                    }
                    else
                    {
                        throw new Exception(string.Format("unknown inventory command type: 0x{0:X2}", (byte)inventoryType));
                    }
                }
                else
                {
                    throw new Exception("inventory command length zero");
                }
            }
            else if (messageId == MessageId.InventoryResponse)
            {
                if (messageBody.Length > 0)
                {
                    InventoryType inventoryType = (InventoryType)messageBody[0];

                    if (inventoryType == InventoryType.ListActiveKsetIds)
                    {
                        KmmBody kmmBody = new InventoryResponseListActiveKsetIds();
                        kmmBody.Parse(messageBody);
                        KmmBody = kmmBody;
                    }
                    else if (inventoryType == InventoryType.ListRsiItems)
                    {
                        KmmBody kmmBody = new InventoryResponseListRsiItems();
                        kmmBody.Parse(messageBody);
                        KmmBody = kmmBody;
                    }
                    else if (inventoryType == InventoryType.ListActiveSuId)
                    {
                        KmmBody kmmBody = new InventoryResponseListActiveSuId();
                        kmmBody.Parse(messageBody);
                        KmmBody = kmmBody;
                    }
                    else if (inventoryType == InventoryType.ListSuIdItems)
                    {
                        KmmBody kmmBody = new InventoryResponseListSuIdItems(messageBody);
                        kmmBody.Parse(messageBody);
                        KmmBody = kmmBody;
                    }
                    else if (inventoryType == InventoryType.ListKeysetTaggingInfo)
                    {
                        KmmBody kmmBody = new InventoryResponseListKeysetTaggingInfo();
                        kmmBody.Parse(messageBody);
                        KmmBody = kmmBody;
                    }
                    else if (inventoryType == InventoryType.ListActiveKeys)
                    {
                        KmmBody kmmBody = new InventoryResponseListActiveKeys();
                        kmmBody.Parse(messageBody);
                        KmmBody = kmmBody;
                    }
                    else if (inventoryType == InventoryType.ListMnp)
                    {
                        KmmBody kmmBody = new InventoryResponseListMnp();
                        kmmBody.Parse(messageBody);
                        KmmBody = kmmBody;
                    }
                    else if (inventoryType == InventoryType.ListKmfRsi)
                    {
                        KmmBody kmmBody = new InventoryResponseListKmfRsi();
                        kmmBody.Parse(messageBody);
                        KmmBody = kmmBody;
                    }
                    else
                    {
                        throw new Exception(string.Format("unknown inventory response type: 0x{0:X2}", (byte)inventoryType));
                    }
                }
                else
                {
                    throw new Exception("inventory response length zero");
                }
            }
            else if (messageId == MessageId.ModifyKeyCommand)
            {
                KmmBody kmmBody = new ModifyKeyCommand();
                kmmBody.Parse(messageBody);
                KmmBody = kmmBody;
            }
            else if (messageId == MessageId.NegativeAcknowledgment)
            {
                KmmBody kmmBody = new NegativeAcknowledgment();
                kmmBody.Parse(messageBody);
                KmmBody = kmmBody;
            }
            else if (messageId == MessageId.RekeyAcknowledgment)
            {
                KmmBody kmmBody = new RekeyAcknowledgment();
                kmmBody.Parse(messageBody);
                KmmBody = kmmBody;
            }
            else if (messageId == MessageId.ZeroizeResponse)
            {
                KmmBody kmmBody = new ZeroizeResponse();
                kmmBody.Parse(messageBody);
                KmmBody = kmmBody;
            }
            else if (messageId == MessageId.LoadAuthenticationKeyResponse)
            {
                KmmBody kmmBody = new LoadAuthenticationKeyResponse(messageBody);
                kmmBody.Parse(messageBody);
                KmmBody = kmmBody;
            }
            else if (messageId == MessageId.DeleteAuthenticationKeyResponse)
            {
                KmmBody kmmBody = new DeleteAuthenticationKeyResponse(messageBody);
                kmmBody.Parse(messageBody);
                KmmBody = kmmBody;
            }
            else if (messageId == MessageId.SessionControl)
            {
                if (messageBody.Length > 0)
                {
                    byte version = messageBody[0];

                    if (version == 0x00)
                    {
                        KmmBody kmmBody = new SessionControl();
                        kmmBody.Parse(messageBody);
                        KmmBody = kmmBody;
                    }
                    else
                    {
                        throw new Exception(string.Format("unknown session control - version: 0x{0:X2}", version));
                    }
                }
                else
                {
                    throw new Exception("session control body length zero");
                }
            }
            else if (messageId == MessageId.LoadConfigResponse)
            {
                KmmBody kmmBody = new LoadConfigResponse();
                kmmBody.Parse(messageBody);
                KmmBody = kmmBody;
            }
            else
            {
                throw new Exception(string.Format("unknown kmm - message id: {0} (0x{1:X2})", messageId.ToString(), (byte)messageId));
            }
        }

        private void Mfid90Parse(byte[] contents)
        {
            if (contents.Length < 10)
            {
                throw new ArgumentOutOfRangeException(string.Format("length mismatch - expected at least 10, got {0} - {1}", contents.Length.ToString(), BitConverter.ToString(contents)));
            }

            MessageId messageId = (MessageId)contents[0];

            Logger.Debug("rx motorola mfid kmm frame, message id: {0} (0x{1:X2}), contents: {2}", messageId.ToString(), (byte)messageId, Utility.DataFormat(contents));

            int messageLength = 0;
            messageLength |= contents[1] << 8;
            messageLength |= contents[2];

            int messageBodyLength = messageLength - 7;
            byte[] messageBody = new byte[messageBodyLength];
            Array.Copy(contents, 10, messageBody, 0, messageBodyLength);

            /*
             * motorola likes using their mfid for _some_ standard kmms
             * 
             * this is probably because the standard kmms came from motorola's implementation
             * and this behavior is intentional for backwards compatibility with older kvl firmware
             * 
             * so we don't have to duplicate each kmm, just handle the kmms we know are different
             * and hand all other kmms with the motorola mfid to the standard handler
             */

            if (messageId == MessageId.SessionControl)
            {
                if (messageBody.Length > 0)
                {
                    byte version = messageBody[0];

                    if (version == 0x01)
                    {
                        KmmBody kmmBody = new Mfid90SessionControlVer1();
                        kmmBody.Parse(messageBody);
                        KmmBody = kmmBody;
                    }
                    else
                    {
                        throw new Exception(string.Format("unknown session control - version: 0x{0:X2}", version));
                    }
                }
                else
                {
                    throw new Exception("session control body length zero");
                }
            }
            else
            {
                Parse(contents);
            }
        }

        private void ParseWithPreamble(byte[] contents)
        {
            // TODO bounds check

            const byte expectedVersion = 0x00;

            byte parsedVersion = contents[0];

            if (parsedVersion != expectedVersion)
            {
                throw new Exception(string.Format("unknown preamble version: 0x{0:X2}, expected 0x{1:X2}", parsedVersion, expectedVersion));
            }

            byte mfid = contents[1];

            // TODO algid

            // TODO keyid

            // TODO mi

            byte[] frame = new byte[contents.Length - 14];

            Array.Copy(contents, 14, frame, 0, (contents.Length - 14));

            if (mfid == 0x00)
            {
                Parse(frame);
            }
            else if (mfid == 0x90)
            {
                Mfid90Parse(frame);
            }
            else
            {
                throw new Exception(string.Format("unknown mfid: 0x{0:X2}", mfid));
            }
        }
    }
}
