namespace Intel_8086.Registers
{
    public interface RegistersController
    {
        bool Contains(string registryName);
        byte[] GetRegistry(string registryName);
        void SetBytesToRegistry(string registryName, params byte[] bytes);
    }
}
