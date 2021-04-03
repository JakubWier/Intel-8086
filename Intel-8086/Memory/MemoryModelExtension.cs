using System;

namespace Intel_8086.Memory
{
    public static class MemoryModelExtension
    {
        public static byte[] GetMemoryWord(this MemoryModel memory, string adressHex)
        {
            if (Int32.TryParse(adressHex, out int adressIndex))
                return new[] { memory.GetMemoryCell(adressIndex), memory.GetMemoryCell(adressIndex + 1) }; //Little endian
            else
                throw new ArgumentException($"Couldn't parse {adressHex} into Int32");
        }

        public static byte GetMemoryCell(this MemoryModel memory, string adressHex)
        {
            if (Int32.TryParse(adressHex, out int adressIndex))
                return memory.GetMemoryCell(adressIndex);
            else
                throw new ArgumentException($"Couldn't parse {adressHex} into Int32");
        }
    }
}
