using Newtonsoft.Json;

namespace CommentQuality.RestApi.Models
{
    /// <summary>
    /// A piece of text to be submitted to sentiment analysis
    /// </summary>
    public class Document
    {
        /// <summary>
        /// Simple identifier
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// The text of the document
        /// </summary>
        [JsonProperty("text")]
        public string  Text { get; set; }

        /// <summary>
        /// The language code (ISO 636-1) of the text language.
        /// </summary>
        [JsonProperty("languageCode")]
        public string LanguageCode { get; set; }
    }
}
