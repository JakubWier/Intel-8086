namespace Intel_8086.Registers
{
    public interface RegistryController
    {
        bool Contains(string registryName);
        byte[] GetRegistry(string registryName);
        void SetBytesToRegistry(string registryName, params byte[] bytes);
    }
}
