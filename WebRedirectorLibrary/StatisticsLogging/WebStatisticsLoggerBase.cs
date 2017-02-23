using System;
using System.Threading;

namespace WebRedirectorLibrary.StatisticsLogging
{
    public abstract class WebStatisticsLoggerBase: IDisposable
    {
        private Timer _timer;
        private IWebServerStatisticsProvider _statisticsProvider;

        protected WebStatisticsLoggerBase(IWebServerStatisticsProvider statisticsProvider, TimeSpan statisticsInterval)
        {
            _statisticsProvider = statisticsProvider;
            int statisticsIntervalInMilliseconds = (int)statisticsInterval.TotalMilliseconds;
            _timer = new Timer(LogStatistics, null, statisticsIntervalInMilliseconds, statisticsIntervalInMilliseconds);
        }

        private void LogStatistics(object state)
        {
            WebServerStatistics statistics = _statisticsProvider.GetStatistics();
            LogStatisticsCore(statistics);
        }

        protected abstract void LogStatisticsCore(WebServerStatistics statistics);

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_timer != null)
                {
                    _timer.Dispose();
                    _timer = null;
                }
            }            
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}