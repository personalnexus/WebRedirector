using System.Collections.Generic;

namespace WebRedirectorLibrary
{
    public class WebServerOptions
    {
        public string UrlPrefix { get; set; }

        public IList<IWebResponder> Responders { get; } = new List<IWebResponder>();

        public WebRedirectionResponder AddRedirect(string path, string redirectTarget)
        {
            var result = new WebRedirectionResponder(path, redirectTarget);
            Responders.Add(result);
            return result;
        }
    }
}