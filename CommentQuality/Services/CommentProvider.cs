using System.Collections.Generic;
using System.Linq;
using CommentQuality.Interfaces;
using CommentQuality.Models;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace CommentQuality.Services
{
    public class CommentProvider : ICommentProvider
    {
        private readonly ICommentThreadIterator _commentThreadIterator;
        private readonly ICommentIterator _commentIterator;

        private IEnumerator<Comment> _commentsIter;
        private Comment _peekNextComment;

        private bool _initialized = false;

        public CommentProvider(ICommentThreadIterator commentThreadIterator,
            ICommentIterator commentIterator)
        {
            _commentThreadIterator = commentThreadIterator;
            _commentIterator = commentIterator;
        }

        public void Init(string videoId)
        {
            _commentsIter = GetComments(videoId).GetEnumerator();
            _initialized = true;
        }

        public Comment GetNextComment()
        {
            AssertInit();

            if (_peekNextComment != null)
            {
                var tmpComment = _peekNextComment;
                _peekNextComment = null;
                return tmpComment;
            }

            if (_commentsIter.MoveNext())
            {
                return _commentsIter.Current;
            }

            return null;
        }

        public Comment PeekNextComment
        {
            get
            {
                if (_peekNextComment == null)
                {
                    _peekNextComment = GetNextComment();
                }

                return _peekNextComment;
            }
        }

        public bool HasNextComment()
        {
            return PeekNextComment != null;
        }

        private IEnumerable<Comment> GetComments(string videoId)
        {
            foreach (var commentThread in _commentThreadIterator.GetCommentThreads("snippet,replies", videoId))
            {
                // Get the top comment
                yield return commentThread.Snippet.TopLevelComment;
                // Get the replies to the top comment
                if (commentThread.Replies != null && commentThread.Replies.Comments.Any())
                {
                    foreach (var repliesComment in commentThread.Replies?.Comments)
                    {
                        yield return repliesComment;
                    }
                }

                // Finally get the rest of the comment in the comment thread
                foreach (var comment in _commentIterator.GetComments("snippet", commentThread.Id, CommentsResource.ListRequest.TextFormatEnum.PlainText))
                {
                    yield return comment;
                }
            }
        }

        private void AssertInit()
        {
            if (!_initialized)
            {
                throw new NotInitializedException("The comment provider was not initialized");
            }
        }
    }
}