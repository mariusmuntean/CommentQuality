using CommentQuality.RestApi.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;

namespace CommentQuality.RestApi
{
    class AppSettings
    {
        private readonly IConfigurationRoot _configurationRoot;

        public AppSettings(ExecutionContext context)
        {
            _configurationRoot = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }

        private string YouTubeDataApiKey => _configurationRoot["YTApiKey"];

        private string AzureCognitiveServicesApiKey => _configurationRoot["AzureCognitiveServicesApiKey"];
        private string AzureCognitiveServicesEndpointUrl => _configurationRoot["AzureCognitiveServicesEndpointUrl"];

        private string GoogleCredentials_Type => _configurationRoot["type"];
        private string GoogleCredentials_ProjectId => _configurationRoot["project_id"];
        private string GoogleCredentials_PrivateKeyId => _configurationRoot["private_key_id"];
        private string GoogleCredentials_PrivateKey => _configurationRoot["private_key"];
        private string GoogleCredentials_ClientEmail => _configurationRoot["client_email"];
        private string GoogleCredentials_ClientId => _configurationRoot["client_id"];
        private string GoogleCredentials_AuthUri => _configurationRoot["auth_uri"];
        private string GoogleCredentials_TokenUri => _configurationRoot["token_uri"];
        private string GoogleCredentials_AuthProviderCertUrl => _configurationRoot["auth_provider_x509_cert_url"];
        private string GoogleCredentials_ClientCertUrl => _configurationRoot["client_x509_cert_url"];

        public YouTubeApiSettings YouTubeApiSettings => new YouTubeApiSettings
        {
            ApiKey = YouTubeDataApiKey
        };

        public AzureCognitiveServicesConfig AzureCognitiveServicesConfig => new AzureCognitiveServicesConfig
        {
            ApiKey = AzureCognitiveServicesApiKey,
            EndpointUrl = AzureCognitiveServicesEndpointUrl
        };

        public GoogleCredentialsData GoogleCredentialsData => new GoogleCredentialsData()
        {
            Type = GoogleCredentials_Type,
            ProjectId = GoogleCredentials_ProjectId,
            PrivateKeyId = GoogleCredentials_PrivateKeyId,
            PrivateKey = GoogleCredentials_PrivateKey,
            ClientEmail = GoogleCredentials_ClientEmail,
            ClientId = GoogleCredentials_ClientId,
            AuthUri = GoogleCredentials_AuthUri,
            TokenUri = GoogleCredentials_TokenUri,
            AuthProviderCertUrl = GoogleCredentials_AuthProviderCertUrl,
            ClientCertUrl = GoogleCredentials_ClientCertUrl
        };
    }
}