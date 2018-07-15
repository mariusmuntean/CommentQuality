using System;
using System.Collections.Generic;
using CommentQuality.RestApi.Interfaces;
using CommentQuality.RestApi.Models;
using Google.Apis.YouTube.v3.Data;

namespace CommentQuality.RestApi.Services
{
    /// <summary>
    ///     Thread-safe <see cref="CommentThread" /> provider, designed to be shared by multiple workers
    /// </summary>
    public class CommentThreadProvider : IDisposable, ICommentThreadProvider
    {
        private readonly ICommentThreadIterator _commentThreadIterator;
        private readonly object _lock;
        private IEnumerator<CommentThread> _commentThreadEnumerator;

        public CommentThreadProvider(ICommentThreadIterator commentThreadIterator)
        {
            _commentThreadIterator = commentThreadIterator;
            _lock = new object();
        }

        public void Init(string videoId, string part)
        {
            _commentThreadEnumerator = _commentThreadIterator.GetCommentThreads(part, videoId).GetEnumerator();
        }

        public CommentThread GetNextCommentThread()
        {
            if (_commentThreadEnumerator == null) throw new NotInitializedException("Call Init() first!");

            lock (_lock)
            {
                if (_commentThreadEnumerator.MoveNext()) return _commentThreadEnumerator.Current;
            }

            return null;
        }

        public void Dispose()
        {
            _commentThreadEnumerator?.Dispose();
        }
    }
}