using System;
using static System.Diagnostics.Debug;
using Intel_8086.Registers;

namespace Tests_Intel_8086
{
    class PointerRegistersTest
    {
        public void StartAllTests()
        {
            TestSetAndReadBytes();
        }

        public void TestSetAndReadBytes() //MOV AL, BH
        {
            byte[] test;
            RegistersController registry = new IndexRegisters();
            registry.SetBytesToRegistry("BP", 0, 1);
            test = registry.GetRegistry("BP");
            Assert(BitConverter.ToUInt16(test) == 256, $"Registry set BP failure, test value: {BitConverter.ToUInt16(test)} expected 256");

            registry.SetBytesToRegistry("SP", 0, 255);
            test = registry.GetRegistry("SP");
            Assert(BitConverter.ToUInt16(test) == 65280, $"Registry set SP failure, test value: {BitConverter.ToUInt16(test)} expected 65280");

            registry.SetBytesToRegistry("BP", 1, 0, 1, 0, 1, 0);
            test = registry.GetRegistry("BP");
            Assert(BitConverter.ToUInt16(test) == 1, $"Registry set BP failure, test value: {BitConverter.ToUInt16(test)} expected 1");

            registry.SetBytesToRegistry("SP", 255);
            test = registry.GetRegistry("SP");
            Assert(BitConverter.ToUInt16(test) == 255, $"Registry set SP failure, test value: {BitConverter.ToUInt16(test)} expected 255");

            registry.SetBytesToRegistry("BP");
            test = registry.GetRegistry("BP");
            Assert(BitConverter.ToUInt16(test) == 1, $"Registry set BP failure, test value: {BitConverter.ToUInt16(test)} expected 1");
        }
    }
}
