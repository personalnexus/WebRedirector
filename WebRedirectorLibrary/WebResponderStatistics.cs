namespace WebRedirectorLibrary
{
    public class WebResponderStatistics
    {
        public WebResponderStatistics()
        {
        }

        internal WebResponderStatistics(WebResponderInfo responderInfo): this()
        {
            HitCount = responderInfo.GetAndResetHitCount();
            Path = responderInfo.Responder.Path;
        }

        public int HitCount { get; set; }
        public string Path { get; set; }
    }
}