using System.Collections.Generic;

namespace WebRedirectorLibrary
{
    public class WebServerOptions
    {
        public string UrlPrefix { get; set; }

        public IList<IWebResponder> Responders { get; set; } = new List<IWebResponder>();
    }
}