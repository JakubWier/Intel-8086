using System.Collections.Generic;

namespace Intel_8086.Registers
{
    //public delegate void RegistryChangedHandler(byte newValue);
    class GeneralPurposeRegisters : RegistryOperator, Observable
    {
        private List<Observer> observers;
        Registry []registers;
        public GeneralPurposeRegisters(Registry[] registers)
        {
            this.registers = registers;
            observers = new List<Observer>();
        }

        public GeneralPurposeRegisters(Registry[] registers, params Observer[] observer)
        {
            this.registers = registers;
            observers = new List<Observer>(observer);
        }

        public bool Contains(string registryName)
        {
            if (registryName == null)
                throw new System.NullReferenceException("RegistryName cannot be null.");

            registryName = registryName.Replace('L', 'X');
            registryName = registryName.Replace('H', 'X');

            foreach (Registry reg in registers)
                if (reg.Name == registryName)
                    return true;
            return false;
        }

        private bool Contains(string registryName, out Registry registry)
        {
            if (registryName == null)
                throw new System.NullReferenceException("RegistryName cannot be null.");

            registryName = registryName.Replace('L', 'X');
            registryName = registryName.Replace('H', 'X');

            foreach (Registry reg in registers)
                if (reg.Name == registryName)
                {
                    registry = reg;
                    return true;
                }
            registry = null;
            return false;
        }

        /// <summary>
        /// Searches for the registry and sets input bytes.
        /// Informs observers by sending registry reference to them.
        /// Function intentionally simulates data loss if parameter "bytes" is too wide for 16bit registry or it's 8bit half. 
        /// </summary>
        /// <returns>
        /// Returns false if registry do not exists.
        /// </returns>
        /// <exception cref="System.NullReferenceException">Thrown when one of the arguments is null.</exception>
        public bool TrySetBytesToRegistry(string registryName, params byte[] bytes)
        {
            if (!Contains(registryName, out Registry registry))
                return false;

            if (bytes == null)
                throw new System.NullReferenceException("Byte array cannot be null.");

            registry.TrySetBytes(bytes: bytes);

            /*if (registryName.EndsWith('L'))
                registry[0] = bytes[0];
            else if (registryName.EndsWith('H'))
                registry[1] = bytes[0];
            else
            {
                registry[0] = bytes[0];
                if (bytes.Length > 1)
                    registry[1] = bytes[1];
                else
                    registry[1] = 0;
            }*/
            UpdateObservers(registry);
            return true;
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
