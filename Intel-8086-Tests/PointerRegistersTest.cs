using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Intel_8086.Registers;

namespace Tests_Intel_8086
{
    [TestClass]
    public class PointerRegistersTest
    {
        public void StartAllTests()
        {
            TestSetAndReadBytes();
        }

        [TestMethod]
        public void TestSetAndReadBytes() //MOV AL, BH
        {
            byte[] test;
            RegistersController registry = new PointerRegisters();
            registry.SetBytesToRegistry("BP", 0, 1);
            test = registry.GetRegistry("BP");
            Assert.AreEqual(BitConverter.ToUInt16(test), 256);

            registry.SetBytesToRegistry("SP", 0, 255);
            test = registry.GetRegistry("SP");
            Assert.AreEqual(BitConverter.ToUInt16(test), 65280);

            registry.SetBytesToRegistry("BP", 1, 0, 1, 0, 1, 0);
            test = registry.GetRegistry("BP");
            Assert.AreEqual(BitConverter.ToUInt16(test), 1);

            registry.SetBytesToRegistry("SP", 255);
            test = registry.GetRegistry("SP");
            Assert.AreEqual(BitConverter.ToUInt16(test), 255);

            registry.SetBytesToRegistry("BP");
            test = registry.GetRegistry("BP");
            Assert.AreEqual(BitConverter.ToUInt16(test), 1);
        }
    }
}
