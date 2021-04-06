using System;

namespace Intel_8086.Memory
{
    public static class MemoryModelExtension
    {
        public static void SetMemoryWord(this MemoryModel memory, int address, Int16 value)
        {
            byte[] word = BitConverter.GetBytes(value);
            memory.SetMemoryCell(address, word[0]);
            memory.SetMemoryCell(address+1, word[1]);
        }

        public static byte[] GetMemoryWord(this MemoryModel memory, int address)
        {
            if (address >= memory.GetMemoryLength - 1)
                throw new ArgumentException($"Address is out of range memory block.");

            return new[] { memory.GetMemoryCell(address), memory.GetMemoryCell(address + 1) }; //Little endian
        }

        public static byte[] GetMemoryWord(this MemoryModel memory, string addressHex)
        {
            if (Int32.TryParse(addressHex, out int adressIndex))
                return new[] { memory.GetMemoryCell(adressIndex), memory.GetMemoryCell(adressIndex + 1) }; //Little endian
            else
                throw new ArgumentException($"Couldn't parse {addressHex} into Int32");
        }

        public static byte GetMemoryCell(this MemoryModel memory, string addressHex)
        {
            if (Int32.TryParse(addressHex, System.Globalization.NumberStyles.HexNumber, null, out int adressIndex))
                return memory.GetMemoryCell(adressIndex);
            else
                throw new ArgumentException($"Couldn't parse {addressHex} into Int32");
        }
    }
}
