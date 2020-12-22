using System.Net.Http;

namespace TwitterStub.Core.Exceptions
{
    public class RateExceededException : HttpRequestException
    {
        public RateExceededException() : base()
        {

        }
    }
}
