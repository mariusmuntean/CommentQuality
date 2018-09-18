using CommentQuality.Core.Models;
using System.Threading.Tasks;

namespace CommentQuality.Core.Interfaces
{
    /// <summary>
    /// When implemented in a subclass it performs sentiment analysis on a <see cref="DocumentBatch"/>
    /// </summary>
    public interface IDocumentBatchSentimentAnalyzer
    {
        /// <summary>
        /// Analyzes the document batch.
        /// </summary>
        /// <returns>The sentiment analysis for the provided DocumentBatch</returns>
        /// <param name="documentBatch">Document batch.</param>
        Task<DocumentBatchSentiment> AnalyzeDocumentBatchAsync(DocumentBatch documentBatch);
    }
}