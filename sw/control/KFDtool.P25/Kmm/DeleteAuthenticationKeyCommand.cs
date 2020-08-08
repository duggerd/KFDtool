﻿using System;
using System.Collections;

namespace KFDtool.P25.Kmm
{
    public class DeleteAuthenticationKeyCommand : KmmBody
    {
        public bool TargetSpecificSuId { get; private set; }

        public bool DeleteAllKeys { get; private set; }

        public SuId SuId { get; private set; }

        public override MessageId MessageId
        {
            get
            {
                return MessageId.DeleteAuthenticationKeyCommand;
            }
        }

        public override ResponseKind ResponseKind
        {
            get
            {
                return ResponseKind.Immediate;
            }
        }

        public DeleteAuthenticationKeyCommand(bool targetSpecificSuId, bool deleteAllKeys, SuId suId)
        {
            if (suId == null)
            {
                throw new ArgumentNullException("suId");
            }

            TargetSpecificSuId = targetSpecificSuId;
            DeleteAllKeys = deleteAllKeys;
            SuId = suId;
        }

        public override byte[] ToBytes()
        {
            byte[] contents = new byte[8];

            /* authentication instruction */
            BitArray authenticationInstruction = new BitArray(8, false);
            authenticationInstruction.Set(0, TargetSpecificSuId);
            authenticationInstruction.Set(1, DeleteAllKeys);
            authenticationInstruction.CopyTo(contents, 0);

            /* suid */
            byte[] suId = SuId.ToBytes();
            Array.Copy(suId, 0, contents, 1, suId.Length);

            return contents;
        }

        public override void Parse(byte[] contents)
        {
            throw new NotImplementedException();
        }
    }
}
