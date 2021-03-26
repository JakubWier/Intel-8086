namespace Intel_8086.Registers
{
    interface Registry
    {
        byte[] Bytes { get; }
        void TrySetBytes(int startIndex = 0, params byte[] bytes);
        bool IsRegistry(string registryName, out Registry registry);
        string Name { get; }
    }
}
