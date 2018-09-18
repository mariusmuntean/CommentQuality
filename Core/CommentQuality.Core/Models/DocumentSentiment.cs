using Newtonsoft.Json;

namespace CommentQuality.Core.Models
{
    public class DocumentSentiment
    {
        /// <summary>
        /// The analyzed document ID
        /// </summary>
        /// <value>The identifier.</value>
        [JsonProperty("id")]
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// A score in the range of 0 (negative) to 1 (positive).
        /// Values close to 0.5 are neutral or indeterminate. A score of 0.5 indicates neutrality.
        /// When a string cannot be analyzed for sentiment or has no sentiment, the score is always 0.5 exactly.
        /// </summary>
        /// <value>The score.</value>
        [JsonProperty("score")]
        public double Score
        {
            get;
            set;
        }
    }
}