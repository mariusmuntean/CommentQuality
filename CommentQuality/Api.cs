using System;
using CommentQuality.Interfaces;
using CommentQuality.Models;
using CommentQuality.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExecutionContext = Microsoft.Azure.WebJobs.ExecutionContext;

namespace CommentQuality
{
    public static class Api
    {
        [FunctionName("GetComments")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "youtube/comments/{videoId}")]
            HttpRequest req,
            ExecutionContext context,
            TraceWriter log,
            string videoId)
        {
            log.Info($"C# HTTP trigger function processed a request. - {videoId}");

            await foo(context, log, videoId);

            string name = req.Query["name"];

            string requestBody = new StreamReader(req.Body).ReadToEnd();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            return name != null
                ? (ActionResult) new OkObjectResult($"Hello, {name}")
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }

        private static async Task foo(ExecutionContext context, TraceWriter log, string videoId)
        {
            var appSettings = new AppSettings(context);
            IYouTubeDataApi youtubeDataApi = new YouTubeDataApi(appSettings.YouTubeApiSettings);

            var commentThreadIterator = new CommentThreadIterator(youtubeDataApi);
            var commentThreadProvider = new CommentThreadProvider(commentThreadIterator);
            commentThreadProvider.Init(videoId, "snippet,replies");

            var tasks = new List<Task>();
            int commentCount = 0;
            var dateTimeBeforeCounting = DateTime.Now;
            for (int i = 0; i < 50; i++)
            {
                var newTask = Task.Run(() =>
                {
                    var commentIterator = new CommentIterator(youtubeDataApi);
                    var commentProvider2 = new CommentProvider2(commentThreadProvider, commentIterator);

                    var docBatchProvider2 = new DocumentBatchProvider2(
                        new BatchedCommentsProviderConfig(10, 10000,
                            document => !string.IsNullOrWhiteSpace(document.Text)),
                        commentProvider2
                    );

                    DocumentBatch docBatch;
                    while ((docBatch = docBatchProvider2.GetNextDocumentBatch()).Documents.Any())
                    {
                        foreach (var docBatchDocument in docBatch.Documents)
                        {
                            log.Info($"{docBatchDocument.Id} - {docBatchDocument.Text}");
                            Interlocked.Increment(ref commentCount);
                        }

                        log.Info($"Comment count = {commentCount}");
                    }
                });
                tasks.Add(newTask);
            }

            await Task.WhenAll(tasks);
            var countDuration = DateTime.Now.Subtract(dateTimeBeforeCounting);
            log.Info($"Final comment count = {commentCount}, took {countDuration.TotalSeconds} seconds");
        }

        private static void hae(ExecutionContext context, TraceWriter log, string videoId)
        {
            // ToDo: kinda slow for 10k comments. Consider using Durable Functions to split up the work

            var appSettings = new AppSettings(context);
            IYouTubeDataApi youtubeDataApi = new YouTubeDataApi(appSettings.YouTubeApiSettings);

            var commentThreadIterator = new CommentThreadIterator(youtubeDataApi);
            var commentIterator = new CommentIterator(youtubeDataApi);

            var batchedCommentsProvider = new DocumentBatchProvider(
                new BatchedCommentsProviderConfig(10, 10000, document => !string.IsNullOrWhiteSpace(document.Text)),
                new CommentProvider(commentThreadIterator, commentIterator));
            batchedCommentsProvider.Init(videoId);

            DocumentBatch docBatch;
            while ((docBatch = batchedCommentsProvider.GetNextDocumentBatch()).Documents.Any())
            {
                foreach (var docBatchDocument in docBatch.Documents)
                {
                    log.Info($"{docBatchDocument.Id} - {docBatchDocument.Text}");
                }
            }
        }
    }
}