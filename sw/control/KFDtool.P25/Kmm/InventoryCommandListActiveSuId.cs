using KFDtool.Shared;
using System;

namespace KFDtool.P25.Kmm
{
    public class InventoryCommandListActiveSuId : KmmBody
    {
        public override MessageId MessageId
        {
            get
            {
                return MessageId.InventoryCommand;
            }
        }

        public InventoryType InventoryType
        {
            get
            {
                return InventoryType.ListActiveSuId;
            }
        }

        public override ResponseKind ResponseKind
        {
            get
            {
                return ResponseKind.Immediate;
            }
        }

        public InventoryCommandListActiveSuId()
        {
        }

        public override byte[] ToBytes()
        {
            byte[] contents = new byte[1];

            /* inventory type */
            contents[0] = (byte)InventoryType;

            return contents;
        }

        public override void Parse(byte[] contents)
        {
            int expectedLength = 1;

            if (contents.Length != expectedLength)
            {
                throw new Exception(
                    string.Format(
                        "Parsing InventoryCommandListActiveSuId failed" + Environment.NewLine +
                        "Reason: Length mismatch - expected {0}, got {1}" + Environment.NewLine +
                        "Contents: {2}",
                        expectedLength,
                        contents.Length,
                        Utility.DataFormat(contents)
                    )
                );
            }

            InventoryType expectedInventoryType = InventoryType.ListActiveSuId;

            InventoryType parsedInventoryType = (InventoryType)contents[0];

            if (parsedInventoryType != expectedInventoryType)
            {
                throw new Exception(
                    string.Format(
                        "Parsing InventoryCommandListActiveSuId failed" + Environment.NewLine +
                        "Reason: InventoryType at index 0 mismatch - expected {0} (0x{1:X2}), got {2} (0x{3:X2})" + Environment.NewLine +
                        "Contents: {4}",
                        expectedInventoryType.ToString(),
                        (byte)expectedInventoryType,
                        parsedInventoryType.ToString(),
                        (byte)parsedInventoryType,
                        Utility.DataFormat(contents)
                    )
                );
            }
        }
    }
}
