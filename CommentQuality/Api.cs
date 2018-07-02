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
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "youtube/comments/{videoId}")]
            HttpRequest req,
            ExecutionContext context,
            TraceWriter log,
            string videoId)
        {
            log.Info($"C# HTTP trigger function processed a request. - {videoId}");

            await hae(context, log);

            string name = req.Query["name"];

            string requestBody = new StreamReader(req.Body).ReadToEnd();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            return name != null
                ? (ActionResult) new OkObjectResult($"Hello, {name}")
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }

        private static async Task hae(ExecutionContext context, TraceWriter log)
        {
            var appSettings = new AppSettings(context);

            var youTubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = appSettings.YouTubeApiSettings.ApiKey,
                ApplicationName = "CommentQuality"
            });

            string nextCommentThreadsBatchToken = null;
            string nextCommentsBatchToken = null;
            var commentIdx = 0;
            do
            {
                var listReq = youTubeService.CommentThreads.List("snippet,replies");
                listReq.VideoId = "5iltz8JTRKw";
                listReq.PageToken = nextCommentThreadsBatchToken;

                var resp = await listReq.ExecuteAsync();
                nextCommentThreadsBatchToken = resp.NextPageToken != nextCommentThreadsBatchToken ? resp.NextPageToken : string.Empty;

                foreach (var commentThread in resp.Items)
                {
                    commentIdx++;
                    log.Info($"{commentIdx} Top Level Comment: {commentThread.Snippet.TopLevelComment.Snippet.TextOriginal}");

                    do
                    {
                        var commentListReq = youTubeService.Comments.List("snippet");
                        commentListReq.ParentId = commentThread.Id;
                        commentListReq.TextFormat = CommentsResource.ListRequest.TextFormatEnum.PlainText;
                        commentListReq.PageToken = nextCommentsBatchToken;

                        var comments = await commentListReq.ExecuteAsync();
                        nextCommentsBatchToken = comments.NextPageToken != nextCommentThreadsBatchToken ? comments.NextPageToken : null;

                        foreach (var commentsItem in comments.Items)
                        {
                            commentIdx++;
                            log.Info($"{commentIdx}     {commentsItem.Snippet.TextOriginal}");
                        }
                    } while (nextCommentsBatchToken != null);

                    log.Info("");
                }
            } while (nextCommentThreadsBatchToken != null);
        }
    }
}