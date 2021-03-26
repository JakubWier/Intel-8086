namespace Intel_8086.Registers
{
    class CommonRegistry : Registry
    {
        public CommonRegistry(string registryName, int length)
        {
            Name = registryName;
            bytes = new byte[length];
        }

        public byte[] Bytes => bytes;

        public string Name { get; }

        private byte[] bytes;

        public void TrySetBytes(int startIndex = 0, params byte[] bytes)
        {
            for(int i = startIndex; i < Bytes.Length; i++)
            {
                this.bytes[i] = bytes[i];
            }
        }

        public bool IsRegistry(string registryName, out Registry registry)
        {
            if (Name == registryName)
            {
                registry = this;
                return true;
            }
            registry = null;
            return false;
        }
    }
}
