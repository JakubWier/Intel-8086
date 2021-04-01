using System.Collections.Generic;

namespace Intel_8086.Registers
{
    class GeneralPurposeRegisters : RegistersController, Observable
    {
        private List<Observer> observers;

        byte[][] registryBlock;
        public GeneralPurposeRegisters()
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

        /// <summary>
        /// Sets passed bytes to selected register.
        /// Function intentionally simulates data loss if parameter "bytes" is too wide for 16bit registry or it's 8bit half. 
        /// </summary>
        public void SetBytesToRegistry(string registryName, params byte[] bytes)
        {
            int registryIndex = ToRegistryIndex(registryName);
            if (bytes == null || registryName.Equals(null) || registryIndex > 11)
                return;

            if (registryName.EndsWith('X'))
            {
                registryBlock[registryIndex][0] = bytes[0];
                if (bytes.Length > 1)
                    registryBlock[registryIndex][1] = bytes[1];
                else
                    registryBlock[registryIndex][1] = 0;
            }
            else if (registryName.EndsWith('H'))
            {
                SetHighByte(registryIndex, bytes[0]);
            }
            else if(registryName.EndsWith('L'))
            {
                SetLowByte(registryIndex, bytes[0]);
            }

            (string regName, byte[] newValue) data = (new string(registryName[0] + "X"), registryBlock[registryIndex]);
            UpdateObservers(data);
            return;
        }

        private int ToRegistryIndex(string registryName) => registryName switch
        {
            "AX" => 0,
            "AH" => 0,
            "AL" => 0,
            "BX" => 1,
            "BH" => 1,
            "BL" => 1,
            "CX" => 2,
            "CH" => 2,
            "CL" => 2,
            "DX" => 3,
            "DH" => 3,
            "DL" => 3,
            _ => -1
        };

        private void SetHighByte(int registryIndex, byte singleByte)
        {
            registryBlock[registryIndex][1] = singleByte;
        }
        private void SetLowByte(int registryIndex, byte singleByte)
        {
            registryBlock[registryIndex][0] = singleByte;
        }

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
