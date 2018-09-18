using CommentQuality.Core.Interfaces;
using CommentQuality.Core.Models;
using CommentQuality.Core.Stuff;
using System;
using System.Threading.Tasks;

namespace CommentQuality.Core.Services
{
    public class AzureCognitiveServicesDocumentBatchSentimentAnalyzer : IDocumentBatchSentimentAnalyzer
    {
        private readonly RestApi _restApi;

        public AzureCognitiveServicesDocumentBatchSentimentAnalyzer(RestApi restApi)
        {
            _restApi = restApi;
        }

        public Task<DocumentBatchSentiment> AnalyzeDocumentBatchAsync(DocumentBatch documentBatch)
        {
            try
            {
                return _restApi.GetTextSentimentAzure(documentBatch);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{this.GetType().FullName} threw exception: {ex}");
                return Task.FromResult(new DocumentBatchSentiment());
            }
        }
    }
}