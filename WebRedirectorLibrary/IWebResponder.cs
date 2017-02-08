using System.Net;

namespace WebRedirectorLibrary
{
    public interface IWebResponder
    {
        string Path { get; }
        void ProcessRequest(HttpListenerContext context);
    }
}