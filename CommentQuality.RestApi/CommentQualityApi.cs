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
using static Google.Apis.YouTube.v3.CommentsResource.ListRequest;
using ExecutionContext = Microsoft.Azure.WebJobs.ExecutionContext;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using System.Net.Security;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using CommentQuality.RestApi.Models;
using System;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;

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
            commentThreadListRequest.MaxResults = 100;
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
            TextFormatEnum textFormat = TextFormatEnum.PlainText;
            TextFormatEnum.TryParse(textFormatStr, out textFormat);

            var commentsListRequest = youTubeDataApi.GetCommentListRequest(part, parentId, textFormat, pageToken);
            commentsListRequest.MaxResults = 100;
            var comments = await commentsListRequest.ExecuteAsync();

            return new OkObjectResult(comments);
        }

        private static ITextAnalyticsClient _textAnalyticsClient;

        [FunctionName("GetTextSentiment_Azure")]
        public static async Task<IActionResult> GetTextSentimentAzure(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "text/sentiment/azure")]HttpRequest request,
            TraceWriter traceWriter,
            ExecutionContext executionContext
        )
        {
            var appsettings = new AppSettings(executionContext);
            var azureCognitiveServicesConfig = appsettings.AzureCognitiveServicesConfig;

            _textAnalyticsClient = _textAnalyticsClient ?? GetTextAnalyticsClient(azureCognitiveServicesConfig);

            var documentBatch = JsonConvert.DeserializeObject<DocumentBatch>(await GetBodyAsString(request));

            var documents = documentBatch.Documents.Select((Document doc) => new MultiLanguageInput
            {
                Id = doc.Id,
                Language = doc.LanguageCode,
                Text = doc.Text
            }).ToList();

            try
            {
                var result = await _textAnalyticsClient.SentimentAsync(new MultiLanguageBatchInput(documents));

                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                string error = ex.ToString() + ex.InnerException?.ToString();
                traceWriter.Error(error, ex);
                return new BadRequestObjectResult(error);
            }
        }


        private static async Task<string> GetBodyAsString(HttpRequest request)
        {
            var bodyStr = string.Empty;
            using (var bodyStreamReader = new StreamReader(request.Body))
            {
                bodyStr = await bodyStreamReader.ReadToEndAsync();
            }
            return bodyStr;
        }

        private static ITextAnalyticsClient GetTextAnalyticsClient(AzureCognitiveServicesConfig config)
        {
            var textAnalyticsClient = new TextAnalyticsClient(new ApiKeyServiceClientCredentials(config.ApiKey));
            textAnalyticsClient.BaseUri = new Uri(config.EndpointUrl);

            return textAnalyticsClient;
        }
    }
}