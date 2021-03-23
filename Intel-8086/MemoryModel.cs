using System;
using System.Collections;
using System.Linq;

namespace Intel_8086
{
    class MemoryModel
    {
        byte[] memoryBlock;
        BitArray adressBus;
        
        public MemoryModel(int adressBusLength)
        {
            memoryBlock = new byte[(int)Math.Pow(2, adressBusLength)];
            adressBus = new BitArray(adressBusLength);
        }

        public byte GetMemoryCell(byte[] entryAdress, byte[] offset)
        {
            throw new NotImplementedException();
        }
    }
}
