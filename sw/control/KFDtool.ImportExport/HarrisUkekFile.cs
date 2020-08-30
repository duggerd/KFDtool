using KFDtool.Shared;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;

namespace KFDtool.ImportExport
{
    public class HarrisUkekFile
    {
        private XmlDocument UkekDoc;

        public void ParseFile(byte[] data)
        {
            try
            {
                UkekDoc = new XmlDocument();

                using (MemoryStream ms = new MemoryStream(data))
                {
                    ms.Flush();
                    ms.Position = 0;

                    UkekDoc.Load(ms);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("unable to parse file", ex);
            }
        }

        public List<HarrisKek> GetKeks()
        {
            List<HarrisKek> keys = new List<HarrisKek>();

            XmlNodeList keyTable = UkekDoc.SelectNodes("/personality/paramdata/table[@id='4']/row");

            foreach (XmlNode node in keyTable)
            {
                // key type

                XmlNode keyTypeNode = node.SelectSingleNode("param[@name='KeyType']");

                string keyType = keyTypeNode.SelectSingleNode("@value").Value;

                if (keyType == "KEK")
                {
                    HarrisKek keyItem = new HarrisKek();

                    // key

                    XmlNode keyNode = node.SelectSingleNode("param[@name='Key']");

                    string key = keyNode.SelectSingleNode("@value").Value;

                    keyItem.Key = Utility.ByteStringToByteList(key);

                    // algorithm id

                    XmlNode algIdNode = node.SelectSingleNode("param[@name='ALGID']");

                    string algId = algIdNode.SelectSingleNode("@value").Value;

                    keyItem.AlgorithmId = int.Parse(algId);

                    // key id

                    XmlNode keyIdNode = node.SelectSingleNode("param[@name='KID']");

                    string keyId = keyIdNode.SelectSingleNode("@value").Value;

                    keyItem.KeyId = int.Parse(keyId);

                    // sln

                    XmlNode slnNode = node.SelectSingleNode("param[@name='SLN']");

                    string sln = slnNode.SelectSingleNode("@value").Value;

                    keyItem.Sln = int.Parse(sln);

                    // keyset id

                    XmlNode keysetIdNode = node.SelectSingleNode("param[@name='KSID']");

                    string keysetId = keysetIdNode.SelectSingleNode("@value").Value;

                    keyItem.KeysetId = int.Parse(keysetId);

                    // rsi

                    XmlNode rsiNode = node.SelectSingleNode("param[@name='RSI']");

                    string rsi = rsiNode.SelectSingleNode("@value").Value;

                    keyItem.Rsi = int.Parse(rsi);

                    keys.Add(keyItem);
                }
            }

            return keys;
        }

        public List<HarrisLlak> GetLlaks()
        {
            List<HarrisLlak> keys = new List<HarrisLlak>();

            XmlNodeList keyTable = UkekDoc.SelectNodes("/personality/paramdata/table[@id='4']/row");

            foreach (XmlNode node in keyTable)
            {
                // key type

                XmlNode keyTypeNode = node.SelectSingleNode("param[@name='KeyType']");

                string keyType = keyTypeNode.SelectSingleNode("@value").Value;

                if (keyType == "LLAK")
                {
                    HarrisLlak keyItem = new HarrisLlak();

                    // key

                    XmlNode keyNode = node.SelectSingleNode("param[@name='Key']");

                    string key = keyNode.SelectSingleNode("@value").Value;

                    keyItem.Key = Utility.ByteStringToByteList(key);

                    // algorithm id

                    XmlNode algidNode = node.SelectSingleNode("param[@name='ALGID']");

                    string algId = algidNode.SelectSingleNode("@value").Value;

                    keyItem.AlgId = int.Parse(algId);

                    // suid

                    XmlNode suidNode = node.SelectSingleNode("param[@name='SUID']");

                    string suid = suidNode.SelectSingleNode("@value").Value;

                    // wacn id

                    int wacnId = int.Parse(suid.Substring(0, 5), NumberStyles.HexNumber);

                    keyItem.WacnId = wacnId;

                    // system id

                    int systemId = int.Parse(suid.Substring(5, 3), NumberStyles.HexNumber);

                    keyItem.SystemId = systemId;

                    // unit id

                    int unitId = int.Parse(suid.Substring(8, 6), NumberStyles.HexNumber);

                    keyItem.UnitId = unitId;

                    keys.Add(keyItem);
                }
            }

            return keys;
        }
    }
}
