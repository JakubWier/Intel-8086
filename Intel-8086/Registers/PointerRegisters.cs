using System.Collections.Generic;

namespace Intel_8086.Registers
{
    class PointerRegisters : RegistersController, Observable
    {
        private List<Observer> observers;

        byte[][] registryBlock;
        public PointerRegisters()
        {
            registryBlock = new byte[2][];
            registryBlock[0] = new byte[2];
            registryBlock[1] = new byte[2];

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
            "BP" => 0,
            "SP" => 1,
            _ => -1
        };

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

        public bool Contains(string registryName)
        {
            if (ToRegistryIndex(registryName) != -1)
                return true;
            return false;
        }
    }
}
