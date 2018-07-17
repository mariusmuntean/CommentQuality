using CommentQuality.OouiForms.Interfaces;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System.Collections.Generic;

namespace CommentQuality.OouiForms.Services
{
    public class CommentIterator : ICommentIterator
    {
        private string nextCommentsBatchToken = null;
        private readonly IRestApi _restApi;

        public CommentIterator(IRestApi restApi)
        {
            _restApi = restApi;
        }

        public IEnumerable<Comment> GetComments(string part, string parentId, CommentsResource.ListRequest.TextFormatEnum textFormat)
        {
            do
            {
                var comments = _restApi.GetCommentsFromThread(parentId, part, textFormat.ToString(), nextCommentsBatchToken).GetAwaiter().GetResult();
                if (comments == null)
                {
                    nextCommentsBatchToken = null;
                    yield break;
                }

                nextCommentsBatchToken = comments.NextPageToken != nextCommentsBatchToken ? comments.NextPageToken : null;

                foreach (var comment in comments.Items)
                {
                    yield return comment;
                }
            } while (nextCommentsBatchToken != null);
        }
    }
}