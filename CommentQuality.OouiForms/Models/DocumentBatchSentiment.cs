using System.Collections.Generic;
namespace CommentQuality.OouiForms.Models
{
    /// <summary>
    /// The sentiment analysis for a DocumentBatch
    /// </summary>
    public class DocumentBatchSentiment
    {
        public DocumentBatchSentiment()
        {
            Documents = new List<DocumentSentiment>();
        }

        public List<DocumentSentiment> Documents
        {
            get;
            set;
        }
    }
}
