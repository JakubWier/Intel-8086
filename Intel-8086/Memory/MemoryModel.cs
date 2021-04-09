using System;
using System.Collections.Generic;

namespace Intel_8086.Memory
{
    public class MemoryModel : Memory, Observable
    {
        private static MemoryModel instance;
        List<Observer> observers;
        static int addressBusLength = 0;
        byte[] memoryBlock;

        public int GetMemoryLength => memoryBlock.Length;
        public static int SetAddressBusLength { set { instance = new MemoryModel(value); } }

        private MemoryModel(int addressBusLength) {
            memoryBlock = new byte[(int)Math.Pow(2, addressBusLength)];
            MemoryModel.addressBusLength = addressBusLength;
            observers = new List<Observer>();
        }

        public static MemoryModel GetInstance() //Naive implementation
        {
            if (instance == null)
                instance = new MemoryModel(addressBusLength);
            return instance;
        }

        public byte GetMemoryCell(uint physicalAddress) => memoryBlock[physicalAddress];

        public void SetMemoryCell(uint physicalAddress, byte value)
        {
            memoryBlock[physicalAddress] = value;
            UpdateObservers(physicalAddress);
        }

        public void SetMemoryWord(uint physicalAddress, byte[] values)
        {
            memoryBlock[physicalAddress] = values[0];
            memoryBlock[physicalAddress + 1] = values[1];
            UpdateObservers(physicalAddress);
        }

        public void UpdateObservers(object data)
        {
            foreach (Observer observer in observers)
                observer.Update(data);
        }

        public void AddObserver(Observer observer)
        {
            observers.Add(observer);
        }

        public void RemoveObserver(Observer observer)
        {
            observers.Remove(observer);
        }
    }
}
