using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace CommentQuality.OouiForms.Models
{
    /// <summary>
    /// A bunch of comments.
    /// </summary>
    public class DocumentBatch
    {
        public DocumentBatch()
        {
            Documents = new List<Document>();
        }

        /// <summary>
        /// The documents in this batch
        /// </summary>
        [JsonProperty("Documents")]
        public List<Document> Documents { get; set; }

        /// <summary>
        /// Gets the combined length of the text in all documents
        /// </summary>
        [JsonIgnore]
        public int TotalDocumentTextLenth => Documents.Sum(document => document.Text.Length);

        /// <summary>
        /// Adds a <see cref="Document"/> to the current batch
        /// </summary>
        /// <param name="document"></param>
        public void AddDocument(Document document)
        {
            Documents.Add(document);
        }
    }
}