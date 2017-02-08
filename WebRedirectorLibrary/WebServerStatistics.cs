using System;
using System.Collections.Generic;

namespace WebRedirectorLibrary
{
    public class WebServerStatistics
    {
        public WebServerStatistics()
        {
            Timestamp = DateTime.Now;
            Responders = new List<WebResponderStatistics>();
        }

        public int InvalidRequestCount { get; set; }
        public DateTime Timestamp { get; set; }
        public IList<WebResponderStatistics> Responders { get; set; }
    }
}