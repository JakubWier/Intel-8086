namespace Intel_8086.Console
{
    public interface CommandInterpreter
    {
        void InputCommand(string line);
        void AddHandler(RegistryCommandHandler handler);
    }
}
