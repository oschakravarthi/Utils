using System.Net.Http;

namespace SubhadraSolutions.Utils.Net.Http
{
    public class HttpResponseAndAttemptNumber
    {
        public HttpResponseAndAttemptNumber(HttpResponseMessage response, int attemptNumber)
        {
            Response = response;
            AttemptNumber = attemptNumber;
        }

        public HttpResponseMessage Response { get; private set; }
        public int AttemptNumber { get; private set; }
    }
}