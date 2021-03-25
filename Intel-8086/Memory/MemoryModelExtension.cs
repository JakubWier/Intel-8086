using System;

namespace Intel_8086.Memory
{
    public static class MemoryModelExtension
    {
        public static byte[] GetWord(this MemoryModel memory, string adressHex)
        {
            if (Int32.TryParse(adressHex, out int adressIndex))
                return new[] { memory.GetCell(adressIndex), memory.GetCell(adressIndex+1) }; //Representation of Little endian
            else
                throw new ArgumentException($"Couldn't parse {adressHex} into Int32");
        }

        public static byte GetCell(this MemoryModel memory, string adressHex)
        {
            if (Int32.TryParse(adressHex, out int adressIndex))
                return memory.GetCell(adressIndex);
            else
                throw new ArgumentException($"Couldn't parse {adressHex} into Int32");
        }
    }
}
