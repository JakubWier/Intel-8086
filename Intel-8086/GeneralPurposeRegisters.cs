using System;
using System.Collections.Generic;

namespace Intel_8086
{
    public delegate void RegistryChangedHandler(byte newValue);
    class GeneralPurposeRegisters : IRegistryModel, IObservable
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

        public GeneralPurposeRegisters(params IObserver[] observer)
        {
            registryBlock = new byte[4][];
            registryBlock[0] = new byte[2];
            registryBlock[1] = new byte[2];
            registryBlock[2] = new byte[2];
            registryBlock[3] = new byte[2];

            observers = new List<IObserver>(observer);
        }

        byte[][] registryBlock;

        public byte[] GetRegistry(RegistryType registryType)
        {
            int regIndex = (int)registryType % 4;
            switch (regIndex)
            {
                case 0:
                    return registryBlock[0];
                case 1:
                    return registryBlock[1];
                case 2:
                    return registryBlock[2];
                case 3:
                    return registryBlock[3];
                default:
                    return null;
            }
        }

        /// <summary>
        /// Sets passed bytes to selected register.
        /// Function intentionally simulates data loss if parameter "bytes" is too wide for 16bit registry or it's 8bit half. 
        /// </summary>
        public void SetBytesToRegistry(RegistryType registryType, params byte[] bytes)
        {
            int registryIndex = (int)registryType;
            if (bytes == null || registryType.Equals(null) || registryIndex > 11)
                return;

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
            else
            {
                registryIndex -= 8;
                SetLowByte(registryIndex, bytes[0]);
            }
            (RegistryType reg, byte[] newValue) data = (registryType, registryBlock[registryIndex]);
            UpdateObservers(data);
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

        public void UpdateObservers(object data)
        {
            foreach (IObserver observer in observers)
                observer.Update(data);
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
