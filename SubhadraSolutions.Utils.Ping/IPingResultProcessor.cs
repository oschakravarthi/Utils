namespace SubhadraSolutions.Utils.Ping
{
    public interface IPingResultProcessor
    {
        decimal ProcessPingResult(string pingResult, string commandPrompt);

        string RemovePadding(string output, string commandPrompt);
    }

}
