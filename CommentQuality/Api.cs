using CommentQuality.Interfaces;
using CommentQuality.Services;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace CommentQuality
{
    public static class Api
    {
        [FunctionName("GetComments")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "youtube/comments/{videoId}")]
            HttpRequest req,
            ExecutionContext context,
            TraceWriter log,
            string videoId)
        {
            log.Info($"C# HTTP trigger function processed a request. - {videoId}");

            hae(context, log);

            string name = req.Query["name"];

            string requestBody = new StreamReader(req.Body).ReadToEnd();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            return name != null
                ? (ActionResult)new OkObjectResult($"Hello, {name}")
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }

        private static void hae(ExecutionContext context, TraceWriter log)
        {
            var appSettings = new AppSettings(context);
            IYouTubeDataApi youtubeDataApi = new YouTubeDataApi(appSettings.YouTubeApiSettings);

            var commentThreadIterator = new CommentThreadIterator(youtubeDataApi);
            var commentIterator = new CommentIterator(youtubeDataApi);

            var commentIdx = 0;
            foreach (var commentThread in commentThreadIterator.GetCommentThreads("snippet,replies", "5iltz8JTRKw"))
            {
                commentIdx++;
                log.Info($"{commentIdx} --- {commentThread.Snippet.TopLevelComment.Snippet.TextOriginal}");
                foreach (var comment in commentIterator.GetComments("snippet", commentThread.Id, CommentsResource.ListRequest.TextFormatEnum.PlainText))
                {
                    commentIdx++;
                    log.Info($"{commentIdx} - {comment.Snippet.TextOriginal}");
                }
                log.Info("\n");
            }
        }
    }
}