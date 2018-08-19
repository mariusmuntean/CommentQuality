﻿using CommentQuality.OouiForms.Interfaces;
using CommentQuality.OouiForms.Models;
using CommentQuality.OouiForms.Stuff;
using System;
using System.Threading.Tasks;

namespace CommentQuality.OouiForms.Services
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