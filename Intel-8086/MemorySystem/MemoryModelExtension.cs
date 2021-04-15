using System;

namespace Intel_8086.MemorySystem
{
    public static class MemoryModelExtension
    {
        public static void SetMemoryWord(this MemoryModel memory, uint physicalAddress, UInt16 value)
        {
            byte[] word = BitConverter.GetBytes(value);
            memory.SetMemoryWord(physicalAddress, word);
        }

        public static byte[] GetMemoryWord(this MemoryModel memory, uint physicalAddress)
        {
            if (physicalAddress >= memory.GetMemoryLength - 1)
                throw new ArgumentException($"Address is out of range memory block.");

            return new[] { memory.GetMemoryCell(physicalAddress), memory.GetMemoryCell(physicalAddress + 1) }; //Little endian
        }
    }
}
