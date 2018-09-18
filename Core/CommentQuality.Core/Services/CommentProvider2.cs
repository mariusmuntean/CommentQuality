using System.Collections.Generic;
using CommentQuality.Core.Interfaces;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace CommentQuality.Core.Services
{
    public class CommentProvider2 : ICommentProvider2
    {
        private readonly ICommentThreadProvider _commentThreadProvider;
        private readonly ICommentIterator _commentIterator;

        private IEnumerator<Comment> _commentsIter;
        private Comment _peekNextComment;

        public CommentProvider2(ICommentThreadProvider commentThreadProvider, ICommentIterator commentIterator)
        {
            _commentThreadProvider = commentThreadProvider;
            _commentIterator = commentIterator;
        }

        public Comment GetNextComment()
        {
            EnsureInitialized();

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

        private IEnumerable<Comment> GetComments()
        {
            CommentThread commentThread = null;
            while ((commentThread = _commentThreadProvider.GetNextCommentThread()) != null)
            {
                // Get the top comment
                yield return commentThread.Snippet.TopLevelComment;

                // Finally get the rest of the comment in the comment thread
                foreach (var comment in _commentIterator.GetComments("snippet", commentThread.Id,
                    CommentsResource.ListRequest.TextFormatEnum.PlainText))
                {
                    yield return comment;
                }
            }
        }

        private void EnsureInitialized()
        {
            if (_commentsIter == null)
                _commentsIter = GetComments().GetEnumerator();
        }
    }
}