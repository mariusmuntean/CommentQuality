using System;
using System.Threading.Tasks;
using CommentQuality.OouiForms.Interfaces;
using CommentQuality.OouiForms.Models;
using System.Linq;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using System.Collections.Generic;
using CommentQuality.OouiForms.Stuff;

namespace CommentQuality.OouiForms.Services
{
    public class AzureCognitiveServicesDocumentBatchSentimentAnalyzer : IDocumentBatchSentimentAnalyzer
    {
        private readonly RestApi _restApi;

        public AzureCognitiveServicesDocumentBatchSentimentAnalyzer(RestApi restApi)
        {
            _restApi = restApi;
        }

        public async Task<DocumentBatchSentiment> AnalyzeDocumentBatchAsync(DocumentBatch documentBatch)
        {
            try
            {
                var resonse = await _restApi.GetTextSentimentAzure(documentBatch);

                return CreateDocumentBatchSentimentFromSentimentBatchResult(resonse);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{this.GetType().FullName} threw exception: {ex}");
                return new DocumentBatchSentiment();
            }
        }

        private DocumentBatchSentiment CreateDocumentBatchSentimentFromSentimentBatchResult(SentimentBatchResult sentimentBatchResult)
        {
            var documentSentimentList = sentimentBatchResult
                                .Documents
                                .Select((resultItem) => new DocumentSentiment()
                                {
                                    Id = resultItem.Id,
                                    Score = resultItem.Score ?? -1.0d
                                })
                                .ToList();

            return new DocumentBatchSentiment
            {
                Documents = documentSentimentList ?? new List<DocumentSentiment>()
            };
        }
    }
}
