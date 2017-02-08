using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace WebRedirectorLibrary
{
    public class WebServer: IDisposable
    {
        private HttpListener _httpListener;
        private Dictionary<string, WebResponderInfo> _respondersByPath;
        private int _invalidRequestCount;

        public void Start(WebServerOptions options)
        {
            if (_httpListener != null)
            {
                throw new InvalidOperationException(string.Format("Cannot call {0} when instance has already been started.", nameof(WebServer.Start)));
            }

            _respondersByPath = options.Responders.ToDictionary(x => "/" + x.Path, x => new WebResponderInfo(x));

            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add(options.UrlPrefix);
            _httpListener.Start();

            Task.Run(new Action(ProcessRequests));
        }

        private void ProcessRequests()
        {
            while (_httpListener.IsListening)
            {
                try
                {
                    HttpListenerContext context = _httpListener.GetContext();
                    ProcessRequest(context);
                }
                catch (Exception exception)
                {
                    // Ignore exceptions if _httpListener has been stopped
                    if (_httpListener.IsListening)
                    {   
                        OnErrorOccured(new WebServerErrorOccuredEventArgs("An unexpected error occurred while processing requests.", exception));
                    }
                    continue;
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
            WebResponderInfo responder;
            string path = context.Request.Url.AbsolutePath;
            if (!_respondersByPath.TryGetValue(path, out responder))
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
                    OnErrorOccured(new WebServerErrorOccuredEventArgs(string.Format("An error occurred while processing a request for {0}", context.Request.RawUrl), e));
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
