using System.Collections.Generic;
using System.Linq;

namespace CommentQuality.Models
{
    /// <summary>
    /// A bunch of comments.
    /// </summary>
    class DocumentBatch
    {
        private List<Document> _documents = new List<Document>();

        /// <summary>
        /// A read-only copy of the documents in this batch
        /// </summary>
        public List<Document> Documents => new List<Document>(_documents.AsReadOnly());

        /// <summary>
        /// Gets the combined length of the text in all documents
        /// </summary>
        public int TotalDocumentTextLenth => _documents.Sum(document => document.Text.Length);

        /// <summary>
        /// Adds a <see cref="Document"/> to the current batch
        /// </summary>
        /// <param name="document"></param>
        public void AddDocument(Document document)
        {
            _documents.Add(document);
        }
    }
}