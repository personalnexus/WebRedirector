using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using WebRedirectorLibrary.StatisticsLogging;

namespace WebRedirectorLibrary
{
    public class WebServer: IDisposable, IWebServerStatisticsProvider
    {
        private HttpListener _httpListener;
        private Dictionary<string, WebResponderInfo> _respondersByPath;
        private int _invalidRequestCount;

        public void Start(WebServerOptions options)
        {
            if (_httpListener != null)
            {
                throw new InvalidOperationException($"Cannot call {nameof(WebServer.Start)} again when instance has already been started.");
            }

            _respondersByPath = options.Responders.ToDictionary(x => "/" + x.Path, x => new WebResponderInfo(x));

            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add(options.UrlPrefix);
            _httpListener.Start();

            Task.Run(new Action(ProcessRequests));
        }

        private void ProcessRequests()
        {
            // copy to local, because field is nulled when listener is stopped
            HttpListener httpListener = _httpListener;
            if (httpListener != null)
            {
                while (httpListener.IsListening)
                {
                    try
                    {
                        HttpListenerContext context = httpListener.GetContext();
                        ProcessRequest(context);
                    }
                    catch (Exception exception)
                    {
                        // Ignore exceptions if _httpListener has been stopped
                        if (httpListener.IsListening)
                        {
                            OnErrorOccured(new WebServerErrorOccuredEventArgs("An unexpected error occurred while processing requests.", exception));
                        }
                        continue;
                    }
                }
            }
        }

        public WebServerStatistics GetStatistics()
        {
            int invalidRequestCount = Interlocked.Exchange(ref _invalidRequestCount, 0);
            var result = new WebServerStatistics { InvalidRequestCount = invalidRequestCount };
            foreach (WebResponderInfo info in _respondersByPath.Values)
            {
                result.Responders.Add(new WebResponderStatistics(info));
            }
            return result;
        }

        private void ProcessRequest(HttpListenerContext context)
        {
            string path = context.Request.Url.AbsolutePath;
            if (!_respondersByPath.TryGetValue(path, out WebResponderInfo responder))
            {
                Interlocked.Increment(ref _invalidRequestCount);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                context.Response.Close();
            }
            else
            {
                try
                {
                    responder.ProcessRequest(context);
                }
                catch (Exception e)
                {
                    OnErrorOccured(new WebServerErrorOccuredEventArgs($"An error occurred while processing a request for {context.Request.RawUrl}", e));
                }
            }
        }

        protected virtual void OnErrorOccured(WebServerErrorOccuredEventArgs eventArgs)
        {
            ErrorOccured?.Invoke(this, eventArgs);
        }

        public event EventHandler<WebServerErrorOccuredEventArgs> ErrorOccured;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_httpListener != null)
                {
                    _httpListener.Stop();
                    _httpListener.Close();
                    _httpListener = null;
                }
            }
        }
    }
}
