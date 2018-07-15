using System.Net.Http;
using System.Threading.Tasks;

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

        public async Task<string> GetCommentCount(string videoId)
        {
            var httpClient = new HttpClient();
            var requestUrl = _baseUrl + _commentCountRoute.Replace("{videoId}", videoId);
            var response = await httpClient.GetAsync(requestUrl);

            return await response.Content.ReadAsStringAsync();
        }
    }
}