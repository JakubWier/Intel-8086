using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Intel_8086.Registers;

namespace Tests_Intel_8086
{
    [TestClass]
    public class SegmentRegistersTest
    {
        public void StartAllTests()
        {
            TestSetAndReadBytes();
        }

        [TestMethod]
        public void TestSetAndReadBytes() //MOV AL, BH
        {
            byte[] test;
            RegistersController registry = new SegmentRegisters();
            registry.SetBytesToRegistry("CS", 0, 1);
            test = registry.GetRegistry("CS");
            Assert.AreEqual(BitConverter.ToUInt16(test), 256);

            registry.SetBytesToRegistry("SS", 0, 255);
            test = registry.GetRegistry("SS");
            Assert.AreEqual(BitConverter.ToUInt16(test), 65280);

            registry.SetBytesToRegistry("DS", 1, 0, 1, 0, 1, 0);
            test = registry.GetRegistry("DS");
            Assert.AreEqual(BitConverter.ToUInt16(test), 1);

            registry.SetBytesToRegistry("ES", 255);
            test = registry.GetRegistry("ES");
            Assert.AreEqual(BitConverter.ToUInt16(test), 255);

            registry.SetBytesToRegistry("ES");
            test = registry.GetRegistry("ES");
            Assert.AreEqual(BitConverter.ToUInt16(test), 255);
        }
    }
}
