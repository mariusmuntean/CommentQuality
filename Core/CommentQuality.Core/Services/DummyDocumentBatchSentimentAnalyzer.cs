using CommentQuality.Core.Interfaces;
using CommentQuality.Core.Models;
using System;
using System.Threading.Tasks;

namespace CommentQuality.Core.Services
{
    public class DummyDocumentBatchSentimentAnalyzer : IDocumentBatchSentimentAnalyzer
    {
        public DummyDocumentBatchSentimentAnalyzer()
        {
        }

        public Task<DocumentBatchSentiment> AnalyzeDocumentBatchAsync(DocumentBatch documentBatch)
        {
            var docBatchSentiment = new DocumentBatchSentiment();
            var rnd = new Random();
            foreach (var doc in documentBatch.Documents)
            {
                docBatchSentiment.Documents.Add(new DocumentSentiment()
                {
                    Id = doc.Id,
                    Score = rnd.NextDouble()
                });
            }

            return Task.FromResult(docBatchSentiment);
        }
    }
}