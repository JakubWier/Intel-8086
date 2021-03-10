using System;
using System.Collections.Generic;

namespace Intel_8086
{
    public delegate void RegistryChangedHandler(byte newValue);
    class GeneralPurposeRegisters : IRegistry, IObservable
    {
        private List<IObserver> observers;
        public GeneralPurposeRegisters()
        {
            registryBlock = new byte[4][];
            registryBlock[0] = new byte[2];
            registryBlock[1] = new byte[2];
            registryBlock[2] = new byte[2];
            registryBlock[3] = new byte[2];

            observers = new List<IObserver>();
        }

        byte[][] registryBlock;

        public byte[] GetRegistry(RegistryType registryType) => registryType switch
        {
            RegistryType.AX => registryBlock[0],
            RegistryType.BX => registryBlock[0],
            RegistryType.CX => registryBlock[0],
            RegistryType.DX => registryBlock[0],
            _ => new byte[2]
        };

        /// <summary>
        /// Sets passed bytes to selected register.
        /// Function intentionally simulates data loss if parameter "bytes" is too wide for 16bit registry or it's 8bit half. 
        /// </summary>
        public void SetBytesToRegistry(RegistryType registryType, params byte[] bytes)
        {
            if (bytes == null || registryType.Equals(null))
                return;
            int registryIndex = (int)registryType;

            if (registryType <= RegistryType.DX)
            {
                registryBlock[registryIndex][0] = bytes[0];
                if (bytes.Length > 1)
                    registryBlock[registryIndex][1] = bytes[1];
                else
                    registryBlock[registryIndex][1] = 0;
            }
            else if (registryType >= RegistryType.AH && registryType <= RegistryType.DH)
            {
                registryIndex -= 4;
                SetHighByte(registryIndex, bytes[0]);
            }
            else if (registryType <= RegistryType.DL)
            {
                registryIndex -= 8;
                SetLowByte(registryIndex, bytes[0]);
            }
            else
                throw new ArgumentException("Not supported registry type");

            return;
        }

        private void SetHighByte(int registryIndex, byte singleByte)
        {
            registryBlock[registryIndex][1] = singleByte;
        }
        private void SetLowByte(int registryIndex, byte singleByte)
        {
            registryBlock[registryIndex][0] = singleByte;
        }

        public void UpdateObservers()
        {
            foreach(IObserver observer in observers)
        }

        public void AddObserver(IObserver observer)
        {
            observers.Add(observer);
        }

        public void RemoveObserver(IObserver observer)
        {
            observers.Remove(observer);
        }
    }
}
