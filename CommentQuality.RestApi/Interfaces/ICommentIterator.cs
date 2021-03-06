﻿using System.Collections.Generic;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace CommentQuality.RestApi.Interfaces
{
    public interface ICommentIterator
    {
        IEnumerable<Comment> GetComments(string part, string parentId, CommentsResource.ListRequest.TextFormatEnum textFormat);
    }
}