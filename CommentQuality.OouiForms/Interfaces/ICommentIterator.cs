using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System.Collections.Generic;

namespace CommentQuality.OouiForms.Interfaces
{
    public interface ICommentIterator
    {
        IEnumerable<Comment> GetComments(string part, string parentId, CommentsResource.ListRequest.TextFormatEnum textFormat);
    }
}