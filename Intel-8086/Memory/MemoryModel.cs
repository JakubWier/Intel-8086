using System;
using System.Collections;

namespace Intel_8086.Memory
{
    public class MemoryModel
    {
        byte[] memoryBlock;
        BitArray adressBus;

        public Int16 CS { get; }
        public Int16 SS { get; }
        public Int16 DS { get; }
        public Int16 ES { get; }

        public MemoryModel(int adressBusLength)
        {
            memoryBlock = new byte[(int)Math.Pow(2, adressBusLength)];
            adressBus = new BitArray(adressBusLength);

            CS = 0;
            SS = 4;
            DS = 8;
            ES = 12;
        }

        public byte GetMemoryCell(int adress)
        {
            return memoryBlock[adress];
        }
    }
}
