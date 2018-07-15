using CommentQuality.RestApi.Interfaces;
using CommentQuality.RestApi.Models;
using Google.Apis.YouTube.v3.Data;

namespace CommentQuality.RestApi.Services
{
    class DocumentBatchProvider
    {
        private readonly BatchedCommentsProviderConfig _providerConfig;
        private readonly ICommentProvider _commentProvider;

        private bool _initialized = false;

        public DocumentBatchProvider(BatchedCommentsProviderConfig providerConfig, ICommentProvider commentProvider)
        {
            _providerConfig = providerConfig;
            _commentProvider = commentProvider;
        }

        public void Init(string videoId)
        {
            _commentProvider.Init(videoId);
            _initialized = true;
        }

        public DocumentBatch GetNextDocumentBatch()
        {
            AssertInit();

            var docBatch = new DocumentBatch();

            while (docBatch.Documents.Count < _providerConfig.MaxDocumentCountPerbatch
                   && docBatch.TotalDocumentTextLenth < _providerConfig.MaxTotalDocumentTextLengthPerBatch)
            {
                var comment = _commentProvider.GetNextComment();
                if (comment == null)
                {
                    break;
                }

                var doc = GetDocumentFromComment(comment);
                if (_providerConfig.DocumentPredicate.Invoke(doc))
                {
                    docBatch.AddDocument(doc);
                }
            }

            return docBatch;
        }

        private void AssertInit()
        {
            if (!_initialized)
            {
                throw new NotInitializedException("DocumentBatchProvider not initialized");
            }
        }

        private Document GetDocumentFromComment(Comment comment)
        {
            return new Document
            {
                Id = comment?.Id,
                Text = comment?.Snippet.TextOriginal.Length > 5000 ? comment.Snippet.TextOriginal.Substring(0, 5000) : comment?.Snippet.TextOriginal
            };
        }
    }
}