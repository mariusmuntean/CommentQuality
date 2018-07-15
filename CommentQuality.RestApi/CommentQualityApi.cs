using System.Linq;
using System.Threading.Tasks;
using CommentQuality.RestApi.Extensions;
using CommentQuality.RestApi.Interfaces;
using CommentQuality.RestApi.Services;
using Google.Apis.YouTube.v3;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using ExecutionContext = Microsoft.Azure.WebJobs.ExecutionContext;

namespace CommentQuality.RestApi
{
    public static class CommentQualityApi
    {
        [FunctionName("GetCommentCount")]
        public static async Task<IActionResult> GetCommentCount(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "video/{videoId}/commentCount")]
            HttpRequest request,
            ExecutionContext context,
            TraceWriter traceWriter,
            string videoId)
        {
            var appSettings = new AppSettings(context);
            IYouTubeDataApi youtubeDataApi = new YouTubeDataApi(appSettings.YouTubeApiSettings);

            var vlr = await youtubeDataApi.GetVideoCommentCount(videoId).ExecuteAsync();

            return new OkObjectResult(vlr.Items.FirstOrDefault()?.Statistics.CommentCount);
        }

        [FunctionName("GetCommentThreads")]
        public static async Task<IActionResult> GetCommentThreads(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "video/{videoId}/commentThreads")]
            HttpRequest request,
            ExecutionContext context,
            TraceWriter traceWriter,
            string videoId)
        {
            var appSettings = new AppSettings(context);
            IYouTubeDataApi youtubeDataApi = new YouTubeDataApi(appSettings.YouTubeApiSettings);

            var part = request.Query.GetQueryParamValue("part");
            var pageToken = request.Query.GetQueryParamValue("pageToken");
            var commentThreadListRequest = youtubeDataApi.GetCommentThreadListRequest(part, videoId, pageToken);
            var commentThreads = await commentThreadListRequest.ExecuteAsync();
            return new OkObjectResult(commentThreads);
        }

        [FunctionName("GetCommentsFromThread")]
        public static async Task<IActionResult> GetCommentsFromThread(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "video/{parentId}/comments")]
            HttpRequest request,
            ExecutionContext context,
            TraceWriter traceWriter,
            string parentId)
        {
            var appsettings = new AppSettings(context);
            IYouTubeDataApi youTubeDataApi = new YouTubeDataApi(appsettings.YouTubeApiSettings);

            var part = request.Query.GetQueryParamValue("part");
            var pageToken = request.Query.GetQueryParamValue("pageToken");
            var textFormatStr = request.Query.GetQueryParamValue("textFormat");
            CommentsResource.ListRequest.TextFormatEnum textFormat =
                CommentsResource.ListRequest.TextFormatEnum.PlainText;
            CommentThreadsResource.ListRequest.TextFormatEnum.TryParse(textFormatStr, out textFormat);

            var commentsListRequest = youTubeDataApi.GetCommentListRequest(part, parentId, textFormat, pageToken);
            var comments = await commentsListRequest.ExecuteAsync();

            return new OkObjectResult(comments);
        }
    }
}