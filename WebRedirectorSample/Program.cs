using System;
using System.Diagnostics;
using System.IO;
using WebRedirectorLibrary;
using WebRedirectorLibrary.StatisticsLogging;

namespace WebRedirectorSample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var options = new WebServerOptions();
            options.UrlPrefix = "http://localhost:12345/";
            options.AddRedirect("msdn", "https://msdn.microsoft.com/en-us/default.aspx");
            options.AddRedirect("github", "https://github.com/personalnexus");

            string logFileName = Path.GetTempFileName();

            using (var webServer = new WebServer())
            {
                webServer.ErrorOccured += LogError;
                webServer.Start(options);

                using (var logger = new WebStatisticsCsvFileLogger(logFileName, webServer, TimeSpan.FromSeconds(10)))
                {
                    Console.WriteLine($"{DateTime.Now:HH-mm-ss}: Web server is running at '{options.UrlPrefix}'. Press any key to stop it.");
                    Console.ReadKey();
                }
            }

            Process.Start("notepad", logFileName);
        }

        public static void LogError(object sender, WebServerErrorOccuredEventArgs eventArgs)
        {
            Console.WriteLine($"{DateTime.Now:HH-mm-ss} {eventArgs.ErrorMessage}: {eventArgs.Exception}");
        }
    }
}
