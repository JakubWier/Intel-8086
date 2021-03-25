namespace Intel_8086.Registers
{
    class HalfRegistry : Registry
    {
        public byte[] Bytes
        {
            get => bytes;
            set
            {
                bytes = value;
            }
        }

        public byte this[int i] { 
            get => bytes[i]; 
            set { bytes[i] = value; }
        }

        public string Name { get; }
        byte[] bytes;

        public HalfRegistry(string name, int bytesLength)
        {
            Name = name;
            bytes = new byte[bytesLength];
        }
    }
}
