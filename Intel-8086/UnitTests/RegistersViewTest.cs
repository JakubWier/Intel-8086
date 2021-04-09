using Intel_8086;
using System;
using static System.Diagnostics.Debug;
using Intel_8086.Registers;

namespace Tests_Intel_8086
{
    class RegistersViewTest
    {
        public void StartAllTests()
        {
            TestUpdateAndFields();
        }

        public void TestUpdateAndFields()
        {
            TestGeneralView();
            TestSegmentView();
            TestPointersView();
            TestIndexesView();
        }

        public void TestGeneralView()
        {
            GeneralPurposeRegistersView registryView = new GeneralPurposeRegistersView(new HexParser());
            ValueTuple<string, byte[]> tupleMock = new ValueTuple<string, byte[]>();

            tupleMock.Item1 = "AX";
            tupleMock.Item2 = new byte[] { 0, 1 };
            registryView.Update(tupleMock);
            Assert(registryView.AX == "0x0100");

            tupleMock.Item1 = "BX";
            tupleMock.Item2 = new byte[] { 0, 255 };
            registryView.Update(tupleMock);
            Assert(registryView.BX == "0xFF00");

            tupleMock.Item1 = "CX";
            tupleMock.Item2 = new byte[] { 1, 1 };
            registryView.Update(tupleMock);
            Assert(registryView.CX == "0x0101");

            tupleMock.Item1 = "DX";
            tupleMock.Item2 = new byte[] { 255, 0 };
            registryView.Update(tupleMock);
            Assert(registryView.DX == "0x00FF");
        }

        public void TestSegmentView()
        {
            SegmentRegistersView registryView = new SegmentRegistersView(new HexParser());
            ValueTuple<string, byte[]> tupleMock = new ValueTuple<string, byte[]>();

            tupleMock.Item1 = "CS";
            tupleMock.Item2 = new byte[] { 0, 1 };
            registryView.Update(tupleMock);
            Assert(registryView.CS == "0x0100");

            tupleMock.Item1 = "SS";
            tupleMock.Item2 = new byte[] { 0, 255 };
            registryView.Update(tupleMock);
            Assert(registryView.SS == "0xFF00");

            tupleMock.Item1 = "DS";
            tupleMock.Item2 = new byte[] { 1, 1 };
            registryView.Update(tupleMock);
            Assert(registryView.DS == "0x0101");

            tupleMock.Item1 = "ES";
            tupleMock.Item2 = new byte[] { 255, 0 };
            registryView.Update(tupleMock);
            Assert(registryView.ES == "0x00FF");
        }

        public void TestPointersView()
        {
            PointerRegistersView registryView = new PointerRegistersView(new HexParser());
            ValueTuple<string, byte[]> tupleMock = new ValueTuple<string, byte[]>();

            tupleMock.Item1 = "BP";
            tupleMock.Item2 = new byte[] { 0, 1 };
            registryView.Update(tupleMock);
            Assert(registryView.BP == "0x0100");

            tupleMock.Item1 = "SP";
            tupleMock.Item2 = new byte[] { 0, 255 };
            registryView.Update(tupleMock);
            Assert(registryView.SP == "0xFF00");

            tupleMock.Item1 = "BP";
            tupleMock.Item2 = new byte[] { 1, 1 };
            registryView.Update(tupleMock);
            Assert(registryView.BP == "0x0101");

            tupleMock.Item1 = "SP";
            tupleMock.Item2 = new byte[] { 255, 0 };
            registryView.Update(tupleMock);
            Assert(registryView.SP == "0x00FF");
        }

        public void TestIndexesView()
        {
            IndexRegistersView registryView = new IndexRegistersView(new HexParser());
            ValueTuple<string, byte[]> tupleMock = new ValueTuple<string, byte[]>();

            tupleMock.Item1 = "SI";
            tupleMock.Item2 = new byte[] { 0, 1 };
            registryView.Update(tupleMock);
            Assert(registryView.SI == "0x0100");

            tupleMock.Item1 = "DI";
            tupleMock.Item2 = new byte[] { 0, 255 };
            registryView.Update(tupleMock);
            Assert(registryView.DI == "0xFF00");

            tupleMock.Item1 = "SI";
            tupleMock.Item2 = new byte[] { 1, 1 };
            registryView.Update(tupleMock);
            Assert(registryView.SI == "0x0101");

            tupleMock.Item1 = "DI";
            tupleMock.Item2 = new byte[] { 255, 0 };
            registryView.Update(tupleMock);
            Assert(registryView.DI == "0x00FF");
        }
    }
}
