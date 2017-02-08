using System.Net;
using System.Threading;

namespace WebRedirectorLibrary
{
    internal class WebResponderInfo
    {
        public WebResponderInfo(IWebResponder responder)
        {
            Responder = responder;
        }

        public IWebResponder Responder { get; private set; }

        public int HitCount { get { return _hitCount; } }

        private int _hitCount;

        public int GetAndResetHitCount()
        {
            return Interlocked.Exchange(ref _hitCount, 0);
        }

        public void ProcessRequest(HttpListenerContext context)
        {
            Interlocked.Increment(ref _hitCount);
            Responder.ProcessRequest(context);
        }
    }
}