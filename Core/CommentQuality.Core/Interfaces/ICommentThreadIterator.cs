using Google.Apis.YouTube.v3.Data;
using System.Collections.Generic;

namespace CommentQuality.Core.Interfaces
{
    public interface ICommentThreadIterator
    {
        IEnumerable<CommentThread> GetCommentThreads(string part, string videoId);
    }
}