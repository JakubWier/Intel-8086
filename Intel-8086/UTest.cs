using static System.Diagnostics.Debug;
using Intel_8086;

namespace Tests_Intel_8086
{
    static class UTest
    {
        public static void StartAllTests()
        {
            TestGeneralPurposeRegisters();
        }

        public static void TestGeneralPurposeRegisters()
        {
            GeneralPurposeRegisters registers = new GeneralPurposeRegisters();
            Assert(registers.GetAH.Equals(0));
            registers.SetBytes(RegistryType.AH, 255);
            Assert(registers.GetAH.Equals(255));
            registers.SetBytes(RegistryType.BL, 1);
            Assert(System.BitConverter.ToUInt16(registers.GetBX).Equals(1));
            registers.SetBytes(RegistryType.CL, 255);
            registers.SetBytes(RegistryType.CH, 255);
            Assert(System.BitConverter.ToUInt16(registers.GetCX).Equals(65535));
            registers.SetBytes(RegistryType.DX, 0, 255);
            Assert(System.BitConverter.ToUInt16(registers.GetDX).Equals(65280));
        }

    }
}
