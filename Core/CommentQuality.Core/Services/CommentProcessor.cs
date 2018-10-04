using CommentQuality.Core.Interfaces;
using CommentQuality.Core.Models;
using CommentQuality.Core.Stuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CommentQuality.Core.Services
{
    public class CommentProcessor
    {
        private readonly RestApi _restApi;
        private readonly CommentThreadProvider _commentThreadProvider;
        private readonly IDocumentBatchSentimentAnalyzer _documentBatchSentimentAnalyzer;

        public CommentProcessor(RestApi restApi,
            CommentThreadProvider commentThreadProvider,
            IDocumentBatchSentimentAnalyzer documentBatchSentimentAnalyzer)
        {
            this._restApi = restApi;
            this._commentThreadProvider = commentThreadProvider;
            this._documentBatchSentimentAnalyzer = documentBatchSentimentAnalyzer;
        }

        public async Task ProcessCommentsAsync(string videoId, Action<CommentBatchResult> onCommentBatchResult)
        {
            var totalCommentCount = int.Parse(await _restApi.GetCommentCount(videoId).ConfigureAwait(false));
            var parallelWorkers = Math.Ceiling(totalCommentCount / 100.0d);
            parallelWorkers++;

            parallelWorkers = (int) Math.Min(parallelWorkers, 10);

            _commentThreadProvider.Init(videoId, "snippet,replies");

            var tasks = new List<Task>();
            int commentCount = 0;
            var dateTimeBeforeCounting = DateTime.Now;

            Console.WriteLine($"Analyzing {totalCommentCount} comments with {parallelWorkers} worker(s)");
            for (int i = 0; i < parallelWorkers; i++)
            {
                var newTask = Task.Run(async () =>
                {
                    var commentIterator = new CommentIterator(_restApi);
                    var commentProvider2 = new CommentProvider2(_commentThreadProvider, commentIterator);

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

                        var analysisResult = await _documentBatchSentimentAnalyzer.AnalyzeDocumentBatchAsync(docBatch)
                            .ConfigureAwait(false);

                        onCommentBatchResult.Invoke(new CommentBatchResult()
                        {
                            TotalCommentCount = totalCommentCount,
                            ProcessedCommentCount = commentCount,
                            DocumentBatchSentiment = analysisResult
                        });

                        docBatch = docBatchProvider2.GetNextDocumentBatch();
                    }

                    Console.WriteLine($"Task {Task.CurrentId} ended");
                });
                tasks.Add(newTask);
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
    }
}