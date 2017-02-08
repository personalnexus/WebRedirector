using System;
using System.Threading;
using WebRedirectorLibrary;

namespace WebRedirectorSample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var options = new WebServerOptions();
            options.UrlPrefix = "http://localhost:12345/";
            options.Responders.Add(new WebRedirectionResponder("msdn", "https://msdn.microsoft.com/en-us/default.aspx"));
            options.Responders.Add(new WebRedirectionResponder("github", "https://github.com/personalnexus"));

            using (var webServer = new WebServer())
            {
                webServer.ErrorOccured += LogError;
                webServer.Start(options);
                
                using (var hitCountTimer = new Timer(LogHitCounts, webServer, 10000, 10000))
                {
                    Console.WriteLine("Web server is running at " + options.UrlPrefix + ". Press any key to stop it.");
                    Console.ReadKey();
                }
            }
        }

        public static void LogHitCounts(object state)
        {
            var webServer = (WebServer)state;
            WebServerStatistics statistics = webServer.GetStatistics();
            Console.WriteLine(string.Format("{0:HH-mm-ss} Web Server Statistics:", statistics.Timestamp));
            foreach (WebResponderStatistics responderStatistics in statistics.Responders)
            {
                Console.WriteLine(string.Format("{0}: {1} hits", responderStatistics.Path, responderStatistics.HitCount));
            }
        }

        public static void LogError(object sender, WebServerErrorOccuredEventArgs eventArgs)
        {
            Console.WriteLine(string.Format("{0:HH-mm-ss} {1}: {2}", DateTime.Now, eventArgs.ErrorMessage, eventArgs.Exception));
        }
    }
}
