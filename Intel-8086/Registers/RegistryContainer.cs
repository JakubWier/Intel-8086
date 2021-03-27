namespace Intel_8086.Registers
{
    public interface RegistryContainer
    {
        byte[] GetRegistry(GeneralPurposeRegistryType registryType);
        public void SetBytesToRegistry(GeneralPurposeRegistryType registryType, params byte[] bytes);
    }
}
