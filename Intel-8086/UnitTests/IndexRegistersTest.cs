using System;
using static System.Diagnostics.Debug;
using Intel_8086.Registers;

namespace Tests_Intel_8086
{
    class IndexRegistersTest
    {
        public void StartAllTests()
        {
            TestSetAndReadBytes();
        }

        public void TestSetAndReadBytes() //MOV AL, BH
        {
            byte[] test;
            RegistersController registry = new IndexRegisters();
            registry.SetBytesToRegistry("SI", 0, 1);
            test = registry.GetRegistry("SI");
            Assert(BitConverter.ToUInt16(test) == 256, $"Registry set SI failure, test value: {BitConverter.ToUInt16(test)} expected 256");

            registry.SetBytesToRegistry("DI", 0, 255);
            test = registry.GetRegistry("DI");
            Assert(BitConverter.ToUInt16(test) == 65280, $"Registry set DI failure, test value: {BitConverter.ToUInt16(test)} expected 65280");

            registry.SetBytesToRegistry("SI", 1, 0, 1, 0, 1, 0);
            test = registry.GetRegistry("SI");
            Assert(BitConverter.ToUInt16(test) == 1, $"Registry set SI failure, test value: {BitConverter.ToUInt16(test)} expected 1");

            registry.SetBytesToRegistry("DI", 255);
            test = registry.GetRegistry("DI");
            Assert(BitConverter.ToUInt16(test) == 255, $"Registry set DI failure, test value: {BitConverter.ToUInt16(test)} expected 255");

            registry.SetBytesToRegistry("SI");
            test = registry.GetRegistry("SI");
            Assert(BitConverter.ToUInt16(test) == 1, $"Registry set SI failure, test value: {BitConverter.ToUInt16(test)} expected 1");
        }
    }
}
