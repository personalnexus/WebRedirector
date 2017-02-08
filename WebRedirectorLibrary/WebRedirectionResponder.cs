using System.Net;

namespace WebRedirectorLibrary
{
    public class WebRedirectionResponder : IWebResponder
    {
        public WebRedirectionResponder(string path, string redirectTarget)
        {
            Path = path;
            RedirectTarget = redirectTarget;
        }

        public string Path { get; set; }

        public string RedirectTarget { get; set; }

        public void ProcessRequest(HttpListenerContext context)
        {
            context.Response.Redirect(RedirectTarget);
            context.Response.Close();
        }
    }
}
