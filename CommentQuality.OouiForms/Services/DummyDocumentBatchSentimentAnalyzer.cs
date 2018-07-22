using System;
using System.Threading.Tasks;
using CommentQuality.OouiForms.Interfaces;
using CommentQuality.OouiForms.Models;

namespace CommentQuality.OouiForms.Services
{
    public class DummyDocumentBatchSentimentAnalyzer : IDocumentBatchSentimentAnalyzer
    {
        public DummyDocumentBatchSentimentAnalyzer()
        {
        }

        public Task<DocumentBatchSentiment> AnalyzeDocumentBatchAsync(DocumentBatch documentBatch)
        {
            var docbatchSentiment = new DocumentBatchSentiment();
            var rnd = new Random();
            foreach (var doc in documentBatch.Documents)
            {
                docbatchSentiment.Documents.Add(new DocumentSentiment()
                {
                    Id = doc.Id,
                    Score = DateTime.Now.Second % 20 == 0 ? 0.5 : rnd.NextDouble()
                });
            }

            return Task.FromResult(docbatchSentiment);
        }
    }
}
