using Intel_8086;
using System;
using static System.Diagnostics.Debug;

namespace Tests_Intel_8086
{
    class RegistryViewTest
    {
        public void StartAllTests()
        {
            TestUpdateAndFields();
        }

        public void TestUpdateAndFields()
        {
            RegistryView registryView = new RegistryView(new HexParser());
            ValueTuple<GeneralPurposeRegistryType, byte[]> tupleMock = new ValueTuple<GeneralPurposeRegistryType, byte[]>();

            tupleMock.Item1 = GeneralPurposeRegistryType.AX;
            tupleMock.Item2 = new byte[] { 0, 1 };
            registryView.Update(tupleMock);
            Assert(registryView.AX == "0x0100");

            tupleMock.Item1 = GeneralPurposeRegistryType.BX;
            tupleMock.Item2 = new byte[] { 0, 255 };
            registryView.Update(tupleMock);
            Assert(registryView.BX == "0xFF00");

            tupleMock.Item1 = GeneralPurposeRegistryType.CX;
            tupleMock.Item2 = new byte[] { 1, 1 };
            registryView.Update(tupleMock);
            Assert(registryView.CX == "0x0101");

            tupleMock.Item1 = GeneralPurposeRegistryType.DX;
            tupleMock.Item2 = new byte[] { 255, 0 };
            registryView.Update(tupleMock);
            Assert(registryView.DX == "0x00FF");
        }
    }
}
