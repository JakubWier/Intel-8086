using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Intel_8086.Registers;

namespace Tests_Intel_8086
{
    [TestClass]
    public class GeneralPurposeRegisterTest
    {
        public void StartAllTests()
        {
            TestSetAndReadBytes();
        }

        [TestMethod]
        public void TestSetAndReadBytes() //MOV AL, BH
        {
            byte[] test;
            RegistersController registry = new GeneralPurposeRegisters();
            registry.SetBytesToRegistry("AX", 0, 1);
            test = registry.GetRegistry("AX");
            Assert.AreEqual(BitConverter.ToUInt16(test), 256);

            registry.SetBytesToRegistry("BH", 0, 255);
            test = registry.GetRegistry("BX");
            Assert.AreEqual(BitConverter.ToUInt16(test), 0);

            registry.SetBytesToRegistry("CL", 1, 0, 1, 0, 1, 0);
            test = registry.GetRegistry("CX");
            Assert.AreEqual(BitConverter.ToUInt16(test), 1);

            //Only one argument should set to lower byte.
            registry.SetBytesToRegistry("DX", 255);
            test = registry.GetRegistry("DX");
            Assert.AreEqual(BitConverter.ToUInt16(test), 255);

            //We've set 255 to lower byte and now we are setting 2 to higher.
            //It's becouse we can send even 32bit to DH, then it will take second byte if array may represent 16bit.
            registry.SetBytesToRegistry("DH", 2);
            test = registry.GetRegistry("DX");
            Assert.AreEqual(BitConverter.ToUInt16(test), (255 + 2 * 256));
        }
    }
}
