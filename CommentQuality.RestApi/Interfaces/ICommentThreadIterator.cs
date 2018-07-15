using System.Collections.Generic;
using Google.Apis.YouTube.v3.Data;

namespace CommentQuality.RestApi.Interfaces
{
    public interface ICommentThreadIterator
    {
        IEnumerable<CommentThread> GetCommentThreads(string part, string videoId);
    }
}