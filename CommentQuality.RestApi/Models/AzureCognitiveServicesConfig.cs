namespace CommentQuality.RestApi.Models
{
    public class AzureCognitiveServicesConfig
    {
        /// <summary>
        /// Access key for your subscription.
        /// </summary>
        /// <value>The API key.</value>
        public string ApiKey
        {
            get;
            set;
        }

        /// <summary>
        /// The URL of the endpoint you signed up for.
        /// </summary>
        /// <value>The endpoint URL.</value>
        public string EndpointUrl
        {
            get;
            set;
        }
    }
}
