using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Intel_8086.Registers;

namespace Tests_Intel_8086
{
    [TestClass]
    public class IndexRegistersTest
    {
        public void StartAllTests()
        {
            TestSetAndReadBytes();
        }

        [TestMethod]
        public void TestSetAndReadBytes() //MOV AL, BH
        {
            byte[] test;
            RegistersController registry = new IndexRegisters();
            registry.SetBytesToRegistry("SI", 0, 1);
            test = registry.GetRegistry("SI");
            Assert.AreEqual(BitConverter.ToUInt16(test), 256);

            registry.SetBytesToRegistry("DI", 0, 255);
            test = registry.GetRegistry("DI");
            Assert.AreEqual(BitConverter.ToUInt16(test), 65280);

            registry.SetBytesToRegistry("SI", 1, 0, 1, 0, 1, 0);
            test = registry.GetRegistry("SI");
            Assert.AreEqual(BitConverter.ToUInt16(test), 1);

            registry.SetBytesToRegistry("DI", 255);
            test = registry.GetRegistry("DI");
            Assert.AreEqual(BitConverter.ToUInt16(test), 255);

            registry.SetBytesToRegistry("SI");
            test = registry.GetRegistry("SI");
            Assert.AreEqual(BitConverter.ToUInt16(test), 1);
        }
    }
}
