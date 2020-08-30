using System.Collections.Generic;

namespace KFDtool.ImportExport
{
    public class HarrisKek
    {
        public int KeysetId { get; set; }

        public int Sln { get; set; }

        public int KeyId { get; set; }

        public int AlgorithmId { get; set; }

        public int Rsi { get; set; }

        public List<byte> Key { get; set; }
    }
}
