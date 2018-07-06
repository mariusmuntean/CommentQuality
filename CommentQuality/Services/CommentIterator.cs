using System.Collections.Generic;
using CommentQuality.Interfaces;
using Google.Apis.YouTube.v3.Data;
using static Google.Apis.YouTube.v3.CommentsResource.ListRequest;

namespace CommentQuality.Services
{
    public class CommentIterator : ICommentIterator
    {
        string nextCommentsBatchToken = null;
        private readonly IYouTubeDataApi _youTubeDataApi;

        public CommentIterator(IYouTubeDataApi youTubeDataApi)
        {
            _youTubeDataApi = youTubeDataApi;
        }

        public IEnumerable<Comment> GetComments(string part, string parentId, TextFormatEnum textFormat)
        {
            do
            {
                var commentListReq = _youTubeDataApi.GetCommentListRequest(part, parentId, textFormat, nextCommentsBatchToken);

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
