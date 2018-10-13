using System;

namespace WebRedirectorLibrary
{
    public class WebServerErrorOccuredEventArgs: EventArgs
    {
        public WebServerErrorOccuredEventArgs(string errorMessage, Exception exception)
        {
            ErrorMessage = errorMessage;
            Exception = exception;
        }

        public string ErrorMessage { get; }
        public Exception Exception { get; }
    }
}