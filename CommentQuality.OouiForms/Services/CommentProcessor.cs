using System;
using CommentQuality.OouiForms.Models;
using CommentQuality.OouiForms.Stuff;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using CommentQuality.OouiForms.Interfaces;

namespace CommentQuality.OouiForms.Services
{
    public class CommentProcessor
    {
        private readonly RestApi restApi;
        private readonly CommentThreadProvider commentThreadProvider;
        private readonly IDocumentBatchSentimentAnalyzer documentBatchSentimentAnalyzer;

        public CommentProcessor(RestApi restApi,
                                CommentThreadProvider commentThreadProvider,
                                IDocumentBatchSentimentAnalyzer documentBatchSentimentAnalyzer)
        {
            this.restApi = restApi;
            this.commentThreadProvider = commentThreadProvider;
            this.documentBatchSentimentAnalyzer = documentBatchSentimentAnalyzer;
        }

        public async Task ProcessCommentsAsync(string videoId, Action<CommentBatchResult> onCommentBatchResult)
        {
            var totalCommentCount = int.Parse(await restApi.GetCommentCount(videoId));
            var parallelWorkers = Math.Ceiling(totalCommentCount / 100.0d);

            commentThreadProvider.Init(videoId, "snippet,replies");

            var tasks = new List<Task>();
            int commentCount = 0;
            var dateTimeBeforeCounting = DateTime.Now;

            Console.WriteLine($"Analyzing {totalCommentCount} comments with {parallelWorkers} worker(s)");
            for (int i = 0; i < parallelWorkers; i++)
            {
                var newTask = Task.Run(async () =>
                {
                    var commentIterator = new CommentIterator(restApi);
                    var commentProvider2 = new CommentProvider2(commentThreadProvider, commentIterator);

                    var docBatchProvider2 = new DocumentBatchProvider2(
                        new BatchedCommentsProviderConfig(20, 10000,
                            document => !string.IsNullOrWhiteSpace(document.Text)),
                        commentProvider2
                    );

                    DocumentBatch docBatch = docBatchProvider2.GetNextDocumentBatch();
                    while (docBatch.Documents.Any())
                    {
                        Interlocked.Add(ref commentCount, docBatch.Documents.Count);

                        // ToDo: add intermediate step to detect the Text Language, for now assume it is EN
                        docBatch.Documents.ForEach((doc) => doc.LanguageCode = "en");

                        var analysisResult = await documentBatchSentimentAnalyzer.AnalyzeDocumentBatchAsync(docBatch);

                        onCommentBatchResult.Invoke(new CommentBatchResult()
                        {
                            TotalCommentCount = totalCommentCount,
                            ProcessedCommentCount = commentCount,
                            DocumentBatchSentiment = analysisResult
                        });

                        docBatch = docBatchProvider2.GetNextDocumentBatch();
                    }
                    Console.WriteLine($"{Task.CurrentId} ended");
                });
                tasks.Add(newTask);
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

    }
}
