namespace KFDtool.P25.Kmm
{
    public abstract class KmmBody
    {
        public abstract MessageId MessageId { get; }

        public abstract ResponseKind ResponseKind { get; }

        public abstract byte[] ToBytes();

        public abstract void Parse(byte[] contents);
    }
}
