using SubhadraSolutions.Utils.Net.Telnet;

namespace SubhadraSolutions.Utils.Net.Ping
{
    public class PingTelnetHelper
    {
        private readonly string commandPrompt;
        public TelnetHelper TelnetHelper { get; private set; }

        public PingTelnetHelper(TelnetHelper telnetHelper, string commandPrompt)
        {
            TelnetHelper = telnetHelper;
            this.commandPrompt = commandPrompt;
        }

        public decimal PingIpAddress(string ipAddress, IPingResultProcessor resultProcessor, int timeoutInMilliSeconds)
        {
            string pingResult = TelnetHelper.SendCommand("ping " + ipAddress, commandPrompt, timeoutInMilliSeconds);
            return resultProcessor.ProcessPingResult(pingResult, commandPrompt);
        }
    }

}
