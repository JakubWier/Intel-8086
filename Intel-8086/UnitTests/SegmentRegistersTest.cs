using System;
using static System.Diagnostics.Debug;
using Intel_8086.Registers;

namespace Tests_Intel_8086
{
    class SegmentRegistersTest
    {
        public void StartAllTests()
        {
            TestSetAndReadBytes();
        }

        public void TestSetAndReadBytes() //MOV AL, BH
        {
            byte[] test;
            RegistersController registry = new SegmentRegisters();
            registry.SetBytesToRegistry("CS", 0, 1);
            test = registry.GetRegistry("CS");
            Assert(BitConverter.ToUInt16(test) == 256, $"Registry set CS failure, test value: {BitConverter.ToUInt16(test)} expected 256");

            registry.SetBytesToRegistry("SS", 0, 255);
            test = registry.GetRegistry("SS");
            Assert(BitConverter.ToUInt16(test) == 65280, $"Registry set SS failure, test value: {BitConverter.ToUInt16(test)} expected 65280");

            registry.SetBytesToRegistry("DS", 1, 0, 1, 0, 1, 0);
            test = registry.GetRegistry("DS");
            Assert(BitConverter.ToUInt16(test) == 1, $"Registry set DS failure, test value: {BitConverter.ToUInt16(test)} expected 1");

            registry.SetBytesToRegistry("ES", 255);
            test = registry.GetRegistry("ES");
            Assert(BitConverter.ToUInt16(test) == 255, $"Registry set ES failure, test value: {BitConverter.ToUInt16(test)} expected 255");

            registry.SetBytesToRegistry("ES");
            test = registry.GetRegistry("ES");
            Assert(BitConverter.ToUInt16(test) == 255, $"Registry set ES failure, test value: {BitConverter.ToUInt16(test)} expected 1");
        }
    }
}
