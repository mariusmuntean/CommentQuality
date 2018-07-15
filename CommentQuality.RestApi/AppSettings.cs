using CommentQuality.RestApi.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;

namespace CommentQuality.RestApi
{
    class AppSettings
    {
        private IConfigurationRoot _configurationRoot;

        public AppSettings(ExecutionContext context)
        {
            _configurationRoot = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }

        public string YouTubeDataApiKey => _configurationRoot["YTApiKey"];

        public YouTubeApiSettings YouTubeApiSettings => new YouTubeApiSettings
        {
            ApiKey = YouTubeDataApiKey
        };
    }
}