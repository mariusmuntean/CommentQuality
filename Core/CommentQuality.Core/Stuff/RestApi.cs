using CommentQuality.Core.Interfaces;
using CommentQuality.Core.Models;
using Google.Apis.YouTube.v3.Data;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace CommentQuality.Core.Stuff
{
    public class RestApi : IRestApi
    {
        /**
         * GetCommentCount: http://localhost:7071/api/video/{videoId}/commentCount

        GetComments: http://localhost:7071/api/youtube/comments/{videoId}

        GetCommentsFromThread: http://localhost:7071/api/video/{parentId}/comments

        GetCommentThreads: http://localhost:7071/api/video/{videoId}/commentThreads
         */

        private string _baseUrl = "http://localhost:7071/api";
        private string _commentCountRoute = "/video/{videoId}/commentCount";
        private string _commentThreadsRoute = "/video/{videoId}/commentThreads?part={part}&pageToken={pageToken}";

        private string _textsentimentAzure = "/text/sentiment/azure";
        private string _textsentimentGoogle = "/text/sentiment/google";

        private string _commentsFromThreadRoute =
            "/video/{parentId}/comments?part={part}&pageToken={pageToken}&textFormat={textFormat}";

        private readonly HttpClient _httpClient;

        public RestApi()
        {
            _httpClient = new HttpClient();
        }

        public RestApi(HttpMessageHandler handler)
        {
            _httpClient = new HttpClient(handler);
        }

        public async Task<string> GetCommentCount(string videoId)
        {
            var requestUrl = _baseUrl + _commentCountRoute
                                 .Replace("{videoId}", videoId);
            var response = await _httpClient.GetAsync(requestUrl);

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<CommentThreadListResponse> GetCommentThreads(string videoId, string part, string pageToken)
        {
            var requestUrl = _baseUrl + _commentThreadsRoute
                                 .Replace("{videoId}", videoId)
                                 .Replace("{part}", part)
                                 .Replace("{pageToken}", pageToken);
            var response = await _httpClient.GetAsync(requestUrl);

            var responseStr = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<CommentThreadListResponse>(responseStr);
        }

        public async Task<CommentListResponse> GetCommentsFromThread(string parentId,
            string part,
            string textFormat,
            string pageToken)
        {
            var requestUrl = _baseUrl + _commentsFromThreadRoute.Replace("{parentId}", parentId)
                                 .Replace("{part}", part)
                                 .Replace("{pageToken}", pageToken)
                                 .Replace("{textFormat}", textFormat);

            var response = await _httpClient.GetAsync(requestUrl);
            var responseStr = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<CommentListResponse>(responseStr);
        }

        public async Task<DocumentBatchSentiment> GetTextSentimentAzure(DocumentBatch documentBatch)
        {
            var requestUrl = _baseUrl + _textsentimentAzure;
            //Console.WriteLine(JsonConvert.SerializeObject(documentBatch, Formatting.Indented));
            var response = await _httpClient.PostAsync(requestUrl,
                new StringContent(JsonConvert.SerializeObject(documentBatch)));
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception(response.ReasonPhrase);
            }

            var responseStr = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<DocumentBatchSentiment>(responseStr);
        }

        public async Task<DocumentBatchSentiment> GetTextSentimentGoogle(DocumentBatch documentBatch)
        {
            var requestUrl = _baseUrl + _textsentimentGoogle;
            //Console.WriteLine(JsonConvert.SerializeObject(documentBatch, Formatting.Indented));
            var response = await _httpClient.PostAsync(requestUrl,
                new StringContent(JsonConvert.SerializeObject(documentBatch)));
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception(response.ReasonPhrase);
            }

            var responseStr = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<DocumentBatchSentiment>(responseStr);
        }
    }
}