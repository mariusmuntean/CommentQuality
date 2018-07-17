namespace CommentQuality.OouiForms.Models
{
    /// <summary>
    /// A piece of text to be submitted to sentiment analysis
    /// </summary>
    internal class Document
    {
        /// <summary>
        /// Simple identifier
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The text of the document
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The language code (ISO 636-1) of the text language.
        /// </summary>
        public string LanguageCode { get; set; }
    }
}