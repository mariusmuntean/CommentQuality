using System.Collections.Generic;
using CommentQuality.RestApi.Interfaces;
using Google.Apis.YouTube.v3.Data;

namespace CommentQuality.RestApi.Services
{
    public class CommentThreadIterator : ICommentThreadIterator
    {
        private readonly IYouTubeDataApi _youTubeDataApi;
        string nextCommentThreadsBatchToken = null;

        public CommentThreadIterator(IYouTubeDataApi youTubeDataApi)
        {
            _youTubeDataApi = youTubeDataApi;
        }

        public IEnumerable<CommentThread> GetCommentThreads(string part, string videoId)
        {
            do
            {
                var listReq = _youTubeDataApi.GetCommentThreadListRequest(part, videoId, nextCommentThreadsBatchToken);
                listReq.MaxResults = 100;
                var resp = listReq.ExecuteAsync().GetAwaiter().GetResult();
                nextCommentThreadsBatchToken = resp.NextPageToken != nextCommentThreadsBatchToken ? resp.NextPageToken : null;

                foreach (var commentThread in resp.Items)
                {
                    yield return commentThread;
                }

            } while (nextCommentThreadsBatchToken != null);
        }

    }
}
