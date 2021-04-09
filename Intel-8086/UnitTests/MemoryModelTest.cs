using System;
using static System.Diagnostics.Debug;
using Intel_8086.Registers;
using Intel_8086.Memory;

namespace Tests_Intel_8086
{
    class MemoryModelTest
    {
        public void StartAllTests()
        {
            TestMemory();
        }

        public void TestMemory() //MOV AL, BH
        {
            MemoryModel.SetAddressBusLength = 20;
            MemoryModel memory = MemoryModel.GetInstance();
            string addressHexMock = "FFFFE";
            uint.TryParse(addressHexMock, System.Globalization.NumberStyles.HexNumber, null, out uint result);
            memory.SetMemoryWord(result, new byte[] {255,255});

            byte cell = memory.GetMemoryCell(result);
            Assert(cell == 255);
            cell = memory.GetMemoryCell(result + 1);
            Assert(cell == 255);
            cell = memory.GetMemoryCell(result - 2);
            Assert(cell == 0);

            MemoryModel.SetAddressBusLength = 0;
        }
    }
}
