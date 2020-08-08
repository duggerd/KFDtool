using KFDtool.Shared;
using System;

namespace KFDtool.P25.Kmm
{
    public class InventoryResponseListActiveSuId : KmmBody
    {
        public InventoryType InventoryType { get; private set; }

        public bool ActiveSuId { get; private set; }

        public bool KeyAssigned { get; private set; }

        public SuId SuId { get; private set; }

        public OperationStatus Status { get; private set; }

        public override MessageId MessageId
        {
            get
            {
                return MessageId.InventoryResponse;
            }
        }

        public override ResponseKind ResponseKind
        {
            get
            {
                return ResponseKind.None;
            }
        }

        public InventoryResponseListActiveSuId()
        {
        }

        public override byte[] ToBytes()
        {
            throw new NotImplementedException();
        }

        public override void Parse(byte[] contents)
        {
            int expectedLength = 10;

            if (contents.Length != expectedLength)
            {
                throw new Exception(
                    string.Format(
                        "Parsing InventoryResponseListActiveSuId failed" + Environment.NewLine +
                        "Reason: Length mismatch - expected {0}, got {1}" + Environment.NewLine +
                        "Contents: {2}",
                        expectedLength,
                        contents.Length,
                        Utility.DataFormat(contents)
                    )
                );
            }

            /* inventory type */
            InventoryType = (InventoryType)contents[0];

            /* inventory instruction */
            ActiveSuId = Convert.ToBoolean(contents[1] & 0x01);
            KeyAssigned = Convert.ToBoolean(contents[1] & 0x02);

            /* suid */
            byte[] suId = new byte[7];
            Array.Copy(contents, 2, suId, 0, 7);
            SuId = new SuId(suId);

            /* status */
            Status = (OperationStatus)contents[9];
        }
    }
}
