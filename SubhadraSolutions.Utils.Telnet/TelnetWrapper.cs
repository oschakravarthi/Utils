using System.Net.Sockets;
using System.Net;
using System.Text;

namespace SubhadraSolutions.Utils.Telnet
{
    public class TelnetWrapper : TelnetProtocolHandler
    {
        private class State
        {
            public const int BufferSize = 256;

            public Socket WorkSocket;

            public byte[] Buffer = new byte[256];
        }

        public TelnetWrapper(string host, int port) 
            : this(Dns.GetHostEntry(host), port)
        {

        }
        public TelnetWrapper(IPHostEntry hostEntry, int port)
        {
            this.HostEntry = hostEntry;
            this.Port = port;
        }
        private bool m_a;

        private ManualResetEvent m_b = new ManualResetEvent(initialState: false);

        private ManualResetEvent manualResetEvent = new ManualResetEvent(initialState: false);

        private Socket _socket;

        public int Port { get; private set; }

        public IPHostEntry HostEntry {  get; private set; }

        //public int TerminalWidth
        //{
        //    set
        //    {
        //        windowSize.Width = value;
        //    }
        //}

        //public int TerminalHeight
        //{
        //    set
        //    {
        //        windowSize.Height = value;
        //    }
        //}

        //public string TerminalType
        //{
        //    set
        //    {
        //        terminalType = value;
        //    }
        //}

        public bool Connected => _socket.Connected;

        public event EventHandler Disconnected;
        public event EventHandler<DataAvailableEventArgs> OnDataAvailable;
        
        public void Connect()
        {
            try
            {
                IPAddress address = this.HostEntry.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(address, this.Port);
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.BeginConnect(remoteEP, ConnectCallback, _socket);
                this.m_b.WaitOne();
                Reset();
            }
            catch
            {
                Disconnect();
                throw;
            }
        }

        public void Send(string command)
        {
            try
            {
                byte[] bytes = Encoding.ASCII.GetBytes(command);
                Transpose(bytes);
            }
            catch (Exception innerException)
            {
                Disconnect();
                throw new ApplicationException("Error writing to socket.", innerException);
            }
        }

        public void Receive()
        {
            Receive(_socket);
        }

        public void Disconnect()
        {
            try
            {
                if (_socket != null && _socket.Connected)
                {
                    _socket.Shutdown(SocketShutdown.Both);
                    _socket.Close();
                    this.Disconnected?.Invoke(this, new EventArgs());
                }
            }
            catch
            {
            }
        }

        protected override void Write(byte[] b)
        {
            if (_socket.Connected)
            {
                BeginSend(_socket, b);
            }
            this.manualResetEvent.WaitOne();
        }

        private void ConnectCallback(IAsyncResult asyncresult)
        {
            Socket socket = (Socket)asyncresult.AsyncState;
            socket.EndConnect(asyncresult);
            this.m_b.Set();
        }

        private void Receive(Socket socket)
        {
            State state = new State();
            state.WorkSocket = socket;
            SocketError errorCode = SocketError.Success;
            socket.BeginReceive(state.Buffer, 0, State.BufferSize, SocketFlags.None, out errorCode, ReceiveCallback, state);
        }

        private void ReceiveCallback(IAsyncResult asyncResult)
        {
            State state = (State)asyncResult.AsyncState;
            Socket workSocket = state.WorkSocket;
            if (!this.m_a)
            {
                int num = workSocket.EndReceive(asyncResult);
                if (num > 0)
                {
                    InputFeed(state.Buffer, num);
                    Negotiate(state.Buffer);
                    this.OnDataAvailable?.Invoke(this, new DataAvailableEventArgs(Encoding.ASCII.GetString(state.Buffer, 0, num)));
                    workSocket.BeginReceive(state.Buffer, 0, State.BufferSize, SocketFlags.None, ReceiveCallback, state);
                }
                else
                {
                    Disconnect();
                }
            }
        }

        private void BeginSend(Socket socket, byte[] bytes)
        {
            socket.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, EndSend, socket);
        }

        private void EndSend(IAsyncResult asyncResult)
        {
            Socket socket = (Socket)asyncResult.AsyncState;
            socket.EndSend(asyncResult);
            this.manualResetEvent.Set();
        }

        protected override void SetLocalEcho(bool echo)
        {
        }

        protected override void NotifyEndOfRecord()
        {
        }

        protected override void Dispose(bool disposing)
        {
            Disconnect();
        }
    }
}
