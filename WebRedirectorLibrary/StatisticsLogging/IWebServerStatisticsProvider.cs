namespace WebRedirectorLibrary.StatisticsLogging
{
    public interface IWebServerStatisticsProvider
    {
        WebServerStatistics GetStatistics();
    }
}
