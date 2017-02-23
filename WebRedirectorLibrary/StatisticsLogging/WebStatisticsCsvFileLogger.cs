﻿using System;
using System.Collections.Generic;
using System.IO;

namespace WebRedirectorLibrary.StatisticsLogging
{
    public class WebStatisticsCsvFileLogger : WebStatisticsLoggerBase
    {
        public WebStatisticsCsvFileLogger(string csvFileNamePattern, IWebServerStatisticsProvider statisticsProvider, TimeSpan statisticsInterval) : base(statisticsProvider, statisticsInterval)
        {
            _csvFileNamePattern = csvFileNamePattern;
        }

        private readonly string _csvFileNamePattern;

        protected override void LogStatisticsCore(WebServerStatistics statistics)
        {
            string fileName = string.Format(_csvFileNamePattern, DateTime.Now);
            File.AppendAllLines(fileName, StatisticsToCsv(statistics));
        }

        private IEnumerable<string> StatisticsToCsv(WebServerStatistics statistics)
        {
            if (statistics.InvalidRequestCount != 0)
            {
                yield return GetCsvLine("#InvalidRequests", statistics.InvalidRequestCount);
            }
            foreach (WebResponderStatistics responder in statistics.Responders)
            {
                yield return GetCsvLine(responder.Path, responder.HitCount);
            }
        }

        private string GetCsvLine(string key, object value)
        {
            string line = string.Format("{0:yyyy-MM-dd HH-mm-ss};{1};{2}", DateTime.Now, key, value);
            return line;
        }
    }
}
