namespace Intel_8086.Console
{
    public interface ProcedureHandler
    {
        ProcedureHandler NextHandler { get; set; }
        string HandleOperation(string[] args);
    }
}
