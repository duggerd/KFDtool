namespace KFDtool.P25.TransferConstructs
{
    public class DliIpDevice
    {
        public enum ProtocolOptions
        {
            UDP
        }

        public ProtocolOptions ProtocolType { get; set; }

        public string Hostname { get; set; }

        public int Port { get; set; }

        public SessionControlOptions SessionControlType { get; set; }
    }
}
