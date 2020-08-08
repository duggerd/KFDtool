using System;
using System.Collections.Generic;

namespace KFDtool.P25.Kmm
{
    /* TIA 102.AACA-A 10.2.22 */
    public class NegativeAcknowledgment : KmmBody
    {
        public MessageId AcknowledgedMessageId { get; set; }

        public int MessageNumber { get; set; }

        public OperationStatus Status { get; set; }

        public override MessageId MessageId
        {
            get
            {
                return MessageId.NegativeAcknowledgment;
            }
        }

        public override ResponseKind ResponseKind
        {
            get
            {
                return ResponseKind.None;
            }
        }

        public NegativeAcknowledgment()
        {
        }

        public override byte[] ToBytes()
        {
            List<byte> contents = new List<byte>();

            /* acknowledged message id */
            contents.Add((byte)AcknowledgedMessageId);

            /* message number */
            contents.Add((byte)((MessageNumber >> 8) & 0xFF));
            contents.Add((byte)(MessageNumber & 0xFF));

            /* status */
            contents.Add((byte)Status);

            return contents.ToArray();
        }

        public override void Parse(byte[] contents)
        {
            // we have to handle both the p25 standard (4 bytes) as well as the motorola variant used in LLA and DLI (2 bytes)
            // motorola uses the standard mfid for their non-standard implementation, so we have to use the length to determine how to parse

            if (contents.Length == 2) // motorola variant
            {
                /* acknowledged message id */
                AcknowledgedMessageId = (MessageId)contents[0];

                /* status */
                Status = (OperationStatus)contents[1];
            }
            else if (contents.Length == 4) // p25 standard
            {
                /* acknowledged message id */
                AcknowledgedMessageId = (MessageId)contents[0];

                /* message number */
                MessageNumber |= contents[1] << 8;
                MessageNumber |= contents[2];

                /* status */
                Status = (OperationStatus)contents[3];
            }
            else
            {
                throw new ArgumentOutOfRangeException("contents", string.Format("length mismatch - expected 2 or 4, got {0} - {1}", contents.Length.ToString(), BitConverter.ToString(contents)));
            }
        }
    }
}
