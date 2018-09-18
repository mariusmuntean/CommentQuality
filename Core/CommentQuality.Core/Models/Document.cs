using Newtonsoft.Json;

namespace CommentQuality.Core.Models
{
    /// <summary>
    /// A piece of text to be submitted for sentiment analysis
    /// </summary>
    public class Document
    {
        /// <summary>
        /// Simple identifier
        /// </summary>
        [JsonProperty("Id")]
        public string Id { get; set; }

        /// <summary>
        /// The text of the document
        /// </summary>
        [JsonProperty("Text")]
        public string Text { get; set; }

        /// <summary>
        /// The language code (ISO 636-1) of the text language.
        /// </summary>
        [JsonProperty("LanguageCode")]
        public string LanguageCode { get; set; }
    }
}