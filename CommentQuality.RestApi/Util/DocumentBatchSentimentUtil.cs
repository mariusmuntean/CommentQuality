using CommentQuality.RestApi.Extensions;
using CommentQuality.Shared;
using Google.Cloud.Language.V1;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommentQuality.RestApi.Util
{
    public static class DocumentBatchSentimentUtil
    {
        public static DocumentBatchSentiment FromSentimentBatchResult(SentimentBatchResult sentimentBatchResult)
        {
            var documentSentimentList = sentimentBatchResult
                .Documents
                .Select((resultItem) => new DocumentSentiment
                {
                    Id = resultItem.Id,
                    Score = resultItem.Score ?? -1.0d
                })
                .ToList();

            return new DocumentBatchSentiment
            {
                Documents = documentSentimentList
            };
        }

        // ToDo: reimplement based on the sentence offset - check if reliable
        public static DocumentBatchSentiment FromAnnotatedAnalyzeSentimentResponse(AnalyzeSentimentResponse analyzeSentimentResponse)
        {
            // list of scores for each document
            Dictionary<string, List<float>> documentSentiments = new Dictionary<string, List<float>>();
            var documentId = string.Empty;

            var sentenceEnumerator = analyzeSentimentResponse.Sentences.GetEnumerator();

            while (sentenceEnumerator.MoveNext())
            {
                // If the current sentence is the separator then the next one is the document ID
                // Extract the document ID and continue
                if (sentenceEnumerator.Current.Text.Content.Contains(DocumentBatchExtensions.DocumentSeparator))
                {
                    sentenceEnumerator.MoveNext();
                    documentId = sentenceEnumerator.Current.Text.Content;
                    continue;
                }

                // Id there is no current document ID then there is a problem in the sentences
                if (string.IsNullOrWhiteSpace(documentId))
                {
                    throw new Exception("Sentences are not anotated as expected!");
                }

                // Create a new list of scores for the current document if there is none
                if (!documentSentiments.ContainsKey(documentId))
                {
                    documentSentiments.Add(documentId, new List<float>());
                }

                documentSentiments[documentId].Add(sentenceEnumerator.Current.Sentiment.Score);
            }

            return new DocumentBatchSentiment()
            {
                Documents = documentSentiments.Select(pair => new DocumentSentiment()
                {
                    Id = pair.Key,
                    Score = pair.Value.Average()
                }).ToList()
            };
        }
    }
}