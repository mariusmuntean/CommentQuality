using CommentQuality.Interfaces;
using CommentQuality.Models;
using Google.Apis.YouTube.v3.Data;

namespace CommentQuality.Services
{
    internal class DocumentBatchProvider2
    {
        private readonly BatchedCommentsProviderConfig _providerConfig;
        private readonly ICommentProvider2 _commentProvider2;

        public DocumentBatchProvider2(BatchedCommentsProviderConfig providerConfig,
            ICommentProvider2 commentProvider2)
        {
            _providerConfig = providerConfig;
            _commentProvider2 = commentProvider2;
        }

        public DocumentBatch GetNextDocumentBatch()
        {
            var docBatch = new DocumentBatch();

            while (docBatch.Documents.Count < _providerConfig.MaxDocumentCountPerbatch
                   && docBatch.TotalDocumentTextLenth < _providerConfig.MaxTotalDocumentTextLengthPerBatch)
            {
                var comment = _commentProvider2.GetNextComment();
                if (comment == null) break;

                var doc = GetDocumentFromComment(comment);
                if (_providerConfig.DocumentPredicate.Invoke(doc)) docBatch.AddDocument(doc);
            }

            return docBatch;
        }

        private Document GetDocumentFromComment(Comment comment)
        {
            return new Document
            {
                Id = comment?.Id,
                Text = comment?.Snippet.TextOriginal.Length > 5000
                    ? comment.Snippet.TextOriginal.Substring(0, 5000)
                    : comment?.Snippet.TextOriginal
            };
        }
    }
}