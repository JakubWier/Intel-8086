using Intel_8086.Registers;

namespace Intel_8086.Console
{
    public interface RegistryCommandHandler
    {
        RegistryCommandHandler NextHandler { get; set; }
        string HandleOperation(string[] args, params RegistersController[] supportedRegisters);
    }
}
