using SubhadraSolutions.Utils.Diagnostics;

namespace SubhadraSolutions.Utils.Telnet
{

    public class TelnetHelper
    {
        private TelnetWrapper m_a;

        private volatile string receivedData;

        private volatile bool c;

        private volatile string d;

        internal TelnetHelper(TelnetWrapper telnetWrapper)
        {
            this.m_a = telnetWrapper;
            this.m_a.OnDataAvailable += OnDataAvailable;
        }

        public void WaitFor(string expectedString)
        {
            receivedData = null;
            c = false;
            d = expectedString;
            this.m_a.Receive();
            if (!string.IsNullOrEmpty(expectedString))
            {
                while (!c)
                {
                    Thread.Yield();
                }
            }
            receivedData = null;
            d = null;
        }

        public string SendCommand(string command, string expectedString, int timeoutInMilliSeconds)
        {
            receivedData = null;
            c = false;
            d = expectedString;
            this.m_a.Send(command + this.m_a.CRLF);
            this.m_a.Receive();
            if (!string.IsNullOrEmpty(expectedString))
            { 
                var elapsedMilliseconds = SharedStopwatch.Elapsed.TotalMilliseconds;
                while (!c && (timeoutInMilliSeconds <= 0 || SharedStopwatch.Elapsed.TotalMilliseconds - elapsedMilliseconds <= timeoutInMilliSeconds))
                {
                    Thread.Yield();
                }
            }
            string result = receivedData;
            receivedData = null;
            d = null;
            return result;
        }

        private void OnDataAvailable(object sender, DataAvailableEventArgs args)
        {
            receivedData += args.Data;
            if (!string.IsNullOrEmpty(d) && (args.Data.Contains(d) || (receivedData != null && receivedData.Contains(d))))
            {
                c = true;
            }
        }

        public static TelnetHelper CreateTelnetSession(string hostName, int port, LoginInfo loginInfo)
        {
            TelnetWrapper telnetWrapper = new TelnetWrapper(hostName, port);
            //telnetWrapper.Disconnected += disconnectedEventHandler;
            TelnetHelper telnetHelper = new TelnetHelper(telnetWrapper);
            telnetWrapper.Connect();
            telnetHelper.WaitFor(loginInfo.LoginPrompt);
            telnetHelper.SendCommand(loginInfo.Login, loginInfo.PasswordPrompt, 0);
            telnetHelper.SendCommand(loginInfo.Password, loginInfo.CommandPrompt, 0);
            return telnetHelper;
        }

        public void Disconnect()
        {
            this.m_a.Disconnect();
        }
    }

}
