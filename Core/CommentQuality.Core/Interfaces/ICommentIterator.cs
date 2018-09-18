using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System.Collections.Generic;

namespace CommentQuality.Core.Interfaces
{
    public interface ICommentIterator
    {
        IEnumerable<Comment> GetComments(string part, string parentId, CommentsResource.ListRequest.TextFormatEnum textFormat);
    }
}