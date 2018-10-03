using System;
using System.Threading.Tasks;
using CommentQuality.Core.Interfaces;
using CommentQuality.Core.Models;
using CommentQuality.Core.Stuff;

namespace CommentQuality.Core.Services
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
                var resonse = await _restApi.GetTextSentimentGoogle(documentBatch).ConfigureAwait(false);

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