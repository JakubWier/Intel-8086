namespace Intel_8086.Registers
{
    class DoubleRegistry : Registry, Observable
    {
        Registry[] registries;

        public byte[] Bytes
        {
            get
            {
                int totalLength = 0;
                foreach (Registry reg in registries)
                    totalLength += reg.Bytes.Length;
                byte[] b = new byte[totalLength];
                int index = 0;
                foreach (Registry registry in registries)
                {
                    registry.Bytes.CopyTo(b, index);
                    index = registry.Bytes.Length;
                }
                return b;
            }
        }

        public readonly int Capacity;

        public string Name { get; }

        public DoubleRegistry(string name, params Registry[] registries)
        {
            Name = name;
            this.registries = registries;
            Capacity = registries.Length;
        }

        public void TrySetBytes(int startIndex = 0, params byte[] bytes)
        {
            if (bytes == null)
                throw new System.NullReferenceException("Cannot assign NULL to registry bytes.");

            byte[] regBytes;
            int i = startIndex;
            foreach (Registry registry in registries)
            {
                regBytes = registry.Bytes;
                for (int k = 0; k < regBytes.Length; i++, k++)
                {
                    regBytes[k] = bytes[i];
                }
            }

            UpdateObservers(this as Registry);
        }

        public bool IsRegistry(string registryName, out Registry registry)
        {
            if (registryName == null)
                throw new System.NullReferenceException("Cannot compare NULL to registry name.");

            if(registryName == Name)
            {
                registry = this;
                return true;
            }

            foreach (Registry reg in registries)
                if(reg.IsRegistry(registryName, out Registry foundRegistry))
                {
                    registry = foundRegistry;
                    return true;
                }
            registry = null;
            return false;
        }

        public Observer observer;

        public void UpdateObservers(object data)
        {
            observer.Update(this as Registry);
        }

        public void AddObserver(Observer observer)
        {
            this.observer = observer;
        }

        public void RemoveObserver(Observer observer)
        {
            throw new System.NotImplementedException();
        }
    }
}
