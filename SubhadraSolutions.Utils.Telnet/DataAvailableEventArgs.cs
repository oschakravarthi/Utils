namespace SubhadraSolutions.Utils.Telnet
{
    public class DataAvailableEventArgs : EventArgs
    {
        public string Data { get; private set; }

        public DataAvailableEventArgs(string output)
        {
            this.Data = output;
        }
    }
}
