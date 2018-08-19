using CommentQuality.OouiForms.Interfaces;
using CommentQuality.OouiForms.Models;
using CommentQuality.OouiForms.Stuff;
using System;
using System.Threading.Tasks;

namespace CommentQuality.OouiForms.Services
{
    public class GoogleCloudLanguageServiceDocumentBatchSentimentAnalyzer : IDocumentBatchSentimentAnalyzer
    {
        private readonly RestApi _restApi;

        public GoogleCloudLanguageServiceDocumentBatchSentimentAnalyzer(RestApi restApi)
        {
            _restApi = restApi;
        }

        public async Task<DocumentBatchSentiment> AnalyzeDocumentBatchAsync(DocumentBatch documentBatch)
        {
            try
            {
                var resonse = await _restApi.GetTextSentimentGoogle(documentBatch);

                return resonse;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{this.GetType().FullName} threw exception: {ex}");
                return new DocumentBatchSentiment();
            }
        }
    }
}