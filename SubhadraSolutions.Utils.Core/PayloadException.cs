using System;

namespace SubhadraSolutions.Utils
{
    public class PayloadException : Exception
    {
        public PayloadException(Exception innerException, object payload) : base(innerException.Message, innerException)
        {
            this.Payload = payload;
        }

        public object Payload { get; private set; }

        public override string ToString()
        {
            var s = base.ToString();
            return $"{s} \nPayload: {Payload}";
        }
    }
}