using SubhadraSolutions.Utils.Telnet;

namespace SubhadraSolutions.Utils.Ping
{
    public class PingTelnetHelper
    {
        private readonly string commandPrompt;
        public TelnetHelper TelnetHelper { get; private set; }

        public PingTelnetHelper(TelnetHelper telnetHelper, string commandPrompt)
        {
            this.TelnetHelper = telnetHelper;
            this.commandPrompt = commandPrompt;
        }

        public decimal PingIpAddress(string ipAddress, IPingResultProcessor resultProcessor, int timeoutInMilliSeconds)
        {
            string pingResult = this.TelnetHelper.SendCommand("ping " + ipAddress, this.commandPrompt, timeoutInMilliSeconds);
            return resultProcessor.ProcessPingResult(pingResult, this.commandPrompt);
        }
    }

}
