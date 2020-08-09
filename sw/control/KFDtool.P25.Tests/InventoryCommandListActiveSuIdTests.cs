using System;
using KFDtool.P25.Kmm;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KFDtool.P25.Tests.Kmm
{
    [TestClass]
    public class InventoryCommandListActiveSuIdTests
    {
        [TestMethod]
        public void RoundTrip()
        {
            InventoryCommandListActiveSuId txKmm = new InventoryCommandListActiveSuId();

            byte[] data = txKmm.ToBytes();

            InventoryCommandListActiveSuId rxKmm = new InventoryCommandListActiveSuId();

            rxKmm.Parse(data);
        }

        [TestMethod]
        public void IncorrectLength()
        {
            byte[] data = new byte[0];

            InventoryCommandListActiveSuId rxKmm = new InventoryCommandListActiveSuId();

            Assert.ThrowsException<Exception>(() => rxKmm.Parse(data));
        }

        [TestMethod]
        public void IncorrectInventoryType()
        {
            byte[] data = new byte[] { 0xFA }; // 0xFA is unassigned

            InventoryCommandListActiveSuId rxKmm = new InventoryCommandListActiveSuId();

            Assert.ThrowsException<Exception>(() => rxKmm.Parse(data));
        }
    }
}
