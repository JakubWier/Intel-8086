using System;
using System.Collections;

namespace Intel_8086.Memory
{
    public class MemoryModel
    {
        private static MemoryModel instance;
        static int addressBusLength = 0;
        byte[] memoryBlock;

        public int GetMemoryLength => memoryBlock.Length;
        public static int SetAddressBusLength { set { instance = new MemoryModel(value); } }

        private MemoryModel(int addressBusLength) {
            memoryBlock = new byte[(int)Math.Pow(2, addressBusLength)];
            MemoryModel.addressBusLength = addressBusLength;
        }

        public static MemoryModel GetInstance() //Naive implementation
        {
            if (instance == null)
                instance = new MemoryModel(addressBusLength);
            return instance;
        }

        public byte GetMemoryCell(int address) => memoryBlock[address];

        public void SetMemoryCell(int address, byte value)
        {
            memoryBlock[address] = value;
        }
    }
}
