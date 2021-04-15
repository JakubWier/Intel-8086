using Intel_8086.Registers;

namespace Intel_8086.Console
{
    public interface CommandHandler
    {
        CommandHandler NextHandler { get; set; }
        string HandleOperation(string[] args);
    }
}
