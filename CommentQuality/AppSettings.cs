using System.Collections.Specialized;
using System.Configuration;
using CommenQuality.Models;

namespace CommenQuality
{
    class AppSettings
    {
        private NameValueCollection _appSettings;

        public AppSettings()
        {
            _appSettings = ConfigurationManager.AppSettings;
        }

        public string YouTubeDataApiKey => _appSettings["YTApiKey"];

        public YouTubeApiSettings YouTubeApiSettings => new YouTubeApiSettings
        {
            ApiKey = YouTubeDataApiKey
        };
    }
}
