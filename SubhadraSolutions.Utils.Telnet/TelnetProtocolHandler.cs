using SubhadraSolutions.Utils.Abstractions;
using System.Drawing;
using System.Text;

namespace SubhadraSolutions.Utils.Telnet
{
    public abstract class TelnetProtocolHandler : AbstractDisposable
    {
        private const byte m_a = 0;

        private const byte b = 1;

        private const byte c = 2;

        private const byte d = 3;

        private const byte e = 4;

        private const byte f = 5;

        private const byte g = 6;

        private const byte h = 7;

        private const byte i = 8;

        private const byte j = 9;

        private const byte k = byte.MaxValue;

        private const byte l = 239;

        private const byte m = 251;

        private const byte n = 252;

        private const byte o = 253;

        private const byte p = 254;

        private const byte q = 250;

        private const byte r = 240;

        private const byte s = 0;

        private const byte t = 1;

        private const byte u = 3;

        private const byte v = 25;

        private const byte w = 31;

        private const byte x = 24;

        private byte[] y = new byte[0];

        private byte[] z = new byte[2];

        private byte[] aa = new byte[2];

        protected string terminalType = "dumb";

        protected Size windowSize = Size.Empty;

        private byte ab;

        private byte ac;

        private static byte[] ad = new byte[2] { 255, 251 };

        private static byte[] ae = new byte[2] { 255, 252 };

        private static byte[] af = new byte[2] { 255, 253 };

        private static byte[] ag = new byte[2] { 255, 254 };

        private static byte[] ah = new byte[2] { 255, 250 };

        private static byte[] ai = new byte[2] { 255, 240 };

        private static byte aj = 0;

        private static byte ak = 1;

        private byte[] al;

        private byte[] am;

        private byte[] an;

        private byte[] ao;

        public string CRLF
        {
            get
            {
                return Encoding.ASCII.GetString(z);
            }
            set
            {
                z = Encoding.ASCII.GetBytes(value);
            }
        }

        public string CR
        {
            get
            {
                return Encoding.ASCII.GetString(aa);
            }
            set
            {
                aa = Encoding.ASCII.GetBytes(value);
            }
        }

        protected abstract void SetLocalEcho(bool echo);

        protected abstract void NotifyEndOfRecord();

        protected abstract void Write(byte[] b);

        private void a(byte A_0)
        {
            Write(new byte[1] { A_0 });
        }

        protected void Reset()
        {
            ab = 0;
            al = new byte[256];
            an = new byte[256];
            am = new byte[256];
            ao = new byte[256];
        }

        public TelnetProtocolHandler()
        {
            Reset();
            z[0] = 13;
            z[1] = 10;
            aa[0] = 13;
            aa[1] = 0;
        }

        protected void SendTelnetControl(byte code)
        {
            Write(new byte[2] { 255, code });
        }

        private void a(byte A_0, byte[] A_1, int A_2)
        {
            byte b = A_0;
            if (b == 24 && A_2 > 0 && A_1[0] == ak)
            {
                Write(ah);
                a(24);
                a(aj);
                Write(Encoding.ASCII.GetBytes(terminalType));
                Write(ai);
            }
        }

        protected void Transpose(byte[] buf)
        {
            int num = 0;
            byte[] array = new byte[buf.Length * 2];
            byte[] array2;
            for (int i = 0; i < buf.Length; i++)
            {
                switch (buf[i])
                {
                    case byte.MaxValue:
                        array[num++] = byte.MaxValue;
                        array[num++] = byte.MaxValue;
                        break;
                    case 10:
                        if (al[128] != 253)
                        {
                            while (array.Length - num < z.Length)
                            {
                                array2 = new byte[array.Length * 2];
                                Array.Copy(array, 0, array2, 0, num);
                                array = array2;
                            }
                            for (int k = 0; k < z.Length; k++)
                            {
                                array[num++] = z[k];
                            }
                        }
                        else
                        {
                            array[num++] = buf[i];
                        }
                        break;
                    case 13:
                        if (al[128] != 253)
                        {
                            while (array.Length - num < aa.Length)
                            {
                                array2 = new byte[array.Length * 2];
                                Array.Copy(array, 0, array2, 0, num);
                                array = array2;
                            }
                            for (int j = 0; j < aa.Length; j++)
                            {
                                array[num++] = aa[j];
                            }
                        }
                        else
                        {
                            array[num++] = buf[i];
                        }
                        break;
                    default:
                        array[num++] = buf[i];
                        break;
                }
            }
            array2 = new byte[num];
            Array.Copy(array, 0, array2, 0, num);
            Write(array2);
        }

        protected int Negotiate(byte[] nbuf)
        {
            int num = y.Length;
            if (num == 0)
            {
                return -1;
            }
            byte[] array = new byte[3];
            byte[] array2 = new byte[y.Length];
            byte[] array3 = y;
            int a_ = 0;
            int num2 = 0;
            int num3 = 0;
            bool flag = false;
            bool flag2 = false;
            while (!flag && num2 < num && num3 < nbuf.Length)
            {
                byte b = array3[num2++];
                if (b >= 128)
                {
                    b = (byte)(b - 256);
                }
                switch (ab)
                {
                    case 0:
                        if (b == byte.MaxValue)
                        {
                            ab = 1;
                        }
                        else
                        {
                            nbuf[num3++] = b;
                        }
                        break;
                    case 1:
                        switch (b)
                        {
                            case byte.MaxValue:
                                ab = 0;
                                nbuf[num3++] = byte.MaxValue;
                                break;
                            case 251:
                                ab = 3;
                                break;
                            case 252:
                                ab = 5;
                                break;
                            case 254:
                                ab = 6;
                                break;
                            case 253:
                                ab = 4;
                                break;
                            case 239:
                                NotifyEndOfRecord();
                                ab = 0;
                                break;
                            case 250:
                                ab = 2;
                                a_ = 0;
                                break;
                            default:
                                ab = 0;
                                break;
                        }
                        break;
                    case 3:
                        {
                            byte b3;
                            switch (b)
                            {
                                case 1:
                                    b3 = 253;
                                    SetLocalEcho(echo: false);
                                    break;
                                case 3:
                                    b3 = 253;
                                    break;
                                case 25:
                                    b3 = 253;
                                    break;
                                case 0:
                                    b3 = 253;
                                    break;
                                default:
                                    b3 = 254;
                                    break;
                            }
                            if (b3 != an[b + 128] || 251 != am[b + 128])
                            {
                                array[0] = byte.MaxValue;
                                array[1] = b3;
                                array[2] = b;
                                Write(array);
                                an[b + 128] = b3;
                                am[b + 128] = 251;
                            }
                            ab = 0;
                            break;
                        }
                    case 5:
                        {
                            byte b3;
                            switch (b)
                            {
                                case 1:
                                    SetLocalEcho(echo: true);
                                    b3 = 254;
                                    break;
                                case 3:
                                    b3 = 254;
                                    break;
                                case 25:
                                    b3 = 254;
                                    break;
                                case 0:
                                    b3 = 254;
                                    break;
                                default:
                                    b3 = 254;
                                    break;
                            }
                            if (b3 != an[b + 128] || 252 != am[b + 128])
                            {
                                array[0] = byte.MaxValue;
                                array[1] = b3;
                                array[2] = b;
                                Write(array);
                                an[b + 128] = b3;
                                am[b + 128] = 251;
                            }
                            ab = 0;
                            break;
                        }
                    case 4:
                        {
                            byte b3;
                            switch (b)
                            {
                                case 1:
                                    b3 = 251;
                                    SetLocalEcho(echo: true);
                                    break;
                                case 3:
                                    b3 = 251;
                                    break;
                                case 24:
                                    b3 = 251;
                                    break;
                                case 0:
                                    b3 = 251;
                                    break;
                                case 31:
                                    {
                                        Size size = windowSize;
                                        al[b] = 253;
                                        if (size.GetType() != typeof(Size))
                                        {
                                            a(byte.MaxValue);
                                            a(252);
                                            a(31);
                                            b3 = 252;
                                            ao[b] = 252;
                                            break;
                                        }
                                        b3 = 251;
                                        ao[b] = 251;
                                        array[0] = byte.MaxValue;
                                        array[1] = 251;
                                        array[2] = 31;
                                        Write(array);
                                        a(byte.MaxValue);
                                        a(250);
                                        a(31);
                                        a((byte)(size.Width >> 8));
                                        a((byte)(size.Width & 0xFF));
                                        a((byte)(size.Height >> 8));
                                        a((byte)(size.Height & 0xFF));
                                        a(byte.MaxValue);
                                        a(240);
                                        break;
                                    }
                                default:
                                    b3 = 252;
                                    break;
                            }
                            if (b3 != ao[128 + b] || 253 != al[128 + b])
                            {
                                array[0] = byte.MaxValue;
                                array[1] = b3;
                                array[2] = b;
                                Write(array);
                                ao[b + 128] = b3;
                                al[b + 128] = 253;
                            }
                            ab = 0;
                            break;
                        }
                    case 6:
                        {
                            byte b3;
                            switch (b)
                            {
                                case 1:
                                    b3 = 252;
                                    SetLocalEcho(echo: false);
                                    break;
                                case 3:
                                    b3 = 252;
                                    break;
                                case 31:
                                    b3 = 252;
                                    break;
                                case 0:
                                    b3 = 252;
                                    break;
                                default:
                                    b3 = 252;
                                    break;
                            }
                            if (b3 != ao[b + 128] || 254 != al[b + 128])
                            {
                                array[0] = byte.MaxValue;
                                array[1] = b3;
                                array[2] = b;
                                Write(array);
                                ao[b + 128] = b3;
                                al[b + 128] = 254;
                            }
                            ab = 0;
                            break;
                        }
                    case 7:
                        {
                            for (int j = num2; j < y.Length; j++)
                            {
                                if (y[j] == 240)
                                {
                                    flag2 = true;
                                }
                            }
                            if (!flag2)
                            {
                                num2--;
                                flag = true;
                                break;
                            }
                            flag2 = false;
                            if (b == byte.MaxValue)
                            {
                                a_ = 0;
                                ac = b;
                                ab = 8;
                            }
                            else
                            {
                                ab = 0;
                            }
                            break;
                        }
                    case 2:
                        {
                            for (int k = num2; k < y.Length; k++)
                            {
                                if (y[k] == 240)
                                {
                                    flag2 = true;
                                }
                            }
                            if (!flag2)
                            {
                                num2--;
                                flag = true;
                                break;
                            }
                            flag2 = false;
                            byte b4 = b;
                            if (b4 == byte.MaxValue)
                            {
                                ab = 7;
                                break;
                            }
                            ac = b;
                            a_ = 0;
                            ab = 8;
                            break;
                        }
                    case 8:
                        {
                            for (int i = num2; i < y.Length; i++)
                            {
                                if (y[i] == 240)
                                {
                                    flag2 = true;
                                }
                            }
                            if (!flag2)
                            {
                                num2--;
                                flag = true;
                                break;
                            }
                            flag2 = false;
                            byte b2 = b;
                            if (b2 == byte.MaxValue)
                            {
                                ab = 9;
                            }
                            else
                            {
                                array2[a_++] = b;
                            }
                            break;
                        }
                    case 9:
                        switch (b)
                        {
                            case byte.MaxValue:
                                ab = 8;
                                array2[a_++] = byte.MaxValue;
                                break;
                            case 240:
                                a(ac, array2, a_);
                                ac = 0;
                                ab = 0;
                                break;
                            case 250:
                                a(ac, array2, a_);
                                ab = 2;
                                break;
                            default:
                                ab = 0;
                                break;
                        }
                        break;
                    default:
                        ab = 0;
                        break;
                }
            }
            byte[] destinationArray = new byte[num - num2];
            Array.Copy(y, num2, destinationArray, 0, num - num2);
            y = destinationArray;
            return num3;
        }

        protected void InputFeed(byte[] b, int len)
        {
            byte[] destinationArray = new byte[y.Length + len];
            Array.Copy(y, 0, destinationArray, 0, y.Length);
            Array.Copy(b, 0, destinationArray, y.Length, len);
            y = destinationArray;
        }
    }

}
