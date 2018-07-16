using System.Net.Http;
using System.Threading.Tasks;
using Google.Apis.YouTube.v3.Data;
using Newtonsoft.Json;

namespace CommentQuality.OouiForms.Stuff
{
    internal class RestApi
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

        private string _commentsFromThreadRoute =
            "/video/{parentId}/comments?part={part}&pageToken={pageToken}&textFormat={textFormat}";

        private HttpClient _httpClient;

        public RestApi()
        {
            _httpClient = new HttpClient();
        }

        public async Task<string> GetCommentCount(string videoId)
        {
            _httpClient = new HttpClient();
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
    }
}