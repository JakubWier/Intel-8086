using System.Collections.Generic;

namespace Intel_8086.Registers
{
    class SegmentRegisters : RegistersController, Observable
    {
        private List<Observer> observers;

        byte[][] registryBlock;
        public SegmentRegisters()
        {
            registryBlock = new byte[4][];
            registryBlock[0] = new byte[2];
            registryBlock[1] = new byte[2];
            registryBlock[2] = new byte[2];
            registryBlock[3] = new byte[2];

            observers = new List<Observer>();
        }

        public byte[] GetRegistry(string registryName)
        {
            int regIndex = ToRegistryIndex(registryName);
            return registryBlock[regIndex];
        }

        public void SetBytesToRegistry(string registryName, params byte[] bytes)
        {
            int registryIndex = ToRegistryIndex(registryName);
            if (bytes == null || bytes.Length == 0 || registryName.Equals(null) || registryIndex == -1)
                return;

            registryBlock[registryIndex][0] = bytes[0];
            if (bytes.Length > 1)
                registryBlock[registryIndex][1] = bytes[1];
            else
                registryBlock[registryIndex][1] = 0;

            (string regName, byte[] newValue) data = (registryName, registryBlock[registryIndex]);
            UpdateObservers(data);
            return;
        }

        private int ToRegistryIndex(string registryName) => registryName switch
        {
            "CS" => 0,
            "SS" => 1,
            "DS" => 2,
            "ES" => 3,
            _ => -1
        };

        public bool Contains(string registryName)
        {
            if (ToRegistryIndex(registryName) != -1)
                return true;
            return false;
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
