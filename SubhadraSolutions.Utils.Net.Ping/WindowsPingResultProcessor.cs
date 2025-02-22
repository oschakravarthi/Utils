namespace SubhadraSolutions.Utils.Net.Ping
{
    public class WindowsPingResultProcessor : AbstractPingResultProcessor, IPingResultProcessor
    {
        public decimal ProcessPingResult(string pingOutput, string commandPrompt)
        {
            if (pingOutput != null)
            {
                if (pingOutput.IndexOf("Destination net unreachable") > -1)
                {
                    return 100m;
                }
                int num = pingOutput.IndexOf("% loss)");
                if (num != -1)
                {
                    int num2 = pingOutput.LastIndexOf("(", num);
                    string value = pingOutput.Substring(num2 + 1, num - num2 - 1);
                    return Convert.ToDecimal(value);
                }
            }
            return 100m;
        }
    }

}
