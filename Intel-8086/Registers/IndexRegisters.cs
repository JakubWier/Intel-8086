using System.Collections.Generic;

namespace Intel_8086.Registers
{
    class IndexRegisters : Observable
    {
        private List<Observer> observers;

        byte[][] registryBlock;
        public IndexRegisters()
        {
            registryBlock = new byte[2][];
            registryBlock[0] = new byte[2];
            registryBlock[1] = new byte[2];

            observers = new List<Observer>();
        }

        public IndexRegisters(params Observer[] observer)
        {
            registryBlock = new byte[2][];
            registryBlock[0] = new byte[2];
            registryBlock[1] = new byte[2];

            observers = new List<Observer>(observer);
        }

        public byte[] GetRegistry(IndexRegistryType registryType)
        {
            int regIndex = (int)registryType % 2;
            switch (regIndex)
            {
                case 0:
                    return registryBlock[0];
                case 1:
                    return registryBlock[1];
                default:
                    return null;
            }
        }

        /// <summary>
        /// Sets passed bytes to selected register.
        /// Function intentionally simulates data loss if parameter "bytes" is too wide for 16bit registry or it's 8bit half. 
        /// </summary>
        public void SetBytesToRegistry(IndexRegistryType registryType, params byte[] bytes)
        {
            int registryIndex = (int)registryType;
            if (bytes == null || registryType.Equals(null) || registryIndex > 11)
                return;

            registryBlock[registryIndex][0] = bytes[0];
            if (bytes.Length > 1)
                registryBlock[registryIndex][1] = bytes[1];
            else
                registryBlock[registryIndex][1] = 0;

            (IndexRegistryType reg, byte[] newValue) data = (registryType, registryBlock[registryIndex]);
            UpdateObservers(data);
            return;
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
