namespace SubhadraSolutions.Utils.Ping
{
    public class UnixPingResultProcessor : AbstractPingResultProcessor, IPingResultProcessor
    {
        public decimal ProcessPingResult(string pingOutput, string commandPrompt)
        {
            if (pingOutput != null)
            {
                int num = pingOutput.IndexOf("% packet loss");
                if (num > -1)
                {
                    int num2 = num - 1;
                    while (pingOutput[num2] != ' ')
                    {
                        num2--;
                    }
                    num2++;
                    string value = pingOutput.Substring(num2, num - num2);
                    return Convert.ToDecimal(value);
                }
            }
            return 100m;
        }
    }

}
