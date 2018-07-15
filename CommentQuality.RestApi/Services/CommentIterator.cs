using System.Collections.Generic;
using CommentQuality.RestApi.Interfaces;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace CommentQuality.RestApi.Services
{
    public class CommentIterator : ICommentIterator
    {
        string nextCommentsBatchToken = null;
        private readonly IYouTubeDataApi _youTubeDataApi;

        public CommentIterator(IYouTubeDataApi youTubeDataApi)
        {
            _youTubeDataApi = youTubeDataApi;
        }

        public IEnumerable<Comment> GetComments(string part, string parentId, CommentsResource.ListRequest.TextFormatEnum textFormat)
        {
            do
            {
                var commentListReq = _youTubeDataApi.GetCommentListRequest(part, parentId, textFormat, nextCommentsBatchToken);
                commentListReq.MaxResults = 100;
                var comments = commentListReq.ExecuteAsync().GetAwaiter().GetResult();
                nextCommentsBatchToken = comments.NextPageToken != nextCommentsBatchToken ? comments.NextPageToken : null;

                foreach (var comment in comments.Items)
                {
                    yield return comment;
                }
            } while (nextCommentsBatchToken != null);
        }
    }
}
