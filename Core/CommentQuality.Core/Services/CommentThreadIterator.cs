using System.Collections.Generic;
using CommentQuality.Core.Interfaces;
using Google.Apis.YouTube.v3.Data;

namespace CommentQuality.Core.Services
{
    public class CommentThreadIterator : ICommentThreadIterator
    {
        private readonly IRestApi _restApi;
        private string nextCommentThreadsBatchToken = null;

        public CommentThreadIterator(IRestApi restApi)
        {
            _restApi = restApi;
        }

        public IEnumerable<CommentThread> GetCommentThreads(string part, string videoId)
        {
            do
            {
                var resp = _restApi.GetCommentThreads(videoId, part, nextCommentThreadsBatchToken).GetAwaiter().GetResult();
                if (resp == null)
                {
                    nextCommentThreadsBatchToken = null;
                    yield break;
                }

                nextCommentThreadsBatchToken = resp.NextPageToken != nextCommentThreadsBatchToken ? resp.NextPageToken : null;

                foreach (var commentThread in resp.Items)
                {
                    yield return commentThread;
                }
            } while (nextCommentThreadsBatchToken != null);
        }
    }
}