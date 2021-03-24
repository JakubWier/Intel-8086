namespace Intel_8086.CommandInterpreter
{
    public interface IProcedureHandling
    {
        IProcedureHandling NextHandler { get; set; }
        string HandleOperation(string[] args);
    }
}
