namespace SubhadraSolutions.Utils.Net.Telnet
{
    public class DataAvailableEventArgs : EventArgs
    {
        public string Data { get; private set; }

        public DataAvailableEventArgs(string output)
        {
            Data = output;
        }
    }
}
