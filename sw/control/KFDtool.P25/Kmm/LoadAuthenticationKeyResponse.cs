using System;

namespace KFDtool.P25.Kmm
{
    public class LoadAuthenticationKeyResponse : KmmBody
    {
        public bool AssignmentSuccess { get; private set; }

        public SuId SuId { get; private set; }

        public OperationStatus Status { get; private set; }

        public override MessageId MessageId
        {
            get
            {
                return MessageId.LoadAuthenticationKeyResponse;
            }
        }

        public override ResponseKind ResponseKind
        {
            get
            {
                return ResponseKind.None;
            }
        }

        public LoadAuthenticationKeyResponse(byte[] contents)
        {
            Parse(contents);
        }

        public override byte[] ToBytes()
        {
            throw new NotImplementedException();
        }

        public override void Parse(byte[] contents)
        {
            if (contents.Length != 9)
            {
                throw new ArgumentOutOfRangeException("contents", string.Format("length mismatch - expected 9, got {0} - {1}", contents.Length.ToString(), BitConverter.ToString(contents)));
            }

            /* authentication instruction */
            AssignmentSuccess = Convert.ToBoolean(contents[0] & 0x01);

            /* suid */
            byte[] suId = new byte[7];
            Array.Copy(contents, 1, suId, 0, 7);
            SuId = new SuId(suId);

            /* status */
            Status = (OperationStatus)contents[8];
        }
    }
}
