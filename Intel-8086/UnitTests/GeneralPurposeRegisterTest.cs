using static System.Diagnostics.Debug;
using System;
using Intel_8086.Registers;

namespace Tests_Intel_8086
{
    class GeneralPurposeRegisterTest
    {
        public void StartAllTests()
        {
            TestSetAndReadBytes();
        }

        public void TestSetAndReadBytes()
        {
            byte[] test;
            Registry ax = new HalfRegistry("AX", 2);
            RegistryOperator registry = new GeneralPurposeRegisters(new Registry[] { ax });
            registry.TrySetBytesToRegistry("AX", 0, 1);
            /*test = registry.GetRegistry(GeneralPurposeRegistryType.AX);
            Assert(BitConverter.ToUInt16(test) == 256, $"Registry set AX failure, test value: {BitConverter.ToUInt16(test)} expected 256");

            registry.TrySetBytesToRegistry("BH", 0, 255);
            test = registry.GetRegistry(GeneralPurposeRegistryType.BX);
            Assert(BitConverter.ToUInt16(test) == 0, $"Registry set BX failure, test value: {BitConverter.ToUInt16(test)} expected 65280");

            registry.TrySetBytesToRegistry("CL", 1, 0, 1, 0, 1, 0);
            test = registry.GetRegistry(GeneralPurposeRegistryType.CX);
            Assert(BitConverter.ToUInt16(test) == 1, $"Registry set CX failure, test value: {BitConverter.ToUInt16(test)} expected 1");

            //Only one argument should set to lower byte.
            registry.TrySetBytesToRegistry("DX", 255);
            test = registry.GetRegistry(GeneralPurposeRegistryType.DX);
            Assert(BitConverter.ToUInt16(test) == 255, $"Registry set DX failure, test value: {BitConverter.ToUInt16(test)} expected 255");

            //We've set 255 to lower byte and now we are setting 2 to higher.
            //It's becouse we can send even 32bit to DH, then it will take second byte if array may represent 16bit.
            registry.TrySetBytesToRegistry("DH", 2);
            test = registry.GetRegistry(GeneralPurposeRegistryType.DX);
            Assert(BitConverter.ToUInt16(test) == (255 + 2 * 256), $"Registry set DX failure, test value: {BitConverter.ToUInt16(test)} expected 767");*/
        }
    }
}
