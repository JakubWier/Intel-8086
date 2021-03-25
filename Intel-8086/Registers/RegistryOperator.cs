namespace Intel_8086.Registers
{
    interface RegistryOperator
    {
        bool Contains(string registryName);
        bool TrySetBytesToRegistry(string registryName, params byte[] bytes);
    }
}
